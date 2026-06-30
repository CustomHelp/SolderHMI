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
    /// Interaktionslogik für PgSt1_RFID.xaml
    /// </summary>
    public partial class PgSt1_RFID_WT : Page
    {
        public PgSt1_RFID_WT()
        {
            
            InitializeComponent();
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
