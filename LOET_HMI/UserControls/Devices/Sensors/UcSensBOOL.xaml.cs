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

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcSensBOOL.xaml
    /// </summary>
    public partial class UcSensBOOL : UserControl
    {

        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpSensStruct = DependencyProperty.Register(
            "DpSensStruct", typeof(StSensor<bool>), typeof(UcSensBOOL), new PropertyMetadata(new StSensor<bool>()));

        public StSensor<bool> DpSensStruct
        {
            get { return (StSensor<bool>)GetValue(_DpSensStruct); }
            set { SetValue(_DpSensStruct, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************





        public UcSensBOOL()
        {
            InitializeComponent();
        }
    }
}
