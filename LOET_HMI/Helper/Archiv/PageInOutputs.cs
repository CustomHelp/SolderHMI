using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LOET_HMI.Klassen
{
    class PageInOutputs : Page
    {
        public PageInOutputs()
        {
            this.Loaded += PageLoadedEvent;
        }

        private void PageLoadedEvent(object sender, RoutedEventArgs e)
        {
            //ucDatagridIn.SetDatagridSource(((MainWindow)Application.Current.MainWindow).rena_HMI.Act_Modul.SettingListIn);
            //ucDatagridOut.SetDatagridSource(((MainWindow)Application.Current.MainWindow).rena_HMI.Act_Modul.SettingListOut);
        }
    }
}
