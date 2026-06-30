using LiveCharts;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using LOET_HMI.PLC_Com_Classes;
using TwinCAT.Ads;
using System.IO;


namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgSt1_ForceDiagramm.xaml
    /// </summary>
    public partial class PgTempPowerDiagramm : Page
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // *******************************************************************************************
        // ******************************* Allgemeine-Variablen **************************************
        // *******************************************************************************************
        public const int ciBufArrLength = 10;
        private int iIndRXArry;

        // *******************************************************************************************
        // ********************************** Empfangene Daten ***************************************
        // *******************************************************************************************
        private bool _bRXDataTransferActive;
        public bool bRXDataTransferActive
        {
            get { return _bRXDataTransferActive; }
            set
            {
                _bRXDataTransferActive = value;
                if(_bRXDataTransferActive)
                {
                    ChartsClearData();

                    stLineTempMelt.ReadPLCArray(); // Vorsicht, aktuell wird in dieses Array ein einziger Wert eingelesen!
                    stLineTempSolder.ReadPLCArray(); // Vorsicht, aktuell wird in dieses Array ein einziger Wert eingelesen!
                    fTempMelt   = float.Parse( stLineTempMelt.sListOfRXData[0]);
                    fTempSolder = float.Parse( stLineTempSolder.sListOfRXData[0]);

                    RegisterBufArrays();                  
                }
                else
                {
                    DeregisterBufArrays();
                }
                OnPropertyChanged();
            }
        }


        private float[] _fRXArrTemp;
        public float[] fRXArrTemp
        {
            get { return _fRXArrTemp; }
            set
            {
                _fRXArrTemp = value;
                OnPropertyChanged();
            }
        }

        private float[] _fRXArrPower;
        public float[] fRXArrPower
        {
            get { return _fRXArrPower; }
            set
            {
                _fRXArrPower = value;
                OnPropertyChanged();
            }
        }

        private float _fTempMelt;
        public float fTempMelt
        {
            get { return _fTempMelt; }
            set
            {
                _fTempMelt = value;
                OnPropertyChanged();
            }
        }

        private float _fTempSolder;
        public float fTempSolder
        {
            get { return _fTempSolder; }
            set
            {
                _fTempSolder = value;
                OnPropertyChanged();
            }
        }

        // *******************************************************************************************
        // ************************************ ADS-Variablen ****************************************
        // *******************************************************************************************
        private TcAdsClient tcClient;
        private int[] hConnect;

        private AdsStream dataStreamVars; // für Variablen
        private BinaryReader binReadVars;

        private AdsStream dataStreamTemp; // für Puffer-Array Temperatur
        private BinaryReader binReadTemp;

        private AdsStream dataStreamPower; // für Puffer-Array Leistung
        private BinaryReader binReadPower;

        List<string> listRXbufferTemp { get; set; } // Puffer für ständiges Auslesen (OnChanged)
        List<string> listRXbufferPower { get; set; } // Puffer für ständiges Auslesen (OnChanged)


        public StArrPLC stLineTempMelt { get; set; }   // für einmaliges Auslesen
        public StArrPLC stLineTempSolder { get; set; } // für einmaliges Auslesen

        // *******************************************************************************************
        // ************************************ OxyPlot-Variablen ************************************
        // *******************************************************************************************
        #region OxyPlot-Variablen 
        OxyPlot.Series.LineSeries seriesTemp;
        OxyPlot.Series.LineSeries seriesPower;

        OxyPlot.Series.LineSeries seriesLineMelt;
        OxyPlot.Series.LineSeries seriesLineSolder;

        public ChartValues<float> cvTemp { get; set; }
        public ChartValues<float> cvTime { get; set; }
        public ChartValues<float> cvPower { get; set; }

        public ChartValues<float> cvLineMelt { get; set; }
        public ChartValues<float> cvLineSolder { get; set; }

        public OxyPlot.PlotModel Model1 { get; private set; }
        public int iSampleSize = 15000;
        #endregion


        // *******************************************************************************************
        // ************************* Weitere Variablen/Properties ************************************
        // *******************************************************************************************
        public int iStation { get; set; }
            
        private DispatcherTimer timer = new DispatcherTimer();
       


        public PgTempPowerDiagramm(int _iStationNr)
        {
            iStation = _iStationNr;

            //DpPLCArraySync = new StPLCArraySync(ciBufArrLength);
            fRXArrTemp = new float[ciBufArrLength];
            fRXArrPower = new float[ciBufArrLength];

            fTempMelt   = (float)30.0;
            fTempSolder = (float)40.0;


            stLineTempMelt   = new StArrPLC("GVL_Project.g_arrEvaluation[" + iStation.ToString() + "].rlTempMelt_PLCtoHMI",   eArrayType.AT_300_REAL, 1);
            stLineTempSolder = new StArrPLC("GVL_Project.g_arrEvaluation[" + iStation.ToString() + "].rlTempSolder_PLCtoHMI", eArrayType.AT_300_REAL, 1);

            InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();

            RegisterStartBit();

            ChartsInitData();
            ChartsSetup();
            DataContext = this;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                //Refresh();
            }));
        }

        private void DoUpdateGraphNow()
        {
            ;
        }

        public void ChartsInitData()
        {
            cvTemp  = new ChartValues<float>();
            cvTime  = new ChartValues<float>();
            cvPower = new ChartValues<float>();

            cvLineMelt   = new ChartValues<float>();
            cvLineSolder = new ChartValues<float>();

            for (int i = 0; i < iSampleSize; i++)
            {
                cvTime.Add((float)0.0);
                cvTemp.Add((float)0.0);             
                cvPower.Add((float)0.0);
            }

            cvLineMelt.Add((float)0.0);
            cvLineSolder.Add((float)0.0);
        }

        private void ChartsClearData()
        {
            iIndRXArry = 0;

            // Alte Werte aus Arrays löschen 
            cvTemp.Clear();
            cvTime.Clear();
            cvPower.Clear();

            cvLineMelt.Clear();
            cvLineSolder.Clear();

            // Array mit 0 initialisieren
            for (int i = 0; i < iSampleSize; i++)
            {
                cvTime.Add((float)0.0);
                cvTemp.Add((float)0.0);
                cvPower.Add((float)0.0);
            }

            cvLineMelt.Add((float)0.0);
            cvLineSolder.Add((float)0.0);
        }

       

        private void ChartsRefresh()
        {
            float fTimeScaling = 0.1f;

            for (int i = 0; i < ciBufArrLength; i++)
            {
                if (iIndRXArry <= iSampleSize)
                {
                    cvTemp[iIndRXArry] = fRXArrTemp[i];
                    cvPower[iIndRXArry] = fRXArrPower[i];
                    cvTime[iIndRXArry] = (float)iIndRXArry * fTimeScaling; // anpassen"
                    iIndRXArry++;
                }
                else
                {
                    //for(int index = 0; index < iSampleSize-1; index++)
                    //{
                    //    cv1temp[index] = cv1temp[index + 1];
                    //    cv1power[index] = cv1power[index + 1];
                    //    cv1time[index] = cv1time[index + 1];
                    //}
                    //cv1temp[iSampleSize - 1] = fRXArrTemp[i];
                    //cv1power[iSampleSize - 1] = fRXArrPower[i];
                    //cv1time[iSampleSize - 1] = fRXArrTemp[i];
                }
            }
            if(iIndRXArry <= iSampleSize)
            {
                for (int k = iIndRXArry; k<iSampleSize; k++)
                {
                    cvTemp[k]  = cvTemp[iIndRXArry - 1];
                    cvPower[k] = cvPower[iIndRXArry - 1];
                    cvTime[k]  = cvTime[iIndRXArry - 1];

                }
            }


            cvLineMelt[0] = fTempMelt;
            cvLineSolder[0] = fTempSolder;


            chartTempPower.Model.Series.Clear();
            seriesTemp.Points.Clear();
            seriesPower.Points.Clear();
            seriesLineMelt.Points.Clear();
            seriesLineSolder.Points.Clear();
            for (int i = 0; i < cvTemp.Count; i++)
            {
                seriesTemp.Points.Add(new DataPoint(cvTime[i], cvTemp[i]));              
            }
            for(int i=0; i< cvPower.Count; i++)
            {
                seriesPower.Points.Add(new DataPoint(cvTime[i], cvPower[i]));
            }

            // Linie durch seine 2 Endpunkte definieren
            seriesLineMelt.Points.Add(new DataPoint(cvTime[0],              cvLineMelt[0]));
            seriesLineMelt.Points.Add(new DataPoint(cvTime[iIndRXArry],     cvLineMelt[0]));

            seriesLineSolder.Points.Add(new DataPoint(cvTime[0],            cvLineSolder[0]));
            seriesLineSolder.Points.Add(new DataPoint(cvTime[iIndRXArry],   cvLineSolder[0]));

            chartTempPower.Model.Series.Add(seriesTemp);
            chartTempPower.Model.Series.Add(seriesPower);
            chartTempPower.Model.Series.Add(seriesLineMelt);
            chartTempPower.Model.Series.Add(seriesLineSolder);
            chartTempPower.Model.InvalidatePlot(true);
            chartTempPower.InvalidateVisual();
        }

        private void ChartsSetup()
        {
            var tmpPlotModel = new PlotModel { Title = Properties.Resources.ProjEval_DiagramTitle };
            tmpPlotModel.Axes.Add(new LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Title = Properties.Resources.ProjEval_Temperature + " [C°] / " + Properties.Resources.ProjEval_Power + " [W]" });
            tmpPlotModel.Axes.Add(new LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = Properties.Resources.ProjEval_Time + " [sec]" });

            seriesTemp = new OxyPlot.Series.LineSeries { Title = Properties.Resources.ProjEval_Temperature };
            seriesPower = new OxyPlot.Series.LineSeries { Title = Properties.Resources.ProjEval_Power };

            seriesLineMelt = new OxyPlot.Series.LineSeries { Title = Properties.Resources.ProjEval_Melt};
            seriesLineSolder = new OxyPlot.Series.LineSeries { Title = Properties.Resources.ProjEval_Solder};

            for (int i = 0; i< cvTemp.Count; i++)
            {
                seriesTemp.Points.Add(new DataPoint(cvTime[i], cvTemp[i]));
            }
            for (int i = 0; i < cvPower.Count; i++)
            {
                seriesPower.Points.Add(new DataPoint(cvTime[i], cvPower[i]));
            }

            tmpPlotModel.Series.Add(seriesTemp);
            tmpPlotModel.Series.Add(seriesPower);
            tmpPlotModel.Series.Add(seriesLineMelt);
            tmpPlotModel.Series.Add(seriesLineSolder);

            chartTempPower.Model = tmpPlotModel;
            chartTempPower.InvalidatePlot(true);
        }



        private void pgForceDiagramm_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void pgForceDiagramm_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            chartTempPower.ResetAllAxes();
        }


        private void RegisterStartBit()
        {

            tcClient = new TcAdsClient(); //Instanz der Klasse TcAdsClient erzeugen
            tcClient.Connect("", 851); // SPS: Lokal; //  Verbindung mit Port 801 auf dem lokalen Computer herstellen


            dataStreamVars = new AdsStream(31);
            binReadVars = new BinaryReader(dataStreamVars, System.Text.Encoding.ASCII); // Encoding wird auf ASCII gesetzt, um Strings lesen zu können

            hConnect = new int[3];


            try
            {
                hConnect[0] = tcClient.AddDeviceNotification("GVL_Project.g_arrEvaluation[" + iStation.ToString() + "].bTransmissionActive", dataStreamVars, 0, 1,
                                AdsTransMode.OnChange, 100, 0, null);

                tcClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void RegisterBufArrays()
        {
            int iArrComponentSize = 4; // TwinCAT REAL ist 4 Byte groß

            dataStreamTemp = new AdsStream(ciBufArrLength * iArrComponentSize);
            binReadTemp = new BinaryReader(dataStreamTemp, System.Text.Encoding.ASCII); // Encoding wird auf ASCII gesetzt, um Strings lesen zu können

            dataStreamPower = new AdsStream(ciBufArrLength * iArrComponentSize);
            binReadPower = new BinaryReader(dataStreamPower, System.Text.Encoding.ASCII); // Encoding wird auf ASCII gesetzt, um Strings lesen zu können


            try
            {
                hConnect[1] = tcClient.AddDeviceNotification("GVL_Project.g_arrEvaluation[" + iStation.ToString() + "].aBufTemp_PLCtoHMI", dataStreamTemp,
                                AdsTransMode.OnChange, 20, 0, null);
                hConnect[2] = tcClient.AddDeviceNotification("GVL_Project.g_arrEvaluation[" + iStation.ToString() + "].aBufPower_PLCtoHMI", dataStreamPower,
                                AdsTransMode.OnChange, 20, 0, null);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            e.DataStream.Position = e.Offset;
            string strValue = "";

            if (e.NotificationHandle == hConnect[0])
            {
                strValue = binReadVars.ReadBoolean().ToString();
                bRXDataTransferActive = Convert.ToBoolean(strValue);
            }
            else if ((e.NotificationHandle == hConnect[1]) && bRXDataTransferActive)
            {
                try
                {
                    listRXbufferTemp = new List<string>();

                    for (int i = 0; i < ciBufArrLength; i++)
                    {
                        listRXbufferTemp.Add(binReadTemp.ReadSingle().ToString());
                        fRXArrTemp[i] = Convert.ToSingle(listRXbufferTemp[i]);
                    }
                    // FRI
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        ChartsRefresh(); // Den Graph erst hier aktualisieren
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if ((e.NotificationHandle == hConnect[2]) && bRXDataTransferActive)
            {
                try
                {
                    listRXbufferPower = new List<string>();

                    for (int i = 0; i < ciBufArrLength; i++)
                    {
                        listRXbufferPower.Add(binReadPower.ReadSingle().ToString());
                        fRXArrPower[i] = Convert.ToSingle(listRXbufferPower[i]);
                    }

                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                    {
                        ChartsRefresh(); // Den Graph erst hier aktualisieren
                    }));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        public void DeregisterBufArrays()
        {
            try
            {
                if(hConnect[1] != 0 )
                    tcClient.DeleteDeviceNotification(hConnect[1]);
                if (hConnect[2] != 0)
                    tcClient.DeleteDeviceNotification(hConnect[2]);

                hConnect[1] = 0;
                hConnect[2] = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            //tcClient.Dispose();
        }


    }
}
