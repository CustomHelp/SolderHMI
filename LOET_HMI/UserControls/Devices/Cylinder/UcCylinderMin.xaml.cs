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

namespace LOET_HMI.UserControls.HOPA_Devices
{
    /// <summary>
    /// Interaction logic for UcCylinderMin.xaml
    /// </summary>
    public partial class UcCylinderMin : UserControl
    {

        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _stCylCom = DependencyProperty.Register(
            "DpCylinderHMI", typeof(StCylinder), typeof(UcCylinderMin), new PropertyMetadata(new StCylinder()));

        public StCylinder DpCylinderHMI
        {
            get { return (StCylinder)GetValue(_stCylCom); }
            set { SetValue(_stCylCom, value); }
        }
        // *******************************************************************************************
        // *******************************************************************************************


        /*
        public static readonly DependencyProperty _eNr = DependencyProperty.Register("eNr", typeof(CylinderEnums), typeof(UcCylinder), new PropertyMetadata());
        public CylinderEnums eNr
        {
            get { return (CylinderEnums)GetValue(_eNr); }
            set { SetValue(_eNr, value); }
        }
        */

        public static readonly DependencyProperty _State = DependencyProperty.Register("State", typeof(eComponentState), typeof(UcCylinderMin), new PropertyMetadata());
        public eComponentState State
        {
            get { return (eComponentState)GetValue(_State); }
            set { SetValue(_State, value); }
        }



        private bool bBtnDown;

        public UcCylinderMin()
        {
            InitializeComponent();
        }

        private void UcCyl_MouseLeave(object sender, MouseEventArgs e)
        {
            //bBtnDown = false;
        }

        private void UcCyl_MouseEnter(object sender, MouseEventArgs e)
        {
            // bBtnDown = false;
        }

        private void BtnChangeState_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //bBtnDown = true;
        }
    }
}
