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
using System.Windows.Shapes;
using LOET_HMI.UserControls;
using LOET_HMI;
using LOET_HMI.PLC_Com_Classes;
using System.Globalization;
using System.Windows.Threading;

using System.Runtime.InteropServices;
using System.Diagnostics;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für Setting_DINT.xaml
    /// </summary>
    public partial class Window_InputNum : Window
    {
        // dynamic: Variable wird zur Laufzeit typisiert. Es erhält den Typ, der in der Setting-Struktur definiert wurde.

        private dynamic _Min_lim;
        public dynamic Min_lim
        {
            get { return _Min_lim; }
            set { _Min_lim = value; }
        }

        private dynamic _Max_lim;
        public dynamic Max_lim
        {
            get { return _Max_lim; }
            set { _Max_lim = value; }
        }

        private string _SettingName;
        public string SettingName
        {
            get { return _SettingName; }
            set { _SettingName = value; }
        }

        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }

        /// <summary>
        /// "Answer" ist ein Property der Klasse "Window_InputNum".
        /// Es setzt und liest das TextBox "txtAnswer".
        /// </summary>
        public string Answer
        {
            set
            { txtAnswer.Text = value; }

            get
            { return txtAnswer.Text; }
        }

        public Point StartupPos { get; set; }

        // Debug:
        public int iCountCharAdded { get; set; }
        public int iCountCharAddedByMouseEvent { get; set; }
        public int iCountCharAddedByTouchEvent { get; set; }
        public bool bCharAddedByMouseEventFirst { get; set; }
        public bool bCharAddedByTouchEventFirst { get; set; }

        public bool bFirstTouchEventCalledAfterWinLoaded { get; set; }
        public int iTouchDeviceInitCount { get; set; }
        public bool bDoInput { get; set; }

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


        public Window_InputNum(int min, int max, bool bIsFloatinPontNumber, string num)
        {
            Min_lim = min;
            Max_lim = max;
            //SettingName = stSetting.sSettingNameHMI;
            //Unit = stSetting.sUnitHMI;


            InitializeComponent();
            this.DataContext = this;

            if (bIsFloatinPontNumber)
            {
                btnSep.Visibility = Visibility.Visible; // Separator-Button sichtbar
            }
            else
            {
                btnSep.Visibility = Visibility.Hidden; // Separator-Button unsichtbar
            }
            this.Answer = num;
            InitButtons();
        }


        public Window_InputNum(dynamic stSetting)
        {
            // **********************************************
            // ********* Version 1: ohne Datenbank **********
            try
            {
                Min_lim = stSetting.MinHMI;
                Max_lim = stSetting.MaxHMI;
                SettingName = stSetting.sSettingNameHMI;
                Unit = stSetting.sUnitHMI;
            }
            catch { /* dynamic-Fallback V1/V2: der jeweils nicht passende Zweig wirft bewusst */ }

            // **********************************************
            // ********* Version 2:  mit Datenbank **********    
            try
            {
                Min_lim = stSetting.Min;
                Max_lim = stSetting.Max;
                //SettingName = stSetting.sSettingName;
                //Unit = stSetting.sUnit;
            }
            catch { /* dynamic-Fallback V1/V2: der jeweils nicht passende Zweig wirft bewusst */ }

            InitializeComponent();

            /// <summary>
            /// Verbindung der XAML und C# Properties.
            /// Nötig z.B. für den
            /// - txtAnswer.Text Befehl im C# Code
            /// - Text ="{Binding Min_lim}"  Befehl im XAML-Code.
            ///
            /// Desweiteren ist der Befehl Text="{Binding Path = Min_lim}" gleichbedeutend mit obigem Befehl.
            /// "Path" ist nämlich das Default-Property vom Binding. Daher kann "Path" einfach weggelassen werden. 
            /// 
            /// </summary>
            this.DataContext = this;

            // **********************************************
            // ********* Version 1: ohne Datenbank **********
            try
            {
                this.Answer = Convert.ToString(stSetting.ValHMI, CultureInfo.InvariantCulture.NumberFormat); //2. Argument: Dezimalzeichen wird als Punkt anstatt von Komma dargestellt (wie in der SPS)
                if (stSetting.ValHMI.GetType() == typeof(Int32))
                    btnSep.Visibility = Visibility.Hidden; // Separator-Button unsichtbar
                else
                    btnSep.Visibility = Visibility.Visible; // Separator-Button sichtbar
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // **********************************************
            // ********* Version 2:  Lötanlage spezial ******
            try
            {
                this.Answer = Convert.ToString(stSetting.Val, CultureInfo.InvariantCulture.NumberFormat); //2. Argument: Dezimalzeichen wird als Punkt anstatt von Komma dargestellt (wie in der SPS)            
                if (stSetting.Val.GetType() == typeof(Int32))
                    btnSep.Visibility = Visibility.Hidden; // Separator-Button unsichtbar
                else
                    btnSep.Visibility = Visibility.Visible; // Separator-Button sichtbar
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            InitButtons();

        }

        public void InitButtons()
        {
            //////////////////////
            //////////////////////

            //******* Maus
            btn0.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn1.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn2.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn3.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn4.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn5.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn6.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn7.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn8.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btn9.PreviewMouseDown += AnyBtn_PreviewMouseDown;
            btnSep.PreviewMouseDown += AnyBtn_PreviewMouseDown;


            btn0.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn1.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn2.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn3.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn4.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn5.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn6.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn7.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn8.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btn9.PreviewMouseUp += AnyBtn_PreviewMouseUp;
            btnSep.PreviewMouseUp += AnyBtn_PreviewMouseUp;

            btn0.Click += AnyBtn_Click;
            btn1.Click += AnyBtn_Click;
            btn2.Click += AnyBtn_Click;
            btn3.Click += AnyBtn_Click;
            btn4.Click += AnyBtn_Click;
            btn5.Click += AnyBtn_Click;
            btn6.Click += AnyBtn_Click;
            btn7.Click += AnyBtn_Click;
            btn8.Click += AnyBtn_Click;
            btn9.Click += AnyBtn_Click;
            btnSep.Click += AnyBtn_Click;

            btn0.MouseDown += AnyBtn_MouseDown;
            btn1.MouseDown += AnyBtn_MouseDown;
            btn2.MouseDown += AnyBtn_MouseDown;
            btn3.MouseDown += AnyBtn_MouseDown;
            btn4.MouseDown += AnyBtn_MouseDown;
            btn5.MouseDown += AnyBtn_MouseDown;
            btn6.MouseDown += AnyBtn_MouseDown;
            btn7.MouseDown += AnyBtn_MouseDown;
            btn8.MouseDown += AnyBtn_MouseDown;
            btn9.MouseDown += AnyBtn_MouseDown;
            btnSep.MouseDown += AnyBtn_MouseDown;



            // ******** Touch

            btn0.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn1.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn2.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn3.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn4.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn5.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn6.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn7.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn8.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btn9.PreviewTouchDown += AnyBtn_PreviewTouchDown;
            btnSep.PreviewTouchDown += AnyBtn_PreviewTouchDown;

            btn0.TouchDown += AnyBtn_TouchDown;
            btn1.TouchDown += AnyBtn_TouchDown;
            btn2.TouchDown += AnyBtn_TouchDown;
            btn3.TouchDown += AnyBtn_TouchDown;
            btn4.TouchDown += AnyBtn_TouchDown;
            btn5.TouchDown += AnyBtn_TouchDown;
            btn6.TouchDown += AnyBtn_TouchDown;
            btn7.TouchDown += AnyBtn_TouchDown;
            btn8.TouchDown += AnyBtn_TouchDown;
            btn9.TouchDown += AnyBtn_TouchDown;
            btnSep.TouchDown += AnyBtn_TouchDown;

            btn0.TouchEnter += AnyBtn_TouchEnter;
            btn1.TouchEnter += AnyBtn_TouchEnter;
            btn2.TouchEnter += AnyBtn_TouchEnter;
            btn3.TouchEnter += AnyBtn_TouchEnter;
            btn4.TouchEnter += AnyBtn_TouchEnter;
            btn5.TouchEnter += AnyBtn_TouchEnter;
            btn6.TouchEnter += AnyBtn_TouchEnter;
            btn7.TouchEnter += AnyBtn_TouchEnter;
            btn8.TouchEnter += AnyBtn_TouchEnter;
            btn9.TouchEnter += AnyBtn_TouchEnter;
            btnSep.TouchEnter += AnyBtn_TouchEnter;

            btn0.TouchUp += AnyBtn_TouchUp;
            btn1.TouchUp += AnyBtn_TouchUp;
            btn2.TouchUp += AnyBtn_TouchUp;
            btn3.TouchUp += AnyBtn_TouchUp;
            btn4.TouchUp += AnyBtn_TouchUp;
            btn5.TouchUp += AnyBtn_TouchUp;
            btn6.TouchUp += AnyBtn_TouchUp;
            btn7.TouchUp += AnyBtn_TouchUp;
            btn8.TouchUp += AnyBtn_TouchUp;
            btn9.TouchUp += AnyBtn_TouchUp;
            btnSep.TouchUp += AnyBtn_TouchUp;

            // ********** Focus
            /*
            btn0.GotFocus   += AnyBtn_GotFocus;
            btn1.GotFocus   += AnyBtn_GotFocus;
            btn2.GotFocus   += AnyBtn_GotFocus;
            btn3.GotFocus   += AnyBtn_GotFocus;
            btn4.GotFocus   += AnyBtn_GotFocus;
            btn5.GotFocus   += AnyBtn_GotFocus;
            btn6.GotFocus   += AnyBtn_GotFocus;
            btn7.GotFocus   += AnyBtn_GotFocus;
            btn8.GotFocus   += AnyBtn_GotFocus;
            btn9.GotFocus   += AnyBtn_GotFocus;
            btnSep.GotFocus += AnyBtn_GotFocus;
            */

            //************** Sonstiges
            // btn0.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn1.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn2.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn3.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn4.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn5.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn6.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn7.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn8.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn9.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // btnSep.MouseDown += AnyBtn_OnMouseDownOrTouchDown;
            // 
            // 
            // btn0.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn1.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn2.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn3.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn4.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn5.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn6.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn7.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn8.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btn9.TouchDown += AnyBtn_OnMouseDownOrTouchDown;
            // btnSep.TouchDown += AnyBtn_OnMouseDownOrTouchDown;


        }



        // ****************************************************************
        // ************************ Zeichen-Taster ************************

        // ***************************************
        // ********* Maus EventHandler ***********
        private void AnyBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e) // DiesesEvent gewählt, weil LeftButton=MouseButtonState.Pressed nur bei diesem erfüllt wird, wenn der Eingabe über Maustaster erfolgt.
        {
            ;
        }

        private void AnyBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ;
        }


        private void AnyBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ;
        }

        private void AnyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                AddPressedCharToTextField(sender as Button);

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }
        // ***************************************
        // ********* Touch  EventHandler *********
        private void AnyBtn_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            AddPressedCharToTextField(sender as Button);
        }

        private void AnyBtn_TouchDown(object sender, TouchEventArgs e)
        {
            ;
        }

        private void AnyBtn_TouchEnter(object sender, TouchEventArgs e)
        {
            ;
        }

        private void AnyBtn_TouchUp(object sender, TouchEventArgs e)
        {
            ;
        }

        // ***************************************
        // ********* Focus  EventHandler *********
        private void AnyBtn_GotFocus(object sender, RoutedEventArgs e)
        {
            ;
        }

        // ***************************************
        // ********* Sonstige  EventHandler ******
        private void AnyBtn_OnMouseDownOrTouchDown(object sender, EventArgs e)
        {
            //AddPressedCharToTextField(sender as Button); // MBA 4.9.2020: Ab dem 10. Aufruf des AnyBtn_PreviewTouchDown-EventHandlers wird die AddPressedCharToTextField()-Funktion nicht mehr hier, sondern im AnyBtn_PreviewMouseDown-EventHandler aufgerufen...
        }
        //********************************************************************
        public bool AddPressedCharToTextField(Button btn_pressed)
        {
            bool bResult = false;

            string current_char = Convert.ToString(btn_pressed.Content);
            txtAnswer.Text = txtAnswer.Text + current_char;

            txtAnswer.CaretIndex = txtAnswer.Text.Length; // Kursorposition nach dem letzen Zeichen setzen
            //btnOK.Focus();
            //txtAnswer.Focus();

            iCountCharAdded++;
            bResult = true;
            return bResult;
        }


        // ****************************************************************
        // **************************** OK-Taster *************************
        // Bei IsDefault="True" Property des OK-Buttons wird das Dialogfenster automatisch geschlossen.
        // Daher ist der this.Close(); Befehl hier nicht mehr nötig.

        private void BtnOK_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            CheckEntry();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                CheckEntry();

            GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }

        public void CheckEntry()
        {
            dynamic num_answ = 's';
            // Numerical answer. 
            // "A dynamic type changes its type at runtime based on the value of the expression to the right of the "=" operator."
            // Initialwert ist absichtlich ein String. Falls die folgende Bedingung nicht erfüllt wird, ist der angegebene Wert ungültig.
            // Das Property "Answer" wird  entweder in Int32 oder float konvertiert.

            if (Min_lim.GetType() == typeof(Int32))
            {
                try { num_answ = Convert.ToInt32(Answer); }     // Convert "Answer" to Int32
                catch { MessageBox.Show("Input is not valid!"); return; }
            }
            else if (Min_lim.GetType() == typeof(float))
            {
                try { num_answ = float.Parse(Answer, CultureInfo.InvariantCulture.NumberFormat); }   // Convert "Answer" to float   
                catch { MessageBox.Show("Input is not valid!"); return; }
            }
            else if (Min_lim.GetType() == typeof(double))
            {
                try { num_answ = double.Parse(Answer, CultureInfo.InvariantCulture.NumberFormat); }   // Convert "Answer" to float   
                catch { MessageBox.Show("Input is not valid!"); return; }
            }


            if ((Min_lim <= num_answ) && (num_answ <= Max_lim))
                this.DialogResult = true; // Es ist der Rückgabewert der ShowDialog()-Methode. 
                                          // Falls die Bedingung erfüllt ist, ist der Eingabe über das Dialog-Fenster erfolgreich. Deswegen DialogResult (flag) setzen.
                                          // Nun wird das Dialog-Fenster automatisch geschlossen.
                                          // Wenn DialogResult == true, wird der Wert "Answer" im Code des User-Controls über die "WriteVal"-Methode in die Struktur geschrieben.
            else
                MessageBox.Show("Input is out of range!", "Range", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            System.Threading.Thread.Sleep(200);
        }



        // ****************************************************************
        // *************************** ESC-Taster *************************
        // Bei IsCancel="True" Property des ESC-Buttons wird das Dialogfenster automatisch geschlossen, 
        // wenn das ESC-Button gedrückt wird.
        // Daher ist der this.Close() Befehl hier nicht mehr nötig.


        private void BtnESC_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            this.Close();
        }

        private void BtnESC_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                this.Close();

            GlobalVar.bTouchEventFired = false; // zurücksetzen
        }


        // ****************************************************************
        // ************************ Clear-Taster **************************
        private void BtnClear_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnClearPressed();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            BtnClearPressed();

            if (!GlobalVar.bTouchEventFired)
                BtnClearPressed();

            GlobalVar.bTouchEventFired = false; // zurücksetzen
        }

        public void BtnClearPressed()
        {
            txtAnswer.Text = "";
        }

        // ****************************************************************
        // ************************ Back-Taster ***************************
        private void BtnBack_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnBackPressed();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnBackPressed();

            GlobalVar.bTouchEventFired = false; // zurücksetzen
        }

        public void BtnBackPressed()
        {
            if (txtAnswer.Text != "")
            {
                txtAnswer.Text = txtAnswer.Text.Substring(0, txtAnswer.Text.Length - 1);
                txtAnswer.CaretIndex = txtAnswer.Text.Length; // Kursorposition nach dem letzen Zeichen setzen
            }
        }

        private void Window_InputNumeric_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                BtnBack_Click(sender, e);
            }
        }

        // ****************************************************************
        // ************************ +/- Taster ****************************
        private void BtnSign_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            GlobalVar.bTouchEventFired = true;
            BtnSignPressed();
        }



        private void BtnSign_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVar.bTouchEventFired)
                BtnSignPressed();

            GlobalVar.bTouchEventFired = false; // zurücksetzen
        }


        public void BtnSignPressed()
        {
            //string str = txtAnswer.Text.Substring(0, 1);

            if (txtAnswer.Text == "") // Textbox leer
                txtAnswer.Text = txtAnswer.Text.Insert(0, "-");

            else if (txtAnswer.Text.Substring(0, 1) != "-") // im Textbox steht ein nichtnegativer Wert
                txtAnswer.Text = txtAnswer.Text.Insert(0, "-");

            else if (txtAnswer.Text.Substring(0, 1) == "-") // im Textbox steht ein negativer Wert
                txtAnswer.Text = txtAnswer.Text.Substring(1, txtAnswer.Text.Length - 1);
        }

        // ****************************************************************
        // ****************************************************************


        private void Window_InputNumeric_Loaded(object sender, RoutedEventArgs e)
        {
            // MBA 3.9.2020: auskommentiert
            // txtAnswer.Focus();
            // txtAnswer.CaretIndex = txtAnswer.Text.Length;


            //var current_window = e as Window;
            //Window parent = current_window.Parent;

            //this.Left = this.Owner.Left + (this.Owner.Width - this.ActualWidth) / 2 +500;


            if (this.Owner != null)
            {
                this.Left = this.Owner.Left + this.Owner.Width + 20;
                this.Top = this.Owner.Top + 200;
                //WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else if (StartupPos.X != 0 && StartupPos.Y != 0)
            {
                //Left = StartupPos.X + 20;
                //Top = StartupPos.Y + 70;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
                WindowStartupLocation = WindowStartupLocation.CenterScreen;


            // MBA 3.9.2020
            // Give logical focus to txtFirst TextBox

            DependencyObject focusScope = FocusManager.GetFocusScope(txtAnswer);
            FocusManager.SetFocusedElement(focusScope, txtAnswer);

            txtAnswer.CaretIndex = txtAnswer.Text.Length; // Kursorposition nach dem letzen Zeichen setzen


            //
            //GlobalFunc.SetCursorPos(100, 100);

            iCountCharAdded = 0;
            bFirstTouchEventCalledAfterWinLoaded = false;

        }



        private void Window_InputNumeric_Unloaded(object sender, RoutedEventArgs e)
        {

        }




    }
}
