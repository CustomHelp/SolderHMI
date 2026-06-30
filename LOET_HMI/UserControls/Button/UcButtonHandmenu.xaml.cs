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

namespace LOET_HMI
{
    // Quelle: https://stackoverflow.com/questions/364928/wpf-how-to-get-a-usercontrol-to-inherit-a-button

    /// <summary>
    /// Interaktionslogik für UcButtonHandmenu.xaml
    /// </summary>
    public partial class UcButtonHandmenu : Button, INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty _DpStationsID = DependencyProperty.Register(
            "DpStationsID", typeof(eStationsHMI), typeof(UcButtonHandmenu), new PropertyMetadata(new eStationsHMI()));

        public eStationsHMI DpStationsID
        {
            get { return (eStationsHMI)GetValue(_DpStationsID); }
            set { SetValue(_DpStationsID, value); }
        }


        public UcButtonHandmenu()
        {
            InitializeComponent();
        }
    }
}
