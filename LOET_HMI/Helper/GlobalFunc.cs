using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using LOET_HMI.UserControls;
using LOET_HMI.Displays;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace LOET_HMI
{
    static public class GlobalFunc
    {

        public static void RefreshNavVert(Button btnClicked)
        {
            string sContentOfClickedBtn = Convert.ToString(btnClicked.Content);
            for (int i = 0; i < GlobalVar.dataLOET.listStatVertNav.Count; i++)
            {
                if (GlobalVar.dataLOET.listStatVertNav[i].btnVertNav.Content == sContentOfClickedBtn)
                {
                    //*****************************************************
                    //***************** Modul aktualisieren ***************
                    GlobalVar.dataLOET.Act_Station = GlobalVar.dataLOET.listStatVertNav[i];

                    //*****************************************************
                    //************** Navigation verbinden *****************
                    GlobalVar.navHoriz.Station_Name = sContentOfClickedBtn;
                    GlobalVar.navHoriz.LinkWithStDisplays(GlobalVar.dataLOET.Act_Station);
                }
            }
        }



        public static void  PopUp_SetMainWBackgrDark()
        {
            
            Window windowMain = (MainWindow)Application.Current.MainWindow;
            /*
            // settings for the parent window
            // set the transparency to the half
            windowMain.Opacity = 0.5;
            // blur the whole window
            //window.Effect = new System.Windows.Media.Effects.BlurEffect();
            */

            ((MainWindow)Application.Current.MainWindow).rectDarkBackground.Visibility = Visibility.Visible;
        }

        public static void PopUp_SetMainWBackgrNormal()
        {
            Window windowMain = (MainWindow)Application.Current.MainWindow;
            /*
            //restore Opacity and remove blur after closing the child window
            windowMain.Opacity = 1;
            //window.Effect = null;
            */
            ((MainWindow)Application.Current.MainWindow).rectDarkBackground.Visibility = Visibility.Collapsed;
        }

        public static void VirtualKeyboardInputToTextBox(Point pos, TextBox textbox)
        {
            textbox.Background = Brushes.SkyBlue;

            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(pos, textbox.Text, typeof(TextBox));
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                textbox.Text = virtualKeyboard.AnswerTextBox;

            virtualKeyboard.FirstClickIsDone = false;           

            textbox.Background = Brushes.White;

            //System.Threading.Thread.Sleep(500);   
            
            textbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // Damit bei wiederholtem Click in den Textbox das Event ausgelöst wird.         
           
            //((MainWindow)Application.Current.MainWindow).rena_HMI.Act_Modul.ClusterList1stRow[0].ButtonOfDispl.Focus();
        }

        public static void VirtualKeyboardInputToPasswordBox(Point pos, PasswordBox passwordBox)
        {
            VirtualKeyboard virtualKeyboard = new VirtualKeyboard(pos, passwordBox.Password, typeof(PasswordBox));
            virtualKeyboard.ShowDialog();

            if (virtualKeyboard.DialogResult == true)
                passwordBox.Password = virtualKeyboard.AnswerPasswordBox;

            System.Threading.Thread.Sleep(100);
            passwordBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); // Damit bei wiederholtem Click in den PasswordBox das Event ausgelöst wird.
            //((MainWindow)Application.Current.MainWindow).rena_HMI.Act_Modul.ClusterList1stRow[0].ButtonOfDispl.Focus();
        }


        public static void ActivateNoUserRect()
        {
            ((MainWindow)Application.Current.MainWindow).rectNoUser.Visibility = Visibility.Visible;
        }

        public static void DeactivateNoUserRect()
        {
            ((MainWindow)Application.Current.MainWindow).rectNoUser.Visibility = Visibility.Collapsed;
            
        }

        public static void ShowNoUserMessageBox()
        {
            
            GlobalFunc.PopUp_SetMainWBackgrDark();
            MessageBox.Show(
                Properties.Resources.MsgBox_LoginFirst_Text,//"Please login first", 
                Properties.Resources.MsgBox_LoginFirst_Caption,//"Login", 
                MessageBoxButton.OK, 
                MessageBoxImage.Asterisk);
            GlobalFunc.PopUp_SetMainWBackgrNormal();
            
        }

        public static void ShowNoAuthorization()
        {
            MessageBox.Show(Properties.Resources.MsgBoxUserLev_AccesDeniedCaption,
                            Properties.Resources.MsgBoxUserLev_AccesDeniedText,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Quelle: https://jeremybytes.blogspot.com/2013/07/changing-culture-in-wpf.html
        /// </summary>
        public static void RestartHMI(CultureInfo ci)
        {
            //**************************************************
            //********************** ADS ***********************
            ((MainWindow)Application.Current.MainWindow).DeregisterItems();
            DeregisterPgMessages();
            DeregisterPgOrder();

            //**************************************************
            //**************************************************
            ProcessStartInfo Info = new ProcessStartInfo();
            string culture = ci.Name.ToString();
        //Info.Arguments = "/C ping 127.0.0.1 -n 2 && " + @"C:\CHP\Projekte\20210527_LOET_Translate\HMI_21_05_26_1811_MBA\HMI\LOET_HMI\bin\Release\LOET_HMI " + culture; // Test-SPS -> anpassen, wenn der Pfad sich ändert!
            Info.Arguments = "/C ping 127.0.0.1 -n 5 && " + @"C:\CHP\Project\HMI\LOET_HMI\bin\Release\LOET_HMI " + culture; // SPS
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            App.Current.MainWindow.Close();
        }

        public static void DeregisterPgMessages()
        {
            //**********************************
            // Meldungsfenster:
            //var pgCurrentAlarms = ((MainWindow)Application.Current.MainWindow).dataLOET.Alarms.ClusterList1stRow[0].Display as Pg_CurrentAlarms;
            //var pgCurrentMessages = ((MainWindow)Application.Current.MainWindow).dataLOET.Alarms.ClusterList1stRow[2].Display as Pg_CurrentMessages;

            //pgCurrentAlarms.DeregisterItems();
            //pgCurrentMessages.DeregisterItems(); 
            //**********************************

            var pgTmpMessages = GlobalVar.dataLOET.Maschine.ClusterList1stRow[0].Display as PgMessages;
            try
            {
                int iTmpCount = VisualTreeHelper.GetChildrenCount(pgTmpMessages.spUcMsgs);

                for (int i = 0; i < iTmpCount; i++)
                {
                    var tmpUcMsg = VisualTreeHelper.GetChild(pgTmpMessages.spUcMsgs, i) as UcMessage;
                    tmpUcMsg.DeregisterOPAndMsg();
                }
            }
            catch
            {
                MessageBox.Show("Error at DeregisterPgMessages()");
            }
        }

        public static void DeregisterPgOrder()
        {
            try
            {
                var pgTmpOrder = GlobalVar.dataLOET.Auftrag.ClusterList1stRow[0].Display as PgOrder;

                if(pgTmpOrder != null)
                {
                    try
                    {
                        pgTmpOrder.DeregisterOrder();
                    }
                    catch
                    {
                        MessageBox.Show("Error at DeregisterPgOrder()");
                    }
                }
            }
            catch
            {
                ;
            }
        }


        public static void InitPgMessages()
        {
            var pgTmpMessages = GlobalVar.navHoriz.ClusterListRow1[0].Display as PgMessages;
            int iTmpCount = VisualTreeHelper.GetChildrenCount(pgTmpMessages.spUcMsgs); // Hinweis: wenn ein Exception auftritt, prüfen, ob der Parent-Control der Msg-Usercontrol hier immer noch passt (Stackpanel,Grid, usw)

            if(iTmpCount==0)
            {
                MessageBox.Show("Auf der Seite PgMessage wurden 0 UcMessage-Instanzen gezählt. Es muss mindestens 1 sein. Prüfen, ob der Parent-Control richtig ist!",
                                "InitPgMessages()",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            for (int i = 0; i < iTmpCount; i++)
            {
                try
                {
                    var tmpUcMsg = VisualTreeHelper.GetChild(pgTmpMessages.spUcMsgs, i) as UcMessage; // Hinweis: wenn ein Exception auftritt, prüfen, ob der Parent-Control der Msg-Usercontrol hier immer noch passt (Stackpanel,Grid, usw)
                    tmpUcMsg.RegisterOPAndMsg();
                    tmpUcMsg.bUcMessageInitialized = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,
                                    "InitPgMessages()",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }




        public static int FindIndOfGenericSettingItem()
        {
            int ind = -1;

            return ind;
        }

        public static Type FindTypeOfGenericSettingItem(object item) 
        {
            dynamic selected_sett_obj = null;

            List<IStParamPLCDB> ObjectListForSelectedItem = new List<IStParamPLCDB>();
            ObjectListForSelectedItem.Add(new StParamPLCDB<bool>());
            ObjectListForSelectedItem.Add(new StParamPLCDB<Int32>());
            ObjectListForSelectedItem.Add(new StParamPLCDB<float>());
            ObjectListForSelectedItem.Add(new StParamPLCDB<double>());
            ObjectListForSelectedItem.Add(new StParamPLCDB<string>());

            if (selected_sett_obj == null)
            {
                try
                {
                    selected_sett_obj = item as StParamPLCDB<bool>;  // Den Objekt vom Typ StSettingDINT_HMI in der ausgewählten DataGrid-Zeile in "sett_obj" ablegen
                }
                catch
                {; }
            }

            if (selected_sett_obj == null)
            {
                try
                {
                    selected_sett_obj = item as StParamPLCDB<Int32>;  // Den Objekt vom Typ StSettingDINT_HMI in der ausgewählten DataGrid-Zeile in "sett_obj" ablegen

                }
                catch
                {; }
            }

            if (selected_sett_obj == null)
            {
                try
                {
                    selected_sett_obj = item as StParamPLCDB<float>;  // Den Objekt vom Typ StSettingDINT_HMI in der ausgewählten DataGrid-Zeile in "sett_obj" ablegen
                }
                catch
                {; }
            }

            if (selected_sett_obj == null)
            {
                try
                {
                    selected_sett_obj = item as StParamPLCDB<double>;  // Den Objekt vom Typ StSettingDINT_HMI in der ausgewählten DataGrid-Zeile in "sett_obj" ablegen
                }
                catch
                {; }
            }

            if (selected_sett_obj == null)
            {
                try
                {
                    selected_sett_obj = item as StParamPLCDB<string>;  // Den Objekt vom Typ StSettingDINT_HMI in der ausgewählten DataGrid-Zeile in "sett_obj" ablegen
                }
                catch
                {; }
            }


            try
            {
                ObjectListForSelectedItem[0] = item as StParamPLCDB<bool>;
                ObjectListForSelectedItem[1] = item as StParamPLCDB<Int32>;// ????? Int32 oder int             
                ObjectListForSelectedItem[2] = item as StParamPLCDB<float>;
                ObjectListForSelectedItem[3] = item as StParamPLCDB<double>;
                ObjectListForSelectedItem[4] = item as StParamPLCDB<string>;
            }
            catch {; }

            int ind = -1;
            for (int k = 0; k < ObjectListForSelectedItem.Count; k++)
            {
                if (ObjectListForSelectedItem[k] != null)
                    ind = k; //Index, wo die Liste nicht null ist. Die Liste wird mit diesem Index weiterverwendet.
            }

            Type type = ObjectListForSelectedItem[ind].GetType();
            return type;
        }


        public static string GetComponentStateTxt(eComponentState eState)
        {
            string strState = "...";

            switch (eState)
            {
                case eComponentState.CS_00_Normal:
                    strState = Properties.Resources.ComponentState_OK;
                    break;
                case eComponentState.CS_10_Fault:
                    strState = Properties.Resources.ComponentState_Error;
                    break;
                case eComponentState.CS_20_Manual:
                    strState = Properties.Resources.ComponentState_ManualMode;
                    break;
                case eComponentState.CS_30_Wait:
                    strState = Properties.Resources.ComponentState_Wait;
                    break;
                case eComponentState.CS_40_Warn:
                    strState = Properties.Resources.ComponentState_Warning;
                    break;
                case eComponentState.CS_50_Mess:
                    strState = Properties.Resources.ComponentState_Message;
                    break;
            }

            return strState;
        }

        public static Brush GetComponentStateColor (eComponentState eState)
        {
            System.Windows.Media.Brush brushState = Brushes.Transparent; // Init.

            switch (eState)
            {
                case eComponentState.CS_00_Normal:
                    brushState = (Brush)Application.Current.FindResource("CHP_ColorBrush"); ;
                    break;
                case eComponentState.CS_10_Fault:
                    brushState = System.Windows.Media.Brushes.Red;
                    break;
                case eComponentState.CS_20_Manual:
                    brushState = System.Windows.Media.Brushes.Blue;
                    break;
                case eComponentState.CS_30_Wait:
                    brushState = System.Windows.Media.Brushes.Yellow;
                    break;
                case eComponentState.CS_40_Warn:
                    brushState = System.Windows.Media.Brushes.Yellow;
                    break;
                case eComponentState.CS_50_Mess:
                    brushState = System.Windows.Media.Brushes.Gray;
                    break;
            }
            return brushState;
        }

                /// <summary>
        /// Diese Methode liefert den Station-Namen aufgrund der Station-ID
        /// </summary>
        /// <param name="_iStationID"></param>
        /// <returns></returns>
        public static string GetStationName_DB(int _iModulNr, int _iStationID)
        {
            string strStationNameDB = "";

            try
            {
                if (_iModulNr == 1)
                    strStationNameDB = GlobalVar.dataLOET.listStatMsgModul1.Single(m => m.iGVL_ID == _iStationID).sStationNameDB;
                else if (_iModulNr == 2)
                    strStationNameDB = GlobalVar.dataLOET.listStatMsgModul2.Single(m => m.iGVL_ID == _iStationID).sStationNameDB;
                else if (_iModulNr == 3)
                    strStationNameDB = GlobalVar.dataLOET.listStatMsgModul3.Single(m => m.iGVL_ID == _iStationID).sStationNameDB;
            }
            catch
            {
                strStationNameDB = "error_" + nameof(GetStationName_DB);

                MessageBox.Show("The station name cannot be determined due to the station ID.", //"Der Station-Name kann aufgrund der Station-ID nicht bestimmt werden.", 
                                nameof(UcMessage) + " " + nameof(GetStationName_DB), 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }
            return strStationNameDB;
        }

        /// <summary>
        /// Diese Methode liefert den übersetzten Stationsnamen zum (nicht übersetzten) Datenbank-Stationsnamen
        /// </summary>
        /// <param name="_sStationNameDB"></param>
        /// <returns></returns>
        public static string GetStationName_Translate(int _iModulNr, string _sStationNameDB)
        {
            string strStationNameTransl = "";

            try
            {
                if (_iModulNr == 1)
                    strStationNameTransl = GlobalVar.dataLOET.listStatMsgModul1.Single(m => m.sStationNameDB == _sStationNameDB).sStationNameTransl;
                else if (_iModulNr == 2)
                    strStationNameTransl = GlobalVar.dataLOET.listStatMsgModul2.Single(m => m.sStationNameDB == _sStationNameDB).sStationNameTransl;
                else if (_iModulNr == 3)
                    strStationNameTransl = GlobalVar.dataLOET.listStatMsgModul3.Single(m => m.sStationNameDB == _sStationNameDB).sStationNameTransl;
            }
            catch
            {
                strStationNameTransl = "error_" + nameof(GetStationName_Translate);

                MessageBox.Show("The station name cannot be determined due to the station ID.", //"Der Station-Name kann aufgrund der Station-ID nicht bestimmt werden.", 
                                nameof(UcMessage) + " " + nameof(GetStationName_Translate),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }


            return strStationNameTransl;
        }

        public enum eConcatenateMessageMode
        {
            ForDB   = 10,
            ForHMI  = 20
        }

        public static string ConcatenateMessage(string _str1, string _str2, string _str3, eConcatenateMessageMode _mode)
        {
            string strResult = "";
            string strSeparator = "";

            if (_mode == eConcatenateMessageMode.ForDB)
                strSeparator = "§";
            else if (_mode == eConcatenateMessageMode.ForHMI)
                strSeparator = " ";

            if (_str1 != "")
                strResult = _str1;

            if (_str2 != "")
            {
                if (strResult != "")
                    strResult = strResult + strSeparator + _str2;
                else
                    strResult = _str2;
            }

            if (_str3 != "")
            {
                if (strResult != "")
                    strResult = strResult + strSeparator + _str3;
                else
                    strResult = _str3;
            }


            return strResult;
        }





        /// <summary>
        /// Within C# code to find out if a touch screen exists (doesn't check if its a single or multi-touch device though) by the using System.Windows.Input namespace in PresentationCore.
        /// Quelle: https://stackoverflow.com/questions/5673556/is-it-possible-to-let-my-c-sharp-wpf-program-know-if-the-user-has-a-touchscreen
        /// </summary>
        /// <returns></returns>
        public static bool HasTouchInput()
        {
            foreach (TabletDevice tabletDevice in Tablet.TabletDevices)
            {
                //Only detect if it is a touch Screen not how many touches (i.e. Single touch or Multi-touch)
                if (tabletDevice.Type == TabletDeviceType.Touch)
                    return true;
            }

            return false;
        }



        public static void ShowAnimationBusy(int _iLengthSec)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(_iLengthSec);
            timer.Tick += TimerAnimationBusy_Tick;
            timer.Start();

            ((MainWindow)System.Windows.Application.Current.MainWindow).ucAnimationBusy.Visibility = Visibility.Visible;
        }

        //private static Window windowAnimation { get; set; }

        private static void TimerAnimationBusy_Tick(object sender, EventArgs e)
        {
            var _timer = sender as DispatcherTimer;
            _timer.Stop();

            ((MainWindow)System.Windows.Application.Current.MainWindow).ucAnimationBusy.Visibility = Visibility.Collapsed;
        }











        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

    }
}
