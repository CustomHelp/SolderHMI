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
    /// Interaktionslogik für PgRFID_WT.xaml
    /// </summary>
    public partial class PgRFID_WT : Page
    {    
        public PgRFID_WT(int _iStation)
        {
            InitializeComponent();

            UcRFID_WT_Read.iGVLArrInd_Station  = _iStation;
            UcRFID_WT_Write.iGVLArrInd_Station = _iStation;
            UcRFID_WT_Write.ucReadDataLink = UcRFID_WT_Read;
        }
        

        private void PgRFID_WT_Loaded(object sender, RoutedEventArgs e)
        {
            UcRFID_WT_Read.ADSRegister();
        }

        private void PgRFID_WT_Unloaded(object sender, RoutedEventArgs e)
        {
            UcRFID_WT_Read.ADSDeregister();
        }
    }
}
