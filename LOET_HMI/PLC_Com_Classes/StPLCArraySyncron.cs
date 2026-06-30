using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using TwinCAT.Ads;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;


namespace LOET_HMI.PLC_Com_Classes
{
    //////////////////////////////////////////
    //// MarshalAs (Struktur in der SPS) /////
    //////////////////////////////////////////
    #region    
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Array
    {
        #region
        /*
        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I2)] //VT_I2: short (2 Bytes) -> TwinCAT INT
        public short[] sArr;

        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_INT)] //VT_I2: short (4 Bytes) -> TwinCAT DINT (????? -> Prüfen)
        public int[] iArr;

        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType =VarEnum.VT_R4)] //VT_R4: float (4 Bytes) -> TwinCAT REAL
        public float[] fArr;

        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_R8)] //VT_R8: double (8 Bytes) -> TwinCAT LREAL
        public double[] dArr;
        */
        #endregion

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] //VT_R4: float (4 Bytes) -> TwinCAT REAL
        public float[] fArr;
    }
    #endregion

    public class StPLCArraySync : INotifyPropertyChanged
    {
        public string ADSName { get; set; }
        // ***** ADS Verbindung (ursprünglich)
        //IADSConnection VarCon = new ADSService();
        //private ADSItem Item = new ADSItem();


        // ***** ADS Verbindung (neu, MBA 22.04.2021)
        // Quelle: https://infosys.beckhoff.com/index.php?content=../content/1031/tcsample_vb/html/tcadsnet_vb_sample03.htm&id=
        //private TcAdsClient tcClient;
        //private AdsStream dataStream;
        //private BinaryReader binRead;

        private TcAdsClient tcClient;
        private int[] hConnect;
        private AdsStream dataStream;
        private BinaryReader binRead;
        private AdsStream dataStream2;
        private BinaryReader binRead2;

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private TextBox tbInt;
        private TextBox tbDint;
        private TextBox tbSint;
        private TextBox tbLreal;
        private TextBox tbReal;
        private TextBox tbString;
        private Label label11;
        private TextBox tbBool;
        private System.ComponentModel.Container components = null;

        List<string> listResult { get; set; }

        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region
        private int[] _iArrRX;
        public int[] iArrRX
        {
            get { return _iArrRX; }
            set
            {
                _iArrRX = value;
                OnPropertyChanged();
            }
        }

        private float[] _fArrRX;
        public float[] fArrRX
        {
            get { return _fArrRX; }
            set
            {
                _fArrRX = value;
                OnPropertyChanged();
            }
        }
        #endregion



        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region
        private int _iArrLength;
        public int iArrLength
        {
            get { return _iArrLength; }
            set
            {
                _iArrLength = value;
                OnPropertyChanged();
            }
        }

        private bool _bUpdated;
        public bool bRXDataUpdated
        {
            get { return _bUpdated; }
            set
            {
                _bUpdated = value;
                OnPropertyChanged();
            }
        }
        #endregion

        ////////////////////////////////////////
        ////////////// Konstruktor /////////////
        ////////////////////////////////////////
        #region
        public StPLCArraySync(int _iArrLength)
        {
            iArrLength = _iArrLength;
            fArrRX = new float[_iArrLength];
        }
        #endregion

        ////////////////////////////////////////
        ////////////// ADS - RX ////////////////
        ////////////////////////////////////////
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            // ************* ADS alt
            //Item.sName = ADSName;
            //Item.Type = typeof(ST_Array);

            //Item = VarCon.AddItem(ADSName, typeof(ST_Array));

            //VarCon.EnableCallbackEvent();
            //VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;


            // ************* ADS neu
            dataStream = new AdsStream(31);
            binRead = new BinaryReader(dataStream, System.Text.Encoding.ASCII); // Encoding wird auf ASCII gesetzt, um Strings lesen zu können

            dataStream2 = new AdsStream(10 * 4);
            binRead2 = new BinaryReader(dataStream2, System.Text.Encoding.ASCII); // Encoding wird auf ASCII gesetzt, um Strings lesen zu können

            tcClient = new TcAdsClient(); //Instanz der Klasse TcAdsClient erzeugen
            tcClient.Connect("", 851); // SPS: Lokal; //  Verbindung mit Port 801 auf dem lokalen Computer herstellen

            hConnect = new int[8];

            try
            {
                hConnect[0] = tcClient.AddDeviceNotification("GVL_Project.boolVal", dataStream, 0, 1,
                        AdsTransMode.OnChange, 100, 0, tbBool);
                hConnect[1] = tcClient.AddDeviceNotification("GVL_Project.intVal", dataStream, 1, 2,
                        AdsTransMode.OnChange, 100, 0, tbInt);
                hConnect[2] = tcClient.AddDeviceNotification("GVL_Project.dintVal", dataStream, 3, 4,
                        AdsTransMode.OnChange, 100, 0, tbDint);
                hConnect[3] = tcClient.AddDeviceNotification("GVL_Project.sintVal", dataStream, 7, 1,
                        AdsTransMode.OnChange, 100, 0, tbSint);
                hConnect[4] = tcClient.AddDeviceNotification("GVL_Project.lrealVal", dataStream, 8, 8,
                        AdsTransMode.OnChange, 100, 0, tbLreal);
                hConnect[5] = tcClient.AddDeviceNotification("GVL_Project.realVal", dataStream, 16, 4,
                        AdsTransMode.OnChange, 100, 0, tbReal);
                hConnect[6] = tcClient.AddDeviceNotification("GVL_Project.stringVal", dataStream, 20, 11,
                        AdsTransMode.OnChange, 100, 0, tbString);
                hConnect[7] = tcClient.AddDeviceNotification("GVL_Project.aBufTemp_PLCtoHMI", dataStream2,
                        AdsTransMode.OnChange, 100, 0, tbString);

                tcClient.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {
            DateTime time = DateTime.FromFileTime(e.TimeStamp);
            e.DataStream.Position = e.Offset;
            string strValue = "";

            if (e.NotificationHandle == hConnect[0])
                strValue = binRead.ReadBoolean().ToString();
            else if (e.NotificationHandle == hConnect[1])
                strValue = binRead.ReadInt16().ToString();
            else if (e.NotificationHandle == hConnect[2])
                strValue = binRead.ReadInt32().ToString();
            else if (e.NotificationHandle == hConnect[3])
                strValue = binRead.ReadSByte().ToString();
            else if (e.NotificationHandle == hConnect[4])
                strValue = binRead.ReadDouble().ToString();
            else if (e.NotificationHandle == hConnect[5])
                strValue = binRead.ReadSingle().ToString();
            else if (e.NotificationHandle == hConnect[6])
            {
                strValue = new String(binRead.ReadChars(11));
            }
            else if (e.NotificationHandle == hConnect[7])
            {
                try
                {
                    listResult = new List<string>();

                    for (int i = 0; i < iArrLength; i++)
                    {
                        listResult.Add(binRead2.ReadSingle().ToString());
                        fArrRX[i] = Convert.ToSingle(listResult[i]);
                    }
                    bRXDataUpdated = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
            //((TextBox)e.UserData).Text = String.Format("DateTime: {0},{1}ms; {2}", time, time.Millisecond, strValue);
        }





        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            // ************* ADS alt
            //for (int j = 0; j < e.Item.Count; j++)
            //{
            //    if (Item.iHandle == e.Item[j].iHandle)
            //    {
            //        Item = e.Item[j];
            //    }
            //}

            //if(Item.Value != null)
            //{
            //    try
            //    {
            //        ST_Array tmp = (ST_Array)Item.Value;

            //        for (int i=0; i< iArrLength; i++)
            //        {
            //            fArrRX[i] = tmp.fArr[i];
            //        }
            //        bRXDataUpdated = true;
            //    }
            //    catch
            //    {
            //        ;
            //    }
            //}

            // ************* ADS neu

        }



        public void Deregister()
        {
            // ************* ADS alt
            //VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            //VarCon.RemoveItem(Item);

            // ************* ADS neu
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    tcClient.DeleteDeviceNotification(hConnect[i]);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            tcClient.Dispose();
        }

        #endregion


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
