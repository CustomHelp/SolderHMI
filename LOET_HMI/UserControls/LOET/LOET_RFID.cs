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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls.Primitives;
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;
using System.Windows.Threading;
using System.Threading;



namespace LOET_HMI.UserControls
{
    public static class LOET_RFID_Functions
    {
        // ****************************************************************
        // ********************* Text- und Zahleingabe ********************
        // ****************************************************************
        public static void AnyTextBox_ShowKeyboard(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;

            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(tbSender), tbSender.Text, typeof(TextBox), 12);
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                tbSender.Text = virtualKeyboard.AnswerTextBox;

            virtualKeyboard.FirstClickIsDone = false;

            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTextBox_ShowKeyboard(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;

            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(tbSender), tbSender.Text, typeof(TextBox), 12);
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                tbSender.Text = virtualKeyboard.AnswerTextBox;

            virtualKeyboard.FirstClickIsDone = false;

            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTextBoxNumInput_BYTE(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            if (tbSender.Name == "tbWTNumber" || tbSender.Name == "tbDataversion")
            {
                Window_InputNum winNumInput = new Window_InputNum(0, 127, false, tbSender.Text);
                winNumInput.ShowDialog();

                if (winNumInput.DialogResult == true)
                    tbSender.Text = winNumInput.Answer;

                tbSender.Background = Brushes.White;
            }
            else
            {
                StackPanel panelPar = tbSender.Parent as StackPanel;
                Grid gridPar = panelPar.Parent as Grid;
                UcWTVariant varPar = gridPar.Parent as UcWTVariant;

                Window_InputNum winNumInput = new Window_InputNum(0, 127, false, tbSender.Text);
                winNumInput.ShowDialog();

                if (winNumInput.DialogResult == true)
                    tbSender.Text = winNumInput.Answer;

                tbSender.Background = Brushes.White;

                varPar.focusBtn.Focus();
            }
        }

        public static void AnyTextBoxNumInput_BYTE(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;


            Window_InputNum winNumInput = new Window_InputNum(0, 127, false, tbSender.Text);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
                tbSender.Text = winNumInput.Answer;

            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTextBoxNumInput_DINT(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;

            StParamPLCDB<int> paramTmp = new StParamPLCDB<int>();
            if (tbSender.Text == "")
                paramTmp.Val = 0;
            else
                paramTmp.Val = (int)Convert.ToInt32(tbSender.Text);

            paramTmp.Min = int.MinValue;
            paramTmp.Max = int.MaxValue;

            Window_InputNum winNumInput = new Window_InputNum(paramTmp);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
                tbSender.Text = winNumInput.Answer;


            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTexgtBoxNumInput_DINT(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;

            StParamPLCDB<int> paramTmp = new StParamPLCDB<int>();
            if (tbSender.Text == "")
                paramTmp.Val = 0;
            else
                paramTmp.Val = (int)Convert.ToInt32(tbSender.Text);

            paramTmp.Min = int.MinValue;
            paramTmp.Max = int.MaxValue;

            Window_InputNum winNumInput = new Window_InputNum(paramTmp);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
                tbSender.Text = winNumInput.Answer;


            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTextBoxNumInput_REAL(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;


            StParamPLCDB<float> paramTmp = new StParamPLCDB<float>();
            if (tbSender.Text == "")
                paramTmp.Val = 0;
            else
                paramTmp.Val = (float)Convert.ToDouble(tbSender.Text);

            paramTmp.Min = float.MinValue;
            paramTmp.Max = float.MaxValue;

            Window_InputNum winNumInput = new Window_InputNum(paramTmp);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
                tbSender.Text = winNumInput.Answer;


            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }

        public static void AnyTextBoxNumInput_REAL(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TextBox tbSender = sender as TextBox;
            tbSender.Background = Brushes.SkyBlue;

            StackPanel panelPar = tbSender.Parent as StackPanel;
            Grid gridPar = panelPar.Parent as Grid;
            UcWTVariant varPar = gridPar.Parent as UcWTVariant;


            StParamPLCDB<float> paramTmp = new StParamPLCDB<float>();
            if (tbSender.Text == "")
                paramTmp.Val = 0;
            else
                paramTmp.Val = (float)Convert.ToDouble(tbSender.Text);

            paramTmp.Min = float.MinValue;
            paramTmp.Max = float.MaxValue;

            Window_InputNum winNumInput = new Window_InputNum(paramTmp);
            winNumInput.ShowDialog();

            if (winNumInput.DialogResult == true)
                tbSender.Text = winNumInput.Answer;


            tbSender.Background = Brushes.White;
            varPar.focusBtn.Focus();
        }
        // ****************************************************************
        // ********************* Usercontrol Formattierungen **************
        // ****************************************************************
        public enum eTypeOfWriteVar { NoType, BYTE, DINT, REAL, STRING }


        public static void FormatTextBox_WRITE(List<TextBox> _listTextBox, eTypeOfWriteVar _Type)
        {
            for (int j = 0; j < _listTextBox.Count; j++)
            {
                _listTextBox[j].IsReadOnly = false;

                //if(_Type == eTypeOfWriteVar.BYTE)
                //    _listTextBox[j].PreviewMouseDown += LOET_RFID_Functions.AnyTextBoxNumInput_BYTE; 
                //if (_Type == eTypeOfWriteVar.DINT)
                //    _listTextBox[j].PreviewMouseDown += LOET_RFID_Functions.AnyTextBoxNumInput_DINT;
                //if (_Type == eTypeOfWriteVar.REAL)
                //    _listTextBox[j].PreviewMouseDown += LOET_RFID_Functions.AnyTextBoxNumInput_REAL;
                //if (_Type == eTypeOfWriteVar.STRING)
                //    _listTextBox[j].PreviewMouseDown += LOET_RFID_Functions.AnyTextBox_ShowKeyboard;

                if (_Type == eTypeOfWriteVar.BYTE)
                {
                    _listTextBox[j].MouseDoubleClick += LOET_RFID_Functions.AnyTextBoxNumInput_BYTE;
                }
                if (_Type == eTypeOfWriteVar.DINT)
                {   //_listTextBox[j].GotFocus += LOET_RFID_Functions.AnyTexgtBoxNumInput_DINT;
                    _listTextBox[j].MouseDoubleClick += LOET_RFID_Functions.AnyTexgtBoxNumInput_DINT;
                }
                if (_Type == eTypeOfWriteVar.REAL)
                {
                    _listTextBox[j].MouseDoubleClick += LOET_RFID_Functions.AnyTextBoxNumInput_REAL;
                }
                if (_Type == eTypeOfWriteVar.STRING)
                {
                    _listTextBox[j].MouseDoubleClick += LOET_RFID_Functions.AnyTextBox_ShowKeyboard;
                }
            }
        }

        private static void LOET_RFID_Functions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void FormatTextBox_READ(List<TextBox> _listTextBox)
        {
            for (int j = 0; j < _listTextBox.Count; j++)
            {
                _listTextBox[j].IsReadOnly = true;
                _listTextBox[j].Background = Brushes.Gray;
                _listTextBox[j].IsEnabled = false;
            }
        }


        public static void FormatToggleBtnAndLED_WRITE(List<ToggleButton> _listToggleBtn, List<UcLamp> _listLED)
        {
            for (int i = 0; i < _listToggleBtn.Count; i++)
            {
                _listLED[i].Visibility = Visibility.Collapsed;
                _listToggleBtn[i].Visibility = Visibility.Visible;
            }
        }

        public static void FormatToggleBtnAndLED_READ(List<ToggleButton> _listToggleBtn, List<UcLamp> _listLED)
        {
            for (int i = 0; i < _listToggleBtn.Count; i++)
            {
                _listLED[i].Visibility = Visibility.Visible;
                _listToggleBtn[i].Visibility = Visibility.Collapsed;
            }
        }



    }
}
