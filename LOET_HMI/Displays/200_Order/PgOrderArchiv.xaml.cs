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
    /// Interaktionslogik für PgOrderArchiv.xaml
    /// </summary>
    public partial class PgOrderArchiv : Page
    {
        public PgOrderArchiv()
        {
            InitializeComponent();
        }

        private void PgOrdArchiv_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void PgOrdArchiv_Unloaded(object sender, RoutedEventArgs e)
        {

        }



        private void DGOrderArchiv_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        public void RefreshData()
        {
            dGOrderArchiv.ItemsSource = DBOrder.Handler.GetArchivOrderList();
            dGOrderArchiv.CanUserAddRows = false; // Ohne diesen Befehl erscheint eine zusätzliche leere Zeile
        }


    }


}



