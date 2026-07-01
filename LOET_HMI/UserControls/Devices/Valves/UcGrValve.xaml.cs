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
using LOET_HMI.PLC_Com_Classes;

namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcGrValve.xaml
    /// </summary>
    public partial class UcGrValve : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty _DpValveHMI = DependencyProperty.Register(
            "DpValveHMI", typeof(StDeviceOnOff), typeof(UcGrValve), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpValveHMI
        {
            get { return (StDeviceOnOff)GetValue(_DpValveHMI); }
            set { SetValue(_DpValveHMI, value); }
        }

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private double _ValveRotateAngle;
        public double ValveRotateAngle
        {
            get
            {
                return _ValveRotateAngle;
            }
            set
            {
                _ValveRotateAngle = value;
                OnPropertyChanged();
            }
        }


        private double _OffsetToCenter;
        public double OffsetToCenter
        {
            get
            {
                return _OffsetToCenter;
            }
            set
            {
                _OffsetToCenter = value;
                OnPropertyChanged();
            }
        }


        public double length { get; set; }

        /*
        public Point LeftP1 { get; set; }
        public Point LeftP2 { get; set; }
        public Point LeftP3 { get; set; }

        public Point RightP1 { get; set; }
        public Point RightP2 { get; set; }
        public Point RightP3 { get; set; }
        */

        public UcGrValve()
        {
            

            InitializeComponent();
            DataContext = this;
            length = 20;
            UcValveFunctions.AdjustGeometry(PolygonLeft, PolygonRight, InnerGrid, Row0,CenterCircle, length);
            /*
            length = 20;

            PolygonLeft.Points.Clear();
            PolygonRight.Points.Clear();
            
            PolygonLeft.Points.Add(new Point(0, 0));
            PolygonLeft.Points.Add(new Point(0, length));
            PolygonLeft.Points.Add(new Point(length, length/2));

            PolygonRight.Points.Add(new Point(0, 0));
            PolygonRight.Points.Add(new Point(0, length));
            PolygonRight.Points.Add(new Point(length, length / 2));

            PolygonRight.RenderTransform = new RotateTransform(180, length/2, length/2);
            
            InnerGrid.Width = length;
            InnerGrid.Height = length;
            Row0.Height = new GridLength(length/2 + 2);
            CenterCircle.Height = length/2;
            CenterCircle.Width = length/2;
            */

        }

        private void UcValve_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            double valveCenter = Row0.ActualHeight + Row1.ActualHeight;
            OffsetToCenter = mainGrid.ActualHeight / 2 - valveCenter;

            mainGrid.Height = mainGrid.ActualHeight;
            mainGrid.Width = mainGrid.ActualWidth;

            mainGrid.Margin = new Thickness(0, OffsetToCenter, 0, 0);
            */

            UcValveFunctions.UcLoaded( mainGrid, Row0, Row1);



            RotateTransform rotateTransform1 = new RotateTransform(ValveRotateAngle);
            /// Hinweis:
            /// Im FillingOverview kam öfters zum Befehl "mainGrid.RenderTransformOrigin" folgendes Exception:
            ///     Ein Ausnahmefehler des Typs "System.ArgumentException" ist in WindowsBase.dll aufgetreten.
            ///     Zusätzliche Informationen: 
            ///     "0,5;NaN" ist kein gültiger Wert für die Eigenschaft "RenderTransformOrigin".
            /// Vermutlich ist das Valve-UC hier so klein, dass der Wert "valveCenter/mainGrid.ActualHeight"
            /// nicht mehr vernünftig berechnet werden kann. Deswegen geht der CodeBlock ins try-catch

            double valveCenter = Row0.ActualHeight + Row1.ActualHeight;

            try
            {
                mainGrid.RenderTransformOrigin = new Point(0.5, valveCenter / mainGrid.ActualHeight);
                mainGrid.LayoutTransform = rotateTransform1;
            }
            catch { /* UI-Layout/Rotation optional: bei Fehler unveraendert lassen */ }



            borderClickArea.Background  = Brushes.Transparent;

        }

        private void UcValve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void BorderClickArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DpValveHMI.WriteManualOn();
        }

        private void RectValveClick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DpValveHMI.WriteManualOn();
        }
    }


    public class RectConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Rect(0, 0, System.Convert.ToDouble(value), System.Convert.ToDouble(value) /2); //"x-coord, y-coord, width, height"
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /*
    public class PointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double xValue = (double)values[0];
            double yValue = (double)values[1];
            return new Point(xValue, yValue);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    */


}
