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

namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcGrHeater.xaml
    /// </summary>
    public partial class UcGrHeater : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty _DpHeaterHMI = DependencyProperty.Register(
            "DpHeaterHMI", typeof(StDeviceOnOff), typeof(UcGrHeater), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpHeaterHMI
        {
            get { return (StDeviceOnOff)GetValue(_DpHeaterHMI); }
            set { SetValue(_DpHeaterHMI, value); }
        }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UcGrHeater()
        {
            InitializeComponent();
        }

        private void BorderClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DpHeaterHMI.WriteManualOn();
        }

        private void UcHeater_Loaded(object sender, RoutedEventArgs e)
        {
            borderClickArea.Background = Brushes.Transparent;
        }
    }
}
