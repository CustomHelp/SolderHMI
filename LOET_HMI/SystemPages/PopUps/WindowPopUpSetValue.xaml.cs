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
using LOET_HMI.PLC_Com_Classes;
using System.Globalization;
using System.Reflection;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpSetOutput.xaml
    /// </summary>
    public partial class WindowPopUpSetValue : Window
    {
        #region Lokale Properties
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

        private dynamic _ActVal;
        public dynamic ActVal     // Aktueller Wert
        {
            get { return _ActVal; }
            set { _ActVal = value; }
        }

        private string _SettingName;
        public string SettingName
        {
            get { return _SettingName; }
            set { _SettingName = value; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Unit;
        public string Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }

        // Nur für DropDown:
        private bool _bIsDropdown;
        public bool bIsDropdown
        {
            get { return _bIsDropdown; }
            set { _bIsDropdown = value; }
        }

        private int _iCountDropItems;
        public int iCountDropItems
        {
            get { return _iCountDropItems; }
            set { _iCountDropItems = value; }
        }

        private string[] _arr_sDropDownItems;
        public string[] arr_sDropDownItems
        {
            get { return _arr_sDropDownItems; }
            set { _arr_sDropDownItems = value; }
        }


        /// <summary>
        /// "Answer" ist ein Property der Klasse "Window_InputNum".
        /// Es setzt und liest das TextBox "txtAnswer".
        /// </summary>
        public string AnswInSetValPopUp
        {
            set
            {
                txtAnswer.Text = value;

                if (ActVal.GetType() == typeof(bool))
                    btnSet.IsEnabled = !btnSet.IsEnabled;
                else
                {
                    if(txtAnswer.Text != Convert.ToString(ActVal, CultureInfo.InvariantCulture.NumberFormat))
                        btnSet.IsEnabled = true;
                    else
                        btnSet.IsEnabled = false;
                }
            }
            get { return txtAnswer.Text; }

        }

        public bool FirstClickIsDone { get; set; } // generell der 1. Click wurde gemacht        
        #endregion

        public dynamic stSettingInSetOutputPopUp;


        private Window_InputNum winNumInput;
        private VirtualKeyboard virtualKeyboard;
        private bool bInputWinIsOpen = false;


        // Konstruktor 1
        public WindowPopUpSetValue(dynamic stParam)
        {
            // **********************************************
            // ********* Version 1: ohne Datenbank **********
            #region alte Version ohne Datenbank
            /*
            Min_lim = stParam.MinHMI;
            Max_lim = stParam.MaxHMI;
            ActVal = stParam.ValHMI;
            SettingName = stParam.sSettingNameHMI;
            Unit = stParam.sUnitHMI;
            stSettingInSetOutputPopUp = stParam;


            InitializeComponent();
            DataContext = this;

            AnswerInSetOutputPopUp = Convert.ToString(ActVal, CultureInfo.InvariantCulture.NumberFormat); //2. Argument: Dezimalzeichen wird als Punkt anstatt von Komma dargestellt (wie in der SPS)

            if (stParam.MinHMI.GetType() == typeof(bool))
            {
                txtAnswer.Visibility = Visibility.Collapsed;
                ToggleButton.Visibility = Visibility.Visible;
                ToggleButton.IsChecked = ActVal;
                tbMin.Text = "Range min: 0";
                tbMax.Text = "Range max: 1";


            }
            else
            {
                txtAnswer.Visibility = Visibility.Visible;
                ToggleButton.Visibility = Visibility.Collapsed;
            }
            //btnSet.Visibility = Visibility.Collapsed;
            btnSet.IsEnabled = false;
            */
            #endregion

            // **********************************************
            // ********* Version 2:  mit Datenbank **********   
            // Parameter-Properties in den lokalen WindowPopUpSetValue-Properties ablegen:
            Min_lim = stParam.Min;
            Max_lim = stParam.Max;
            ActVal = stParam.ValDB;
            SettingName = stParam.strName;
            Unit = stParam.strUnit;
            Description = stParam.strDescription;
            stSettingInSetOutputPopUp = stParam;


            InitializeComponent();
            DataContext = this;

            AnswInSetValPopUp = Convert.ToString(ActVal, CultureInfo.InvariantCulture.NumberFormat); //2. Argument: Dezimalzeichen wird als Punkt anstatt von Komma dargestellt (wie in der SPS)

            if (stParam.ValDB.GetType() == typeof(bool)) // Parameter ist Boolsche-Variable
            {
                // Sichtbarkeiten anpassen:
                txtAnswer.Visibility = Visibility.Collapsed;
                gridNumVal.Visibility = Visibility.Collapsed;
                cbDropDown.Visibility = Visibility.Collapsed;               
                ToggleButton.Visibility = Visibility.Visible;
                ToggleButton.IsChecked = ActVal;
                //tbMin.Text = "Untere Grenze: 0";
                //tbMax.Text = "Obere Grenze: 1";
                tbMin.Text = Properties.Resources.DlgSetVal_tbRangeMin + " 0";
                tbMax.Text = Properties.Resources.DlgSetVal_tbRangeMax + " 1";
            }
            else if(stParam.bIsDropdown) // DropDown 
            {
                // Parameter-Properties in den lokalen WindowPopUpSetValue-Properties ablegen:
                iCountDropItems = stParam.iCountDropItems;
                arr_sDropDownItems = new string[iCountDropItems];

                for (int i = 0; i < iCountDropItems; i++)
                    arr_sDropDownItems[i] = stParam.arr_sDropDownItems[i];

                // Sichtbarkeiten anpassen:
                txtAnswer.Visibility = Visibility.Collapsed;
                gridNumVal.Visibility = Visibility.Collapsed;
                cbDropDown.Visibility = Visibility.Visible;
                ToggleButton.Visibility = Visibility.Collapsed;

                cbDropDown.ItemsSource = arr_sDropDownItems;

                //string sTmpActVal = (string)Convert.ChangeType(ActVal, typeof(string), CultureInfo.InvariantCulture);
                //string sTmpItem = "";

                for (int i = 1; i <= arr_sDropDownItems.Count(); i++)
                {
                    
                    //sTmpItem = (string)Convert.ChangeType(cbDropDown.Items[i], typeof(string), CultureInfo.InvariantCulture);

                    try // Im Dropdwon Menü den Item auswählen, der dem aktuellen Parameter-Wert entspricht
                    {
                        //if (sTmpItem == sTmpActVal)
                        if (i == ActVal)
                            cbDropDown.SelectedIndex = i-1; // Indexierung geht bei ComboBox von 0, aber die  Indexierung der DropDownItems in der SPS von 1
                    }
                    catch
                    {
                        MessageBox.Show("Der aktuelle Parameterwert wurde unter den Dropdownmenü-Einträgen nicht gefunden.", // Text
                                        "", // Überschrift
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Asterisk);
                    }

                }

            }
            else // Alle andere Parametertypen
            {
                txtAnswer.Visibility = Visibility.Visible;
                gridNumVal.Visibility = Visibility.Visible;
                cbDropDown.Visibility = Visibility.Collapsed;
                ToggleButton.Visibility = Visibility.Collapsed;
            }
            btnSet.IsEnabled = false;           
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            int i = 0;
            //GlobalVar.bInputTextBlockIsClickedMoreThanOnce = false;
            //GlobalVar.iCountBtnPressedByTouch = 0; // Zähler beim Loaded-Event zurücksetzen
        }


        //***************************************************************************************
        //************************** Eingabefeld anklicken (Maus/Touch) *************************
        // Für Maus-Klick
        private void TxtAnswer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(txtAnswer);
            e.Handled = true; // MBA 4.9.2020
            ClickLogic();

            //FirstClickIsDone = true;
        }


        // 30.11.2020 JOK Klickbare Pufferzone um Textbox, um TouchScreen Bedingung zu erleichtern
        private void StackPanel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            var pos = e.GetPosition(groupPanel);
            // Zur Übersicht für Position und Größe der Textbox
            var buf = txtAnswer.TransformToAncestor(groupPanel).Transform(new Point(0, 0)); // 132, 62.5
            var height = txtAnswer.ActualHeight; //35
            var width = txtAnswer.ActualWidth; //180

            int bufZoneWidth = 50;

            // Positionsabfrage ob Mouse-Klick innerhalb Pufferzone (10px um das Textfeld)
            if (     (pos.X >= buf.X-bufZoneWidth && pos.X <= (buf.X + width + bufZoneWidth))   &&   (pos.Y >= buf.Y-bufZoneWidth &&  pos.Y <= (buf.Y + height + bufZoneWidth))) // 
            {
                if(!((pos.X >= buf.X              && pos.X <= (buf.X + width               ))   &&   (pos.Y >= buf.Y              &&  pos.Y <= (buf.Y + height)))) // ClickLogic() nur im Rahmen um dem Textfeld herum aufrufen. Im Textfeld nicht mehr
                {
                    ClickLogic();
                }
            }

        }






        // Für Touch:
        private void TxtAnswer_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true; // MBA 4.9.2020
            //ClickLogic();
            //
            //**********************************
        }

        public void ClickLogic()
        {
            //TxtAnswer_GotFocus(sender, e);
            if (stSettingInSetOutputPopUp.ValDB.GetType() == typeof(string))
            {

                if(!bInputWinIsOpen)
                {
                    DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                    new Action(() =>                    
                    {
                        bInputWinIsOpen = true; 

                        int iMaxStringLength;
                        if (stSettingInSetOutputPopUp.Max == null  || stSettingInSetOutputPopUp.Max=="")
                            iMaxStringLength = 0;
                        else
                            iMaxStringLength = (int)Convert.ChangeType(stSettingInSetOutputPopUp.Max, typeof(int));

                        
                        virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), txtAnswer.Text, typeof(TextBox), iMaxStringLength);
                        virtualKeyboard.Closed += VirtualKeyboard_Closed;
                        
                        virtualKeyboard.ShowDialog();

                        if (virtualKeyboard.DialogResult == true)
                            AnswInSetValPopUp = virtualKeyboard.AnswerTextBox;
                    }
                 ), DispatcherPriority.Send, null); // MBA 30.11.2020: bisher ContextIdle=3. Die aktuelle Priorität Send=3 ist die höchste 
                }
            }
            else
            {
                if (!bInputWinIsOpen)
                {
                    DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        winNumInput = new Window_InputNum(stSettingInSetOutputPopUp);
                        winNumInput.Owner = this; // Für die Positionierung nötig. Grund: "When a child window is created by a parent window by calling Show, the child window does not have a relationship with the parent window. This means that the child window does not have a reference to the parent window" https://docs.microsoft.com/en-us/dotnet/api/system.windows.window.owner?view=netframework-4.8
                        winNumInput.Closed += WinNumInput_Closed;
                        bInputWinIsOpen = true;
                        winNumInput.ShowDialog(); // Das Setting-Window wird nun geöffnet

                        if (winNumInput.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
                        {
                            AnswInSetValPopUp = winNumInput.Answer;
                        }
                    }
                 ), DispatcherPriority.Send, null); // MBA 30.11.2020: bisher ContextIdle=3. Die aktuelle Priorität Send=3 ist die höchste 
                }
            }

            // Focus auf den nächsten UI-Element setzen, damit bei wiederholtem Click in den TxtAnswer-Texblock das NumInputWindow erscheint
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;
            if (keyboardFocus != null)
                keyboardFocus.MoveFocus(tRequest);

            if (btnSet.IsEnabled)
                btnSet.Focus();
            else
                btnClose.Focus();

            //GlobalVar.bInputTextBlockIsClickedMoreThanOnce = true; // ab dem 2. Aufruf von ClickLogic() ist diese Variable schon true
        }

        private void VirtualKeyboard_Closed(object sender, EventArgs e)
        {
            bInputWinIsOpen = false;
            this.Activate();
        }

        private void WinNumInput_Closed(object sender, EventArgs e)
        {
            bInputWinIsOpen = false;
            this.Activate();
            btnClose.Focus();
        }

        //*****************************************************************************************




        //***************************************************************************************
        //************************** "Close" drücken (Maus/Touch) *******************************
        private void BtnClose_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true;
            //
            //GlobalVar.bTouchEventFired = true;
            //BtnClosePressed();
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            //
            //if (!GlobalVar.bTouchEventFired)
            //    BtnClosePressed();
            //
            //GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen

            e.Handled = true;
            BtnClosePressed();
        }

        public void BtnClosePressed()
        {
            Close();
            //GlobalVar.dataLOET.Act_Station.ClusterList1stRow[0].ButtonOfDispl.Focus(); // Focus auf einen anderen Control setzen, damit der ParamGrid bei wiederholtem Anklicken der gleichen Zeile das "GotFocus" Event auslöst.

        }

        //***************************************************************************************
        //**************************** "Set" drücken (Maus/Touch) *******************************
        private void BtnSet_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true;
            //GlobalVar.bTouchEventFired = true;
            //BtnSetPressed();
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            //
            //if (!GlobalVar.bTouchEventFired)
            //    BtnSetPressed();
            //
            //GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen

            e.Handled = true;
            BtnSetPressed();
        }

        public void BtnSetPressed()
        {
            DialogResult = true; // Es ist der Rückgabewert der ShowDialog()-Methode. Das Dialog-Fenster wird automatisch geschlossen.

            if (ActVal.GetType() == typeof(bool))
                stSettingInSetOutputPopUp.WriteValToPLCAndDB(Convert.ToBoolean(AnswInSetValPopUp));
            else if (ActVal.GetType() == typeof(Int32))
                stSettingInSetOutputPopUp.WriteValToPLCAndDB(Convert.ToInt32(AnswInSetValPopUp));
            else if (ActVal.GetType() == typeof(float))
                stSettingInSetOutputPopUp.WriteValToPLCAndDB(float.Parse(AnswInSetValPopUp, CultureInfo.InvariantCulture.NumberFormat));
            else if (ActVal.GetType() == typeof(double))
                stSettingInSetOutputPopUp.WriteValToPLCAndDB(double.Parse(AnswInSetValPopUp, CultureInfo.InvariantCulture.NumberFormat));
            else if (ActVal.GetType() == typeof(string))
                stSettingInSetOutputPopUp.WriteValToPLCAndDB(AnswInSetValPopUp);

            //GlobalVar.dataLOET.Act_Station.ClusterList1stRow[0].ButtonOfDispl.Focus(); // Focus auf einen anderen Control setzen, damit der ParamGrid bei wiederholtem Anklicken der gleichen Zeile das "GotFocus" Event auslöst.

        }

        //***************************************************************************************
        //*********************************** Dropdown Menü *************************************
        private void CbDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropDownPressed();

        }

        private void CbDropDown_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            //e.Handled = true;
            //GlobalVar.bTouchEventFired = true;
            //DropDownPressed();
        }

        private void CbDropDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //
            //if (!GlobalVar.bTouchEventFired)
            //    DropDownPressed();
            //
            //GlobalVar.bTouchEventFired = false;// Variable immer zurücksetzen
        }


        public void DropDownPressed()
        {
            //if ((string)cbDropDown.SelectedItem == (string)stSettingInSetOutputPopUp.ValDB)
            //    btnSet.IsEnabled = false;

            if ((cbDropDown.SelectedIndex + 1) == stSettingInSetOutputPopUp.ValDB) // +1, weil die Indexierung der Combobox bei 0 startet, aber die Indexierung der Einträge in der SPS bei 1
                btnSet.IsEnabled = false;
            else
            {
                //AnswInSetValPopUp = cbDropDown.SelectedItem.ToString();
                int iIndexOfSelectedSPSItem = cbDropDown.SelectedIndex + 1;
                AnswInSetValPopUp = iIndexOfSelectedSPSItem.ToString();
                btnSet.IsEnabled = true;
            }
        }

        //***************************************************************************************
        //*********************************** Toggle Button *************************************

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            AnswInSetValPopUp = "true";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            AnswInSetValPopUp = "false";
        }


    }
}
