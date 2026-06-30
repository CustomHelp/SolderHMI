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
    /// Interaktionslogik für UcHydCylControl.xaml
    /// </summary>
    public partial class UcGrCylControl : UserControl, INotifyPropertyChanged
    {
        /*
        public static DependencyProperty _Header = DependencyProperty.Register("Header", typeof(string), typeof(UcHydCylControl), new PropertyMetadata());
        public string Header
        {
            get { return (string)GetValue(_Header); }
            set
            {
                SetValue(_Header, value);
                //tbHeader.Text = value;
            }
        }
        */

        public static readonly DependencyProperty _DpCyl = DependencyProperty.Register(
            "DpCyl", typeof(StCylinder), typeof(UcGrCylControl), new PropertyMetadata(new StCylinder()));

        public StCylinder DpCyl
        {
            get { return (StCylinder)GetValue(_DpCyl); }
            set { SetValue(_DpCyl, value); }
        }


        public static readonly DependencyProperty _IsPneumaticCyl = DependencyProperty.Register(
            "IsPneumaticCyl", typeof(bool), typeof(UcGrCylControl), new PropertyMetadata(new bool()));

        public bool IsPneumaticCyl
        {
            get { return (bool)GetValue(_IsPneumaticCyl); }
            set{ SetValue(_IsPneumaticCyl, value);  }
        }

        public static readonly DependencyProperty _RotateAngle = DependencyProperty.Register(
            "RotateAngle", typeof(double), typeof(UcGrCylControl), new PropertyMetadata(new double()));

        public double RotateAngle
        {
            get { return (double)GetValue(_RotateAngle); }
            set { SetValue(_RotateAngle, value); }
        }





        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public UcGrCylControl()
        {
            InitializeComponent();
        }

        private void RectValveLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Alt (Stand: die Mittelstellung beim Hydr.Zyl war noch verwendet)
            /*
            if (DpCyl.bIsHydraulicCylHMI)
                DpCyl.ValveBPressed();
            else
                DpCyl.WriteManualOn();
            */

            DpCyl.WriteManualOn();
        }

        private void RectValveRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Alt (Stand: die Mittelstellung beim Hydr.Zyl war noch verwendet)
            /*
            if (DpCyl.bIsHydraulicCylHMI)
                DpCyl.ValveAPressed();
            else
                DpCyl.WriteManualOn();
            */

            DpCyl.WriteManualOn();
        }







        private void UcCylContr_Loaded(object sender, RoutedEventArgs e)
        {
            //tbHeader.Text = Header;
            if (IsPneumaticCyl == true)
            {
                row0ContainerCylLift1.Height = new GridLength(0);
                row1ContainerCylLift1.Height = new GridLength(0);
                //row2ContainerCylLift1.Height = new GridLength(0);

                gridValveL.Margin = new Thickness(0,0,0,0);

                RotateTransform rotateValvesInUC = new RotateTransform(-90);
                gridValveL.LayoutTransform = rotateValvesInUC;
                gridValveR.LayoutTransform = rotateValvesInUC;

                RotateTransform rotateUC = new RotateTransform(RotateAngle);
                grMainUCGrid.LayoutTransform = rotateUC;

                rectBackground.Fill = Brushes.Transparent;
                rectBackground.StrokeThickness = 0;
                
            }
        }

        private void UcCylContr_Unloaded(object sender, RoutedEventArgs e)
        {
            UcValveFunctions.UcLoaded(gridValveL,  Row0,  Row1 );
            UcValveFunctions.UcLoaded(gridValveR, Row0R, Row1R);
        }

        public class RectConverter : IValueConverter
        {
            public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return new Rect(0, 0, System.Convert.ToDouble(value), System.Convert.ToDouble(value) / 2); //"x-coord, y-coord, width, height"
            }

            public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }
        }
    }
}
