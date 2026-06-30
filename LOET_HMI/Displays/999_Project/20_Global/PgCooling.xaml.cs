using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LOET_HMI.PLC_Com_Classes;
using System.Windows.Threading;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgCooling.xaml
    /// </summary>
    public partial class PgGlobal : Page
    {
        //********************************************************************************************
        // *****************************   Zylinder   ************************************************
        public static readonly DependencyProperty _CylList = DependencyProperty.Register(
            "CylList", typeof(List<StCylinder>), typeof(PgGlobal), new PropertyMetadata(new List<StCylinder>()));

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
            "DevList", typeof(List<StDeviceOnOff>), typeof(PgGlobal), new PropertyMetadata(new List<StDeviceOnOff>()));

        public List<StDeviceOnOff> DevList
        {
            get { return (List<StDeviceOnOff>)GetValue(_DevList); }
            set { SetValue(_DevList, value); }
        }
        public int iDevOnOffCount { get; set; }
        public int iDevOnOffArrayStartInd { get; set; }

        //********************************************************************************************
        // *****************************   HU5000   *******************************************
        public static readonly DependencyProperty _DpGenerator = DependencyProperty.Register(
            "DpGenerator", typeof(StHU5000), typeof(PgGlobal), new PropertyMetadata(new StHU5000()));

        public StHU5000 DpGenerator
        {
            get { return (StHU5000)GetValue(_DpGenerator); }
            set { SetValue(_DpGenerator, value); }
        }

        //********************************************************************************************
        //********************************************************************************************


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DispatcherTimer dipsTime = new DispatcherTimer();

        public PgGlobal()
        {
            //********************************************************************************************
            // *****************************   Zylinder   ************************************************
            iCylCount = 0;
            iCylArrayStartInd = 1;// (int)GVLarrNr_Cylinder.gcCYL_Bauteil1Klemmen;

            CylList = new List<StCylinder>();


            for (int i = 1; i <= iCylCount; i++)
                CylList.Add(new StCylinder());

            //********************************************************************************************
            // *****************************   Device On/Off   *******************************************
            iDevOnOffCount = 3;
            iDevOnOffArrayStartInd = (int)GVLarrNr_DevOnOff.gcDEV_St0_Vaccum;

            DevList = new List<StDeviceOnOff>();

            for (int i = 1; i <= iDevOnOffCount; i++)
                DevList.Add(new StDeviceOnOff());

            //********************************************************************************************
            //********************************************************************************************
            DpGenerator = new StHU5000();

            dipsTime.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dipsTime.Tick += DipsTime_Tick;

            InitializeComponent();
        }

        private void DipsTime_Tick(object sender, EventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Maintenance.iLevel)
            {
                elemGenerator.IsEnabled = false;
                dev1Vacuum.IsEnabled = false;
                dev2Generator.IsEnabled = false;
                dev3Cooler.IsEnabled = false;
            }
            else
            {
                elemGenerator.IsEnabled = true;
                dev1Vacuum.IsEnabled = true;
                dev2Generator.IsEnabled = true;
                dev3Cooler.IsEnabled = true;
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
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

            // Himmel HU5000
            DpGenerator.Register("GVL_Himmel.g_arrHimmel[1]");

            dipsTime.Start();

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
                CylList[i].Deregister();

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
                DevList[i].Deregister();

            // Himmel HU5000
            DpGenerator.Deregister();

            dipsTime.Stop();
        }
    }
}
