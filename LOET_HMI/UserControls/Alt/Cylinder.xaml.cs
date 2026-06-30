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


namespace LOET_HMI
{

    public partial class Cylinder : UserControl
    {

        public Cylinder()
        {
            InitializeComponent();
        }




        public static readonly DependencyProperty _stCylCom = DependencyProperty.Register(
        "DpCylinderHMI", typeof(StCylinder),   typeof(Cylinder),   new PropertyMetadata(new StCylinder()));

        public StCylinder DpCylinderHMI
        {
            get { return (StCylinder)GetValue(_stCylCom); }
            set { SetValue(_stCylCom, value); }
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TbTimeForerun_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

    /*    private void BtnChangeState_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VarCon.WriteItem(ADSName + ".bManOn", typeof(bool),true);

        }

        private void BtnChangeState_MouseUp(object sender, MouseButtonEventArgs e)
        {
            VarCon.WriteItem(ADSName + ".bManOn", typeof(bool), false);

        }*/
    }
}
