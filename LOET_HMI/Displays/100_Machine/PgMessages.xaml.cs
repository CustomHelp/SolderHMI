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

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgMessages.xaml
    /// </summary>
    public partial class PgMessages : Page
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private List<ADSItem> ItemList = new List<ADSItem>();

        //********************************************************************************************
        // *****************************   Zylinder   ************************************************
        public static readonly DependencyProperty _CylList = DependencyProperty.Register(
            "CylList", typeof(List<StCylinder>), typeof(PgMessages), new PropertyMetadata(new List<StCylinder>()));

        public List<StCylinder> CylList
        {
            get { return (List<StCylinder>)GetValue(_CylList); }
            set { SetValue(_CylList, value); }
        }

        public int iCylCount { get; set; }
        public int iCylArrayInd1 { get; set; }
        public int iCylArrayInd2 { get; set; }

        //********************************************************************************************
        // *****************************   Device On/Off   *******************************************
        public static readonly DependencyProperty _DevList = DependencyProperty.Register(
            "DevList", typeof(List<StDeviceOnOff>), typeof(PgMessages), new PropertyMetadata(new List<StDeviceOnOff>()));

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

        public PgMessages()
        {
            //********************************************************************************************
            // *****************************   Zylinder   ************************************************
            iCylCount = 2;
            iCylArrayInd1 = (int)GVLarrNr_Cylinder.gcCYL_St10_Maintenance_Solder;
            iCylArrayInd2 = (int)GVLarrNr_Cylinder.gcCYL_St20_Maintenance_Solder;

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

            ItemList.Add(VarCon.AddItem("GVL_Basic.bSt10ShowQuitNIO", typeof(bool)));
            ItemList.Add(VarCon.AddItem("GVL_Basic.bSt20ShowQuitNIO", typeof(bool)));
            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent; ;

            InitializeComponent();
        }

        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (ItemList[i].iHandle == e.Item[j].iHandle)
                    {
                        ItemList[i].Value = e.Item[j].Value;
                    }
                }
            }

            if (ItemList.Count > 0)
            {
                bool showSt10 = Boolean.Parse(ItemList[0].Value.ToString());
                if(showSt10 == true)
                {
                    quitSt10Btn.Visibility = Visibility.Visible;
                }
                else
                {
                    quitSt10Btn.Visibility = Visibility.Hidden;
                }
                bool showSt20 = Boolean.Parse(ItemList[1].Value.ToString());
                if(showSt20 == true)
                {
                    quitSt20Btn.Visibility = Visibility.Visible;
                }
                else
                {
                    quitSt20Btn.Visibility = Visibility.Hidden;
                }
            }

        }

        private void Grid_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Grid_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void PgStat_Loaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
            {
                int index = 0;
                if (i == 0)
                {
                    index = iCylArrayInd1;
                }
                if (i == 1)
                {
                    index = iCylArrayInd2;
                }
                string box = GVLRefArrays.cCylinder + "[" + Convert.ToString(index) + "]";
                CylList[i].Register(GVLRefArrays.cCylinder + "[" + Convert.ToString(index) + "]");
            }

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
            {
                DevList[i].Register(GVLRefArrays.cDevices + "[" + Convert.ToString(iDevOnOffArrayStartInd + i) + "]");
            }
        }

        private void PgStat_Unloaded(object sender, RoutedEventArgs e)
        {
            // Zylinder
            for (int i = 0; i < CylList.Count; i++)
                CylList[i].Deregister();

            // Device On/Off
            for (int i = 0; i < DevList.Count; i++)
                DevList[i].Deregister();
        }

        private void QuitSt20Btn_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Quality.iLevel)
            {
                GlobalFunc.ShowNoAuthorization();
                return;
            }
            VarCon.WriteItem("GVL_Basic.bSt20QuitNIO", true);
        }

        private void QuitSt10Btn_Click(object sender, RoutedEventArgs e)
        {
            if(GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Quality.iLevel)
            {
                GlobalFunc.ShowNoAuthorization();
                return;
            }
            VarCon.WriteItem("GVL_Basic.bSt10QuitNIO", true);
        }
    }

}
