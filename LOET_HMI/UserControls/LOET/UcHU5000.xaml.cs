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
    /// Interaktionslogik für UcHU5000.xaml
    /// </summary>
    public partial class UcHU5000 : UserControl
    {
        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpHU5000HMI = DependencyProperty.Register(
            "DpHU5000HMI", typeof(StHU5000), typeof(UcHU5000), new PropertyMetadata(new StHU5000()));

        public StHU5000 DpHU5000HMI
        {
            get { return (StHU5000)GetValue(_DpHU5000HMI); }
            set { SetValue(_DpHU5000HMI, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************



        public UcHU5000()
        {
            InitializeComponent();
        }
    }
}
