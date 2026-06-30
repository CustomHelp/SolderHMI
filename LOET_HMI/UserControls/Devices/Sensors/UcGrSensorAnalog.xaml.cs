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
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;
using System.Globalization;


namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcGrSensorAnalogREAL.xaml
    /// </summary>
    public partial class UcGrSensorAnalog : UserControl
    {
        public static readonly DependencyProperty _DpSensorAnalog = DependencyProperty.Register(
            "DpSensorAnalog", typeof(StSensor<double>), typeof(UcGrSensorAnalog), new PropertyMetadata(new StSensor<double>()));

        public StSensor<double> DpSensorAnalog
        {
            get { return (StSensor<double>)GetValue(_DpSensorAnalog); }
            set { SetValue(_DpSensorAnalog, value); }
        }




        public UcGrSensorAnalog()
        {
            InitializeComponent();
        }



        private void UcSettinAnalog_MouseDown(object sender, MouseButtonEventArgs e)
        {

            /*
            WindowPopUpSetOutput window_setting = new WindowPopUpSetOutput(DpSettingREAL_HMI);
            window_setting.ShowDialog();

            if (window_setting.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
            {
                DpSettingREAL_HMI.WriteVal(float.Parse(window_setting.AnswerInSetOutputPopUp,  CultureInfo.InvariantCulture.NumberFormat)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben

                
            }
            */
        }


    }

    // Dieser Converter wird im XAML-Code verwendet
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, System.Globalization.CultureInfo culture)
        {
            return value.GetType().Name;
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, System.Globalization.CultureInfo culture)
        {
            // I don't think you'll need this
            throw new Exception("Can't convert back");
        }
    }
}
