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

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcCounter.xaml
    /// </summary>
    public partial class UcCounter : UserControl
    {

        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpCounterHMI = DependencyProperty.Register(
            "DpCounterHMI", typeof(StCounter), typeof(UcCounter), new PropertyMetadata(new StCounter()));

        public StCounter DpCounterHMI
        {
            get { return (StCounter)GetValue(_DpCounterHMI); }
            set { SetValue(_DpCounterHMI, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************





        public UcCounter()
        {
            InitializeComponent();
        }

        private void TbTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var tblTarget = sender as TextBlock;
            //DpCounterHMI.WriteTargetValue(Convert.ToUInt32(tblTarget.Text));

        }


        private void TbTarget_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();

            StParamPLCDB<uint> paramTmpTarget = new StParamPLCDB<uint>();
            try
            {
                paramTmpTarget.Val = Convert.ToUInt32(DpCounterHMI.uiTarget);
            }
            catch
            {
                paramTmpTarget.Val = 0;
            }
            paramTmpTarget.Min = uint.MinValue;
            paramTmpTarget.Max = uint.MaxValue;

            Window_InputNum InputNum = new Window_InputNum(paramTmpTarget);
            //InputNum.Owner = Application.Current.MainWindow;
            InputNum.ShowDialog(); // Das Setting-Window wird geöffnet.

            if (InputNum.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert angegeben wird)
            {
                DpCounterHMI.WriteTargetValue(Convert.ToUInt32(InputNum.Answer, CultureInfo.InvariantCulture.NumberFormat));
            }

            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
