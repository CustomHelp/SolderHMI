using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace LOET_HMI.Displays._999_Project._20_Global
{
    /// <summary>
    /// Interaction logic for PgSt20_ManFunc.xaml
    /// </summary>
    public partial class PgSt20_ManFunc : Page
    {

        // ADS Verbindung
        IADSConnection VarCon = new ADSService();

        //********************************************************************************************
        // *****************************   Zylinder   ************************************************
        public static readonly DependencyProperty _CylList = DependencyProperty.Register(
            "CylList", typeof(List<StCylinder>), typeof(PgSt20_ManFunc), new PropertyMetadata(new List<StCylinder>()));

        public List<StCylinder> CylList
        {
            get { return (List<StCylinder>)GetValue(_CylList); }
            set { SetValue(_CylList, value); }
        }

        public int iCylCount { get; set; }
        public int iCylArrayStartInd { get; set; }

        //********************************************************************************************
        // *****************************   Device On/Off   *******************************************
        public static readonly DependencyProperty _DevList = DependencyProperty.Register(
            "DevList", typeof(List<StDeviceOnOff>), typeof(PgSt20_ManFunc), new PropertyMetadata(new List<StDeviceOnOff>()));

        public List<StDeviceOnOff> DevList
        {
            get { return (List<StDeviceOnOff>)GetValue(_DevList); }
            set { SetValue(_DevList, value); }
        }
        public int iDevOnOffCount { get; set; }
        public int iDevOnOffArrayStartInd { get; set; }


        //********************************************************************************************
        //********************************************************************************************


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DispatcherTimer dipsTime = new DispatcherTimer();

        public PgSt20_ManFunc()
        {

            //********************************************************************************************
            // *****************************   Zylinder   ************************************************
            iCylCount = 9;
            iCylArrayStartInd = (int)GVLarrNr_Cylinder.gcCYL_St20_Dock_Inductor;

            CylList = new List<StCylinder>();


            for (int i = 1; i <= iCylCount; i++)
                CylList.Add(new StCylinder());

            //********************************************************************************************
            // *****************************   Device On/Off   *******************************************
            iDevOnOffCount = 0; // 6;
            iDevOnOffArrayStartInd = 0;

            DevList = new List<StDeviceOnOff>();

            for (int i = 1; i <= iDevOnOffCount; i++)
                DevList.Add(new StDeviceOnOff());

            dipsTime.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dipsTime.Tick += DipsTime_Tick;

            InitializeComponent();
        }

        private void DipsTime_Tick(object sender, EventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Maintenance.iLevel)
            {
                cylElem1.IsEnabled = false;
                cylElem2.IsEnabled = false;
                cylElem3.IsEnabled = false;
                cylElem4.IsEnabled = false;
                cylElem5.IsEnabled = false;
                cylElem6.IsEnabled = false;
                cylElem7.IsEnabled = false;
            }
            else
            {
                cylElem1.IsEnabled = true;
                cylElem2.IsEnabled = true;
                cylElem3.IsEnabled = true;
                cylElem4.IsEnabled = true;
                cylElem5.IsEnabled = true;
                cylElem6.IsEnabled = true;
                cylElem7.IsEnabled = true;
            }
        }

        private void PgStat_Loaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
            {
                string box = GVLRefArrays.cCylinder + "[" + Convert.ToString(iCylArrayStartInd + i) + "]";
                CylList[i].Register(GVLRefArrays.cCylinder + "[" + Convert.ToString(iCylArrayStartInd + i) + "]");
            }

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
            {
                DevList[i].Register(GVLRefArrays.cDevices + "[" + Convert.ToString(iDevOnOffArrayStartInd + i) + "]");
            }

            dipsTime.Start();
        }

        private void PgStat_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
                CylList[i].Deregister();

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
                DevList[i].Deregister();


            dipsTime.Stop();
        }

        private void ResetMeasuremet_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Maintenance.iLevel)
            {
                return;
            }
            VarCon.WriteItem("GVL_Project.bZeroMeasurementRH", true);
        }
    }
}
