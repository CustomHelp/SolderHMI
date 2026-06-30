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
using LOET_HMI.UserControls;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PageAuftrag.xaml
    /// </summary>
    public partial class PgHandmenuLayout : Page
    {
        public PgHandmenuLayout()
        {
            InitializeComponent();
        }

        public void RefreshHorizNavAndLoadDisplay(UcButtonHandmenu btnClicked)
        {
            Button btnTmp            = new Button(); // für die Schleißfe verwendet
            Button btnFoundInNavVert = new Button(); // der gesuchte Button in der vertikalen Navigation

            for (int i = 0; i < GlobalVar.navVert.Children.Count; i++)
            {
                try
                {
                    if (GlobalVar.dataLOET.listStatVertNav[i].eStationID == btnClicked.DpStationsID)
                    {
                        if(GlobalVar.dataLOET.listStatVertNav[i].eStationID != eStationsHMI._NoID)
                            btnFoundInNavVert = GlobalVar.dataLOET.listStatVertNav[i].btnVertNav;
                        else
                            MsgStationNotFound();
                        break;
                    }
                }
                catch
                {
                    MsgStationNotFound();
                }
            }
            GlobalFunc.RefreshNavVert(btnFoundInNavVert as Button);
        }

        private void MsgStationNotFound()
        {
            MessageBox.Show("Die Station ist mit keinem Display verknüpft.",
                            "Info",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
        }

        #region Alt
        public void LoadDisplay(StationVertNav _stationToLoad, RoutedEventArgs e) // Hier wird der Text in der horizontalen Navigation nicht aktualisiert....
        {
            Button btn = _stationToLoad.ClusterList1stRow[0].ButtonOfDispl;
            GlobalVar.navHoriz.ClusterListRow1 = _stationToLoad.ClusterList1stRow;
            GlobalVar.navHoriz.BtnRow1_Click(btn, e);
        }
        #endregion

        // ****************************************************************
        // ********************* Click-EventHandlers **********************
        // ****************************************************************
        private void BtnSt50_Laser_Click(object sender, RoutedEventArgs e)
        {
            RefreshHorizNavAndLoadDisplay(sender as UcButtonHandmenu);
        }

        private void BtnSt100_Kartonauf_Click(object sender, RoutedEventArgs e)
        {
            RefreshHorizNavAndLoadDisplay(sender as UcButtonHandmenu);
        }

        private void UcButtonHandmenu_Click(object sender, RoutedEventArgs e)
        {
            RefreshHorizNavAndLoadDisplay(sender as UcButtonHandmenu);
        }
    }
}
