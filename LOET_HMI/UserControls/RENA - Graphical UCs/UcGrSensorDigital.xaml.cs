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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcGrSettingDigital.xaml
    /// </summary>
    public partial class UcGrSensorDigital : UserControl, INotifyPropertyChanged
    {


        public static readonly DependencyProperty _DpSensorBOOL_HMI = DependencyProperty.Register(
        "DpSensorBOOL_HMI", typeof(StSensor<bool>), typeof(UcGrSensorDigital), new PropertyMetadata(new StSensor<bool>()));

        public StSensor<bool> DpSensorBOOL_HMI
        {
            get { return (StSensor<bool>)GetValue(_DpSensorBOOL_HMI); }
            set { SetValue(_DpSensorBOOL_HMI, value); }
        }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private bool _IsAlarm = false;
        public bool IsAlarm
        {
            get
            {
                return _IsAlarm;
            }
            set
            {
                _IsAlarm = value;
                OnPropertyChanged();
            }
        }



        public UcGrSensorDigital()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UcSensorDigital_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
            WindowPopUpSetOutput window_setting = new WindowPopUpSetOutput(DpSettingBOOL_HMI);
            window_setting.ShowDialog();


            if (window_setting.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
            {
                DpSettingBOOL_HMI.WriteVal(Convert.ToBoolean(window_setting.AnswerInSetOutputPopUp)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben
            }
            */

        }


    }
}
