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
    /// Interaktionslogik für UcGrPumpe.xaml
    /// </summary>
    public partial class UcGrPump : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty _DpPumpHMI = DependencyProperty.Register(
            "DpPumpHMI", typeof(StDeviceOnOff), typeof(UcGrPump), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpPumpHMI
        {
            get { return (StDeviceOnOff)GetValue(_DpPumpHMI); }
            set { SetValue(_DpPumpHMI, value); }
        }




        public static readonly DependencyProperty _Orientation = DependencyProperty.Register(
                "Orientation", typeof(OrientationPump), typeof(UcGrPump), new PropertyMetadata(OrientationPump.LeftToRight));

        public OrientationPump Orientation
        {
            get { return (OrientationPump)GetValue(_Orientation); }
            set {   SetValue(_Orientation, value); }
        }




        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UcGrPump()
        {
            InitializeComponent();


        }

        private void UcPump_Loaded(object sender, RoutedEventArgs e)
        {
            if (Orientation == OrientationPump.LeftToRight)
            {
                LineHoriz.Visibility = Visibility.Visible;
                LineVert.Visibility = Visibility.Hidden;

                Line1stQuadr.Visibility = Visibility.Hidden;
                Line2ndQuadr.Visibility = Visibility.Visible;
                Line3rdQuadr.Visibility = Visibility.Visible;
                Line4thQuadr.Visibility = Visibility.Hidden;
            }
            if (Orientation == OrientationPump.RightToLeft)
            {
                LineHoriz.Visibility = Visibility.Visible;
                LineVert.Visibility = Visibility.Hidden;

                Line1stQuadr.Visibility = Visibility.Visible;
                Line2ndQuadr.Visibility = Visibility.Hidden;
                Line3rdQuadr.Visibility = Visibility.Hidden;
                Line4thQuadr.Visibility = Visibility.Visible;
            }

            borderClickArea.Background = Brushes.Transparent;
        }

        private void BorderClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //DpPumpHMI.WriteManualOn(); auskommentiert, da es nicht mehr ein fb_DeviceOnOff ist. Es wird über RS232 angesteuert

        }
    }

    public enum OrientationPump { LeftToRight, RightToLeft }
}
