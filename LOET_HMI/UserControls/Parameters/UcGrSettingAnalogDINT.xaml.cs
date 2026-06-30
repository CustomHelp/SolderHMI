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

namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcGrSetting.xaml
    /// </summary>
    public partial class UcGrSettingAnalogDINT : UserControl
    {

        
        public static readonly DependencyProperty _DpSettingDINT_HMI = DependencyProperty.Register(
                "DpSettingDINT_HMI", typeof(StParamPLCDB<Int32>), typeof(UcGrSettingAnalogDINT), new PropertyMetadata(new StParamPLCDB<Int32>()));

        public StParamPLCDB<Int32> DpSettingDINT_HMI
        {
            get { return (StParamPLCDB<Int32>)GetValue(_DpSettingDINT_HMI); }
            set { SetValue(_DpSettingDINT_HMI, value); }
        }
        


        public UcGrSettingAnalogDINT()
        {
            InitializeComponent();
        }

        
        private void UcSettingAnalog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowPopUpSetValue window_setting = new WindowPopUpSetValue(DpSettingDINT_HMI);
            window_setting.ShowDialog();


            if (window_setting.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
            {
                DpSettingDINT_HMI.WriteValToPLCAndDB(Convert.ToInt32(window_setting.AnswInSetValPopUp)); // WriteVal-Funktion des Objekts aufrufen (siehe entsprechende Struktur) und über ADS in die SPS schreiben
            }

        }

        
    }
}
