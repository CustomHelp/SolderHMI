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
using System.Windows.Threading;


namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für VirtualKeyboard.xaml
    /// </summary>
    public partial class VirtualKeyboard : Window
    {

        public class KeyboardButton : Button
        {
            private string _SmallLetter;
            public string SmallLetter {
                get
                {
                    return _SmallLetter;
                }
                set
                {
                    _SmallLetter = value;
                    //OnPropertyChanged();
                }
            }

            private string _BigLetter;
            public string BigLetter
            {
                get
                {
                    return _BigLetter;
                }
                set
                {
                    _BigLetter = value;
                    //OnPropertyChanged();
                }
            }

            public KeyboardButton(string small, string big) // Konstruktor mit Eingabeparameter
            {
                SmallLetter = small;
                BigLetter = big;
            }
            public KeyboardButton() // Konstruktor ohne Eingabeparameter
            {
            }



        }

        private bool _CapslockPressed = false;
        public bool CapslockPressed
        {
            get { return _CapslockPressed; }
            set { _CapslockPressed = value; }
        }

        private bool _SymbBtnPressed = false;
        public bool SymbBtnPressed
        {
            get { return _SymbBtnPressed; }
            set { _SymbBtnPressed = value; }
        }



        public string AnswerTextBox
        {
            set
            { txtAnswer.Text = value; }

            get
            { return txtAnswer.Text; }
        }

        public string AnswerPasswordBox
        {
            set
            { pswAnswer.Password = value; }

            get
            { return pswAnswer.Password; }
        }

        private int iCursorPos { get; set; }

        public List<KeyboardButton> ButtonList1;
        public List<KeyboardButton> ButtonList2;
        public List<KeyboardButton> ButtonList3;
        public List<KeyboardButton> ButtonList4Symb; // Nur für Symbole
        public List<KeyboardButton> ButtonList5Symb; // Nur für Symbole
        public List<KeyboardButton> ButtonList6Symb; // Nur für Symbole

        public List<KeyboardButton> ButtonListNum0;
        public List<KeyboardButton> ButtonListNum1;
        public List<KeyboardButton> ButtonListNum2;
        public List<KeyboardButton> ButtonListNum3;

        private Point _ClickPosition;
        public Point ClickPosition
        {
            get { return _ClickPosition; }
            set { _ClickPosition = value; }
        }

        public string initialText { get; set; }
        public Type typeOfTextField { get; set; }
        public int iMaxLength { get; set; }

        /// <summary>
        /// Beim ersten Klick am Touch-Display wird nur das TouchDown-Event ausgelöst. Das Click-Event wird nicht ausgelöst.
        /// Bei allen weiteren Clicks wird sowohl das TouchDown- als auch das Click-Event ausgelöst.
        /// 
        /// Bei beidem Event wird das Zeichen des gedrückten Tasters in den Textbox/Passwordbox kopiert.
        /// Da ab dem 2. Klick beide Events ausgelöst wird, wird das jeweilige Zeichen grundsätzlich 2-Mal in den Textbox/Passwordbox kopiert.
        /// Um es zu vermeidgen, wird diese boolsche Variable definiert und nach dem 1. Klick auf true gesetzt.
        /// </summary>
        public bool FirstClickIsDone { get; set; } 

        public bool BtnOKwasPressed { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="clicked_position"></param>
        /// <param name="currentText"></param>
        /// <param name="senderType"></param>
        public VirtualKeyboard(Point clicked_position, string currentText, Type senderType)
        {
            InitializeComponent();
            DataContext = this;

            ClickPosition   = clicked_position;
            initialText     = currentText;
            typeOfTextField = senderType;

            InitKeyboard();
        }

        public VirtualKeyboard(Point clicked_position, string currentText, Type senderType, int iMaxStringLength)
        {
            InitializeComponent();
            DataContext = this;

            ClickPosition = clicked_position;
            initialText = currentText;
            typeOfTextField = senderType;
            iMaxLength = iMaxStringLength;

            InitKeyboard();
        }



        public void InitKeyboard()
        {
            ButtonList1 = new List<KeyboardButton>();
            ButtonList2 = new List<KeyboardButton>();
            ButtonList3 = new List<KeyboardButton>();

            ButtonList4Symb = new List<KeyboardButton>();
            ButtonList5Symb = new List<KeyboardButton>();
            ButtonList6Symb = new List<KeyboardButton>();

            ButtonListNum0 = new List<KeyboardButton>();
            ButtonListNum1 = new List<KeyboardButton>();
            ButtonListNum2 = new List<KeyboardButton>();
            ButtonListNum3 = new List<KeyboardButton>();

            ButtonList1.Add(new KeyboardButton("q", "Q"));
            ButtonList1.Add(new KeyboardButton("w", "W"));
            ButtonList1.Add(new KeyboardButton("e", "E"));
            ButtonList1.Add(new KeyboardButton("r", "R"));
            ButtonList1.Add(new KeyboardButton("t", "T"));
            ButtonList1.Add(new KeyboardButton("z", "Z"));
            ButtonList1.Add(new KeyboardButton("u", "U"));
            ButtonList1.Add(new KeyboardButton("i", "I"));
            ButtonList1.Add(new KeyboardButton("o", "O"));
            ButtonList1.Add(new KeyboardButton("p", "P"));
            ButtonList1.Add(new KeyboardButton("ü", "Ü"));
            ButtonList1.Add(new KeyboardButton("q", "Q"));
            ButtonList1.Add(new KeyboardButton("+", "*"));


            ButtonList2.Add(new KeyboardButton("a", "A"));
            ButtonList2.Add(new KeyboardButton("s", "S"));
            ButtonList2.Add(new KeyboardButton("d", "D"));
            ButtonList2.Add(new KeyboardButton("f", "F"));
            ButtonList2.Add(new KeyboardButton("g", "G"));
            ButtonList2.Add(new KeyboardButton("h", "H"));
            ButtonList2.Add(new KeyboardButton("j", "J"));
            ButtonList2.Add(new KeyboardButton("k", "K"));
            ButtonList2.Add(new KeyboardButton("l", "L"));
            ButtonList2.Add(new KeyboardButton("ö", "Ö"));
            ButtonList2.Add(new KeyboardButton("ä", "Ä"));
            ButtonList2.Add(new KeyboardButton("#", "'"));


            ButtonList3.Add(new KeyboardButton("y", "Y"));
            ButtonList3.Add(new KeyboardButton("x", "X"));
            ButtonList3.Add(new KeyboardButton("c", "C"));
            ButtonList3.Add(new KeyboardButton("v", "V"));
            ButtonList3.Add(new KeyboardButton("b", "B"));
            ButtonList3.Add(new KeyboardButton("n", "N"));
            ButtonList3.Add(new KeyboardButton("m", "M"));
            ButtonList3.Add(new KeyboardButton(",", ";"));
            ButtonList3.Add(new KeyboardButton(".", ":"));
            ButtonList3.Add(new KeyboardButton("-", "_"));


            ButtonList4Symb.Add(new KeyboardButton("+", ""));
            ButtonList4Symb.Add(new KeyboardButton("-", ""));
            ButtonList4Symb.Add(new KeyboardButton("*", ""));
            ButtonList4Symb.Add(new KeyboardButton("=", ""));
            ButtonList4Symb.Add(new KeyboardButton(",", ""));
            ButtonList4Symb.Add(new KeyboardButton(".", ""));
            ButtonList4Symb.Add(new KeyboardButton(":", ""));
            ButtonList4Symb.Add(new KeyboardButton(";", ""));
            ButtonList4Symb.Add(new KeyboardButton("!", ""));
            ButtonList4Symb.Add(new KeyboardButton("?", ""));
            ButtonList4Symb.Add(new KeyboardButton("_", ""));

            ButtonList5Symb.Add(new KeyboardButton("^", ""));
            ButtonList5Symb.Add(new KeyboardButton("°", ""));
            ButtonList5Symb.Add(new KeyboardButton("%", ""));
            ButtonList5Symb.Add(new KeyboardButton("&", ""));
            ButtonList5Symb.Add(new KeyboardButton("%", ""));
            ButtonList5Symb.Add(new KeyboardButton("#", ""));
            ButtonList5Symb.Add(new KeyboardButton("~", ""));
            ButtonList5Symb.Add(new KeyboardButton("'", ""));
            ButtonList5Symb.Add(new KeyboardButton("\"", "")); // "-Symbol    IBN         
            ButtonList5Symb.Add(new KeyboardButton("|", ""));

            ButtonList6Symb.Add(new KeyboardButton("/", ""));
            ButtonList6Symb.Add(new KeyboardButton("(", ""));
            ButtonList6Symb.Add(new KeyboardButton(")", ""));
            ButtonList6Symb.Add(new KeyboardButton("{", ""));
            ButtonList6Symb.Add(new KeyboardButton("}", ""));
            ButtonList6Symb.Add(new KeyboardButton("[", ""));
            ButtonList6Symb.Add(new KeyboardButton("]", ""));
            ButtonList6Symb.Add(new KeyboardButton(@"\", ""));


            ButtonListNum0.Add(new KeyboardButton("0", "0"));
            ButtonListNum0.Add(new KeyboardButton(".", "."));

            ButtonListNum1.Add(new KeyboardButton("1", "1"));
            ButtonListNum1.Add(new KeyboardButton("2", "2"));
            ButtonListNum1.Add(new KeyboardButton("3", "3"));

            ButtonListNum2.Add(new KeyboardButton("4", "4"));
            ButtonListNum2.Add(new KeyboardButton("5", "5"));
            ButtonListNum2.Add(new KeyboardButton("6", "6"));

            ButtonListNum3.Add(new KeyboardButton("7", "7"));
            ButtonListNum3.Add(new KeyboardButton("8", "8"));
            ButtonListNum3.Add(new KeyboardButton("9", "9"));


            FormatButtonAndAddToStackPanel(ButtonList1, StackPanel1);
            FormatButtonAndAddToStackPanel(ButtonList2, StackPanel2);
            FormatButtonAndAddToStackPanel(ButtonList3, StackPanel3);

            FormatButtonAndAddToStackPanel(ButtonList4Symb, StackPanel4Symb);
            FormatButtonAndAddToStackPanel(ButtonList5Symb, StackPanel5Symb);
            FormatButtonAndAddToStackPanel(ButtonList6Symb, StackPanel6Symb);

            FormatButtonAndAddToStackPanel(ButtonListNum0, stackPanNumRow0);
            FormatButtonAndAddToStackPanel(ButtonListNum1, stackPanNumRow1);
            FormatButtonAndAddToStackPanel(ButtonListNum2, stackPanNumRow2);
            FormatButtonAndAddToStackPanel(ButtonListNum3, stackPanNumRow3);


            // Button-Width zurückgewinnen und Stackpanel-Margin setzen
            KeyboardButton button_i = new KeyboardButton();
            button_i = StackPanel1.Children[0] as KeyboardButton;
            StackPanel2.Margin = new Thickness(button_i.Width / 2, 0, 0, 0);
            StackPanel3.Margin = new Thickness(button_i.Width, 0, 0, 0);
            //SizeToContent = SizeToContent.WidthAndHeight;

            FirstClickIsDone = false;

            StackPanel4Symb.Visibility = Visibility.Collapsed;
            StackPanel5Symb.Visibility = Visibility.Collapsed;
            StackPanel6Symb.Visibility = Visibility.Collapsed;

            if (iMaxLength > 0)
            {
                spMaxChar.Visibility = Visibility.Visible;
                tbMaxChar.Text = iMaxLength.ToString();
            }
            else
                spMaxChar.Visibility = Visibility.Collapsed;
        }




        public void FormatButtonAndAddToStackPanel(List<KeyboardButton> keyboardlist, StackPanel stackpanel)
        {
            for (int i = 0; i < keyboardlist.Count; i++)
            {
                KeyboardButton i_button = new KeyboardButton();
                i_button = keyboardlist[i];

                if (CapslockPressed)
                    i_button.Content = keyboardlist[i].BigLetter;
                else
                    i_button.Content = keyboardlist[i].SmallLetter;

                //i_button.Width = 40;
                //i_button.Height = 40;
                i_button.Style = (Style)FindResource("VirtualKeyboardButtonStyle");
                i_button.Click              += AnyCharButton_Click;
                //i_button.TouchDown          += AnyCharButton_TouchDown;
                i_button.PreviewTouchDown += AnyCharButton_PreviewTouchDown;
                //i_button.PreviewMouseDown   += AnyCharButton_PreviewMouseDown;
                //i_button.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
                //i_button.TouchDown += AnyBtn_OnMouseDownOrTouchDown;


                stackpanel.Children.Add(i_button);
                stackpanel.ClearValue(StackPanel.HeightProperty);
                stackpanel.ClearValue(StackPanel.WidthProperty);

                // Im XAML-Code wird die Stackpanel-Größe vordefiniert, damit man ihre Anordnung sieht.
                //Nun wird die StackPanelgröße gelöschen, damit seine Größe durch die dynamisch veränderbaren Buttons bestimmt wird
            }
        }



        #region  Click/TouchDown Event-Handlers
        // ****************************************************************
        // ************************ Zeichen-Taster ************************

        private void AnyCharButton_PreviewTouchDown(object sender, TouchEventArgs e)
        {

            GlobalVar.bTouchEventFired = true;
            AddPressedCharToTextField(sender as KeyboardButton);

        }


        private void AnyCharButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                AddPressedCharToTextField(sender as KeyboardButton);

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }
        

        public void AddPressedCharToTextField(KeyboardButton btn_pressed)
        {
            string current_char = Convert.ToString(btn_pressed.Content);


            //bool focus = txtAnswer.Focus();
            if (typeOfTextField == typeof(TextBox))
            {
                txtAnswer.Text = txtAnswer.Text.Insert(iCursorPos,current_char);
                iCursorPos++;
            }
            else
            {
                if (typeOfTextField == typeof(PasswordBox))
                {
                    pswAnswer.Password = pswAnswer.Password + current_char;
                }
            }

            //btnOK.Focus();
        }


        // *************************************************************
        // ************************ Backspace-Button *******************
        private void BtnBack_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnBackPressed();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnBackPressed(); ;

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }

        public void BtnBackPressed()
        {
            if (typeOfTextField == typeof(TextBox))
            {
                if (iCursorPos > 0)
                {
                    txtAnswer.Text = txtAnswer.Text.Remove(iCursorPos - 1, 1);
                    iCursorPos--;
                }
            }
            else if (typeOfTextField == typeof(PasswordBox))
            {
                if (pswAnswer.Password.Length > 0)
                    pswAnswer.Password = pswAnswer.Password.Substring(0, pswAnswer.Password.Length - 1);
            }
        }


        // **************************************************************
        // ************************ Clear-Button ************************
        private void BtnClear_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnClearPressed();
        }


        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnClearPressed();

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }

        public void BtnClearPressed()
        {
            if (typeOfTextField == typeof(TextBox))
                txtAnswer.Text = "";

            else if (typeOfTextField == typeof(PasswordBox))
                pswAnswer.Password = "";

            iCursorPos = 0;
        }


        // *********************************************************
        // ************************ Space-Button *******************

        private void BtnSpace_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnSpacePressed();
        }

        private void BtnSpace_Click(object sender, RoutedEventArgs e)
        {
            
            if (!GlobalVar.bTouchEventFired)
                BtnSpacePressed();

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }


        public void BtnSpacePressed()
        {
            if (typeOfTextField == typeof(TextBox))
            {
                //iCursorPos = txtAnswer.CaretIndex;
                txtAnswer.Text = txtAnswer.Text.Insert(iCursorPos, " ");
                iCursorPos = iCursorPos++;
            }
            else
            {
                if (typeOfTextField == typeof(PasswordBox))
                {
                    pswAnswer.Password = pswAnswer.Password + " ";
                }
            }
        }



        // *********************************************************
        // ********************** Symbol-Button ********************
        private void BtnSymbols_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnSymbolsPressed();
        }

        private void BtnSymbols_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnSymbolsPressed(); 

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }


        public void BtnSymbolsPressed()
        {
            SymbBtnPressed = !SymbBtnPressed;
            if (SymbBtnPressed)
            {
                btnSymbols.Background = Brushes.LightBlue;

                StackPanel1.Visibility      = Visibility.Collapsed;
                StackPanel2.Visibility      = Visibility.Collapsed;
                StackPanel3.Visibility      = Visibility.Collapsed;
                StackPanel4Symb.Visibility  = Visibility.Visible;
                StackPanel5Symb.Visibility  = Visibility.Visible;
                StackPanel6Symb.Visibility  = Visibility.Visible;
            }                
            else
            {
                btnSymbols.ClearValue(Button.BackgroundProperty);

                StackPanel1.Visibility      = Visibility.Visible;
                StackPanel2.Visibility      = Visibility.Visible;
                StackPanel3.Visibility      = Visibility.Visible;
                StackPanel4Symb.Visibility  = Visibility.Collapsed;
                StackPanel5Symb.Visibility  = Visibility.Collapsed;
                StackPanel6Symb.Visibility  = Visibility.Collapsed;
            }    

        }



        // ****************************************************************
        // ************************ Caps-Lock *****************************
        private void BtnCapsLock_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            CapsLockPressed();
        }

        private void BtnCapsLock_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                CapsLockPressed(); ;

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }

        public void CapsLockPressed()
        {
            CapslockPressed = !CapslockPressed;
            if (CapslockPressed)
                btnCapsLock.Background = Brushes.LightBlue; // Hintergrund farbe ändern, wenn CapsLock aktiv ist.
            else
                btnCapsLock.ClearValue(Button.BackgroundProperty); // Gesetzte Hintergrundfarbe löschen, wenn CapsLock deaktiviert ist.

            SwitchLetter(StackPanel1);
            SwitchLetter(StackPanel2);
            SwitchLetter(StackPanel3);
        }

        public void SwitchLetter(StackPanel stackpanel)
        {
            foreach (KeyboardButton button_i in stackpanel.Children)
            {
                if (CapslockPressed)               
                    button_i.Content = button_i.BigLetter;                
                else                
                    button_i.Content = button_i.SmallLetter;               
            }
        }


        // ***********************************************************
        // ************************ OK-Button ************************
        private void BtnOK_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnOkPressed();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnOkPressed();
            //
            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen

            e.Handled = true;
            //BtnOkPressed();
        }

        public void BtnOkPressed()
        {
            int iLengthInput = 0; ;
            if (typeOfTextField == typeof(TextBox))
                iLengthInput = txtAnswer.Text.Length;
            else if (typeOfTextField == typeof(PasswordBox))
                iLengthInput = pswAnswer.Password.Length;


            if (iMaxLength > 0) // wenn dieser Parameter > 0 ist, muss die Stringlänge geprüft
            {
                if (iLengthInput <= iMaxLength)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Input is out of range!", "Range", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            else // Stringlänge muss nicht geprüft werden 
            {
                DialogResult = true;
                Close();
            }




            System.Threading.Thread.Sleep(200);

            BtnOKwasPressed = true;
            FirstClickIsDone = true;
        }

        // *******************************************************
        // ************************ ESC-Button *******************
        private void BtnESC_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            
            // if (!FirstClickIsDone)
            // {
            //     BtnESCPressed();
            //     //FirstClickIsDone = true;
            // }
            // BtnESCPressed();

            GlobalVar.bTouchEventFired = true;
            BtnESCPressed();
        }

        private void BtnESC_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnESCPressed();

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }

        public void BtnESCPressed()
        {
            DialogResult = false;
            Close();
            //FirstClickIsDone = false;
        }




        #endregion



        private void Window_Loaded(object sender, RoutedEventArgs e) // Fenster positionieren
        {
            // Positionieren
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            if (ClickPosition.Y < (desktopWorkingArea.Bottom - this.Height))
            {
                this.Left = desktopWorkingArea.Right / 2 - this.Width / 2;
                this.Top = desktopWorkingArea.Bottom - this.Height;
            }
            else
            {
                this.Left = desktopWorkingArea.Right / 2 - this.Width / 2;
                this.Top = desktopWorkingArea.Top;
            }


            // Textbox/Passwordbox zum schreiben vorbereiten
            if (typeOfTextField == typeof(TextBox))
            {
                txtAnswer.Visibility = Visibility.Visible;
                pswAnswer.Visibility = Visibility.Collapsed;

                txtAnswer.Text = initialText;
                txtAnswer.Focus();
                txtAnswer.CaretIndex = txtAnswer.Text.Length;
                iCursorPos = txtAnswer.CaretIndex;
            }
            else if (typeOfTextField == typeof(PasswordBox))
            {
                txtAnswer.Visibility = Visibility.Collapsed;
                pswAnswer.Visibility = Visibility.Visible;

                pswAnswer.Focus();
                txtAnswer.CaretIndex = pswAnswer.Password.Length;
            }

            FirstClickIsDone = false;
            BtnOKwasPressed = false;

            //GlobalVar.iCountBtnPressedByTouch = 0; // Zähler beim Loaded-Event zurücksetzen


        }

        private void KeyboardWindow_ContentRendered(object sender, EventArgs e) // Wird nach "WindowLoaded" aufgerufen!!
        {
            /*
            // Textbox/Passwordbox zum schreiben vorbereiten
            if (typeOfTextField == typeof(TextBox))
            {
                txtAnswer.Visibility = Visibility.Visible;
                pswAnswer.Visibility = Visibility.Collapsed;

                txtAnswer.Text = initialText;
                txtAnswer.Focus();
                txtAnswer.CaretIndex = txtAnswer.Text.Length;

            }
            else if (typeOfTextField == typeof(PasswordBox))
            {
                txtAnswer.Visibility = Visibility.Collapsed;
                pswAnswer.Visibility = Visibility.Visible;

                pswAnswer.Focus();
            }
            */
        }


        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {               
                //((MainWindow)Application.Current.MainWindow).dataLOET.Act_Station.ClusterList1stRow[0].ButtonOfDispl.Focus();
                GlobalVar.dataLOET.Act_Station.ClusterList1stRow[0].ButtonOfDispl.Focus();
            }
            catch
            { ; }
            
        }



        // *************************************************************************
        // *********************** Keypress programmatically ***********************
        // *************************************************************************
        /// <summary>
        /// Quelle:                     https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        /// List of Virtual-Key Codes:  https://docs.microsoft.com/de-de/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
        /// </summary>

        const int VK_UP = 0x26; //up key
        const int VK_DOWN = 0x28;  //down key
        const int VK_LEFT = 0x25;
        const int VK_RIGHT = 0x27;
        const int VK_TAB = 0x09;
        const int VK_RETURN = 0x0D; // Enter

        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        int press() 
        {
            //Press the key
            GlobalFunc.keybd_event((byte)VK_RIGHT, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            return 0;
        }

        private void TxtAnswer_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }





        private void TxtAnswer_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(txtAnswer.IsFocused)
                iCursorPos = txtAnswer.CaretIndex;
        }








        // *************************************************************************
        // *************************************************************************

    }
}
