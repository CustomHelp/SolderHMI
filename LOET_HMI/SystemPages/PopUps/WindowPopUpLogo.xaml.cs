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
using System.Windows.Shapes;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpLogo.xaml
    /// </summary>
    public partial class WindowPopUpLogo : Window
    {
        private int counter;
        public WindowPopUpLogo()
        {
            InitializeComponent();
            counter = 0;
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            counter++;
            if (counter == 7)
            {
                MessageBox.Show("Entwickler-Modus gestartet.", "EasterEgg", MessageBoxButton.OK, MessageBoxImage.Information);
                counter = 0;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

