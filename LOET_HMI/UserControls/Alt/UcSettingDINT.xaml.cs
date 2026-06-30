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


namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcSettingAnalogDINT.xaml
    /// </summary>
    public partial class UcSettingAnalogDINT : UserControl
    {

        public static readonly DependencyProperty _DpSettingDINT_HMI = DependencyProperty.Register(
        "DpSettingDINT_HMI", typeof(StParamPLCDB<Int32>), typeof(UcSettingAnalogDINT), new PropertyMetadata(new StParamPLCDB<Int32>()));

        public StParamPLCDB<Int32> DpSettingDINT_HMI
        {
            get { return (StParamPLCDB<Int32>)GetValue(_DpSettingDINT_HMI); }
            set { SetValue(_DpSettingDINT_HMI, value); }
        }



        public UcSettingAnalogDINT()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void UcSettingAnalogDINT_UIdef_GotFocus(object sender, RoutedEventArgs e)
        {
            Window_InputNum window_setting_DINT = new Window_InputNum(DpSettingDINT_HMI);
            window_setting_DINT.ShowDialog(); // Das Setting-Window wird geöffnet.

            if(window_setting_DINT.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert angegeben wird)
            {
                if (DpSettingDINT_HMI.Val.GetType() == typeof(Int32))
                    DpSettingDINT_HMI.WriteValToPLCAndDB( Convert.ToInt32(window_setting_DINT.Answer) );
            }
        }



    }
}
