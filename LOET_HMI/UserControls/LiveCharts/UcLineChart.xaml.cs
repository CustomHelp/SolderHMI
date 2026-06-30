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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using System.Threading;
using System.ComponentModel;
using System.Globalization;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcLineChart.xaml
    /// 
    /// Quelle: https://lvcharts.net/App/examples/v1/wpf/Basic%20Line%20Chart
    /// </summary>
    public partial class UcLineChart : UserControl, INotifyPropertyChanged
    {

        // ***********************************************************
        // ************************** Chart **************************
        // ***********************************************************
        public SeriesCollection seriesCollection1 { get; set; }
        public LineSeries seriesDispl { get; set; }
        public LineSeries seriesForce { get; set; }
        public LineSeries seriesData { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        // Func:     https://www.tutorialsteacher.com/csharp/csharp-func-delegate
        // Func is a generic delegate included in the System namespace. It has zero or more input parameters and one out parameter. The last parameter is considered as an out parameter.


        // Zoom:
        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                OnPropertyChanged();
            }
        }

        private ZoomingOptions _zoomingMode;

        // ***********************************************************
        // ************************** Data ***************************
        // ***********************************************************
        public int iCount = 3000;

        public ChartValues<double> displacement { get; set; }
        public ChartValues<double> force { get; set; }


        public string[] arrstrDispl { get; set; }

        public StArrPLC stPLCArrayINT;
        public StArrPLC stPLCArrayLREAL { get; set; }


        // ***********************************************************
        // **************** Interface INotifyPropertyChanged *********
        // ***********************************************************
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public UcLineChart()
        {
            InitializeComponent();


            displacement = new ChartValues<double>();
            force = new ChartValues<double>();

            arrstrDispl = new string[iCount];


            DataContext = this;
        }






        public class ZoomingModeCoverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                switch ((ZoomingOptions)value)
                {
                    case ZoomingOptions.None:
                        return "None";
                    case ZoomingOptions.X:
                        return "X";
                    case ZoomingOptions.Y:
                        return "Y";
                    case ZoomingOptions.Xy:
                        return "XY";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            axisX.MinValue = double.NaN;
            axisX.MaxValue = double.NaN;
            axisY.MinValue = double.NaN;
            axisY.MaxValue = double.NaN;
        }
    }
}
