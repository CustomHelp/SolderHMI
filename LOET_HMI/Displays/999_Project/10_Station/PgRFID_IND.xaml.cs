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

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgRFID_IND.xaml
    /// </summary>
    public partial class PgRFID_IND : Page
    {
        public PgRFID_IND(int _iStation)
        {
            InitializeComponent();

            UcRFID_IND_Read.iGVLArrInd_Station = _iStation;
            UcRFID_IND_Write.iGVLArrInd_Station = _iStation;
            UcRFID_IND_Write.ucReadDataLink = UcRFID_IND_Read;
            
        }


        private void PgRFID_IND_Loaded(object sender, RoutedEventArgs e)
        {
            UcRFID_IND_Read.ADSRegister();
        }

        private void PgRFID_IND_Unloaded(object sender, RoutedEventArgs e)
        {
            UcRFID_IND_Read.ADSDeregister();
        }
    }
}
