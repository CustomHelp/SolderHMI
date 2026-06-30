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

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcDeviceOnOff.xaml
    /// </summary>
    public partial class UcDeviceOnOff : UserControl
    {

        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpDeviceStruct = DependencyProperty.Register(
            "DpDeviceStruct", typeof(StDeviceOnOff), typeof(UcDeviceOnOff), new PropertyMetadata(new StDeviceOnOff()));

        public StDeviceOnOff DpDeviceStruct
        {
            get { return (StDeviceOnOff)GetValue(_DpDeviceStruct); }
            set { SetValue(_DpDeviceStruct, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************




        public UcDeviceOnOff()
        {
            InitializeComponent();
        }
    }
}
