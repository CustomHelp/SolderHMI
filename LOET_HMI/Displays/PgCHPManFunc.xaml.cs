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

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgSt050.xaml
    /// </summary>
    public partial class PgCHPManFunc : Page
    {
        //********************************************************************************************
        // *****************************   Zylinder   ************************************************
        public static readonly DependencyProperty _CylList = DependencyProperty.Register(
            "CylList", typeof(List<StCylinder>), typeof(PgCHPManFunc), new PropertyMetadata(new List<StCylinder>()));

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
            "DevList", typeof(List<StDeviceOnOff>), typeof(PgCHPManFunc), new PropertyMetadata(new List<StDeviceOnOff>()));

        public List<StDeviceOnOff> DevList
        {
            get { return (List<StDeviceOnOff>)GetValue(_DevList); }
            set { SetValue(_DevList, value); }
        }
        public int iDevOnOffCount { get; set; }
        public int iDevOnOffArrayStartInd { get; set; }

        // ********************************************************************************************
        // *****************************   Achse   ****************************************************
        public static readonly DependencyProperty _DpAxis = DependencyProperty.Register(
            "DpAxis", typeof(StBeckhoffAxis), typeof(PgCHPManFunc), new PropertyMetadata(new StBeckhoffAxis()));

        public StBeckhoffAxis DpAxis
        {
            get { return (StBeckhoffAxis)GetValue(_DpAxis); }
            set { SetValue(_DpAxis, value); }
        }
        //********************************************************************************************
        //********************************************************************************************



        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public PgCHPManFunc()
        {
            //********************************************************************************************
            // *****************************   Zylinder   ************************************************
            iCylCount = 5;
            iCylArrayStartInd = 0;//(int)GVLarrNr_Cylinder.gcCYL_Bauteil1Klemmen;

            CylList = new List<StCylinder>(); // MBA 4.4.2021: wichtig bei der Sprachumschaltung. Wenn eine neue Instanz der Anwendung gestartet wird, muss eine neue Liste erstellt werden, sonst bleiben die Listenkomponenten der vorherigen Instanz in der Liste (???)

            for (int i = 1; i <= iCylCount; i++)
                CylList.Add(new StCylinder());

            //********************************************************************************************
            // *****************************   Device On/Off   *******************************************
            iDevOnOffCount = 3; // 6;
            iDevOnOffArrayStartInd = (int)GVLarrNr_DevOnOff.gcDEV_St0_Vaccum; // St30 + St50 zusammen

            DevList = new List<StDeviceOnOff>();

            for (int i = 1; i <= iDevOnOffCount; i++)
                DevList.Add(new StDeviceOnOff());

            //********************************************************************************************
            //********************************************************************************************

            InitializeComponent();
        }

        private void PgStat_Loaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
            {
                CylList[i].Register(GVLRefArrays.cCylinder + "[" + Convert.ToString(iCylArrayStartInd + i) + "]");
            }

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
            {
                DevList[i].Register(GVLRefArrays.cDevices + "[" + Convert.ToString(iDevOnOffArrayStartInd + i) + "]");
            }

            // Achse
            DpAxis.Register("GVL_Axis.g_arrAxisHMI[1]");
        }

        private void PgStat_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
                CylList[i].Deregister();

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
                DevList[i].Deregister();

            // Achse
            DpAxis.Deregister();
        }
    }
}
