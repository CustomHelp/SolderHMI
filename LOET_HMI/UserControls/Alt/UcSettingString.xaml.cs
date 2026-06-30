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
using LOET_HMI.SystemPages.PopUps;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcSettingAnalogString.xaml
    /// </summary>
    public partial class UcSettingAnalogString : UserControl
    {
        public UcSettingAnalogString()
        {
            InitializeComponent();
            DataContext = this;
            //Value.MouseDown += Value_GotFocus;
            //Value.MouseEnter += Value_GotFocus;
           // Value.MouseUp += Value_GotFocus;
            //Value.GotTouchCapture += Value_GotFocus;

        }

        private void Value_GotFocus(object sender, RoutedEventArgs e)
        {
            /*
            Point pos = PointToScreen(Mouse.GetPosition(this));

            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(pos);
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                tbInput.Text = virtualKeyboard.Answer;
            */


            GlobalFunc.VirtualKeyboardInputToTextBox( Mouse.GetPosition(this), tbInput);


        }

    }
}
