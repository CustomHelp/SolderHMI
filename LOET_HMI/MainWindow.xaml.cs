using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Configuration;
using System.Windows.Shapes;
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.Displays;
using System.Runtime.InteropServices;
using System.IO;
using LOET_HMI.Displays._999_Project._10_Station;
using LOET_HMI.Displays._999_Project._20_Global;
using System.Diagnostics;

namespace LOET_HMI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // FullScreen

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;


        int hwnd = FindWindow("Shell_TrayWnd", "");


        // Config
        //bool bUseTranslater = true;

        // Timer
        private System.Windows.Threading.DispatcherTimer timerPLCRequest = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer timerPLCWait = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer timerPLCCheckRunning = new System.Windows.Threading.DispatcherTimer();

        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private List<ADSItem> ItemList = new List<ADSItem>();

        // Variablen
        int iActOP;
        private bool bShowDialog = false;
        private WindowPopUpDialog PopUpDialog;

        public Rectangle rectNoUser = new Rectangle(); // wenn kein Benutzer angemeldet ist, wird dieses Rechteck aktiviert
        public Rectangle rectDarkBackground = new Rectangle();

        //********************************************************************************************
        // *****************************   CHP-Buttons   *********************************************
        public static readonly DependencyProperty _BtnListMain = DependencyProperty.Register(
            "BtnListMain", typeof(List<StButton>), typeof(MainWindow), new PropertyMetadata(new List<StButton>()));

        public List<StButton> BtnListMain
        {
            get { return (List<StButton>)GetValue(_BtnListMain); }
            set { SetValue(_BtnListMain, value); }
        }
        //********************************************************************************************
        //********************************************************************************************

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            // ***********************************************************************************************
            // ****************************** Init. Globale Variablen ****************************************
            // ***********************************************************************************************
            // Globale Variablen initialisieren
            GlobalVar.bHMIInitialized = false;

            for (int i = 1; i <= 1; i++)
                BtnListMain.Add(new StButton());

            GlobalVar.dataLOET = new ProjectData();
            GlobalVar.Userlevels = new UserLevelsProject(); // Balog 22.6.2020: VORSICHT, ES WIRD NICHT VON DER DATENBANK GELADEN
            GlobalVar.debugADS = new ADSDebug();
            GlobalVar.navHoriz = HorizNav;
            GlobalVar.navVert = stackpanelNavVert;



            // ***********************************************************************************************
            // ********************************* Datenbank Debug-Einstellungen  ******************************
            // ***********************************************************************************************
            GlobalVar.debugDB = new DBDebug();
            GlobalVar.debugDB.bWantUseParam = true;
            GlobalVar.debugDB.bWantUseUser = true;

            if (!GlobalVar.debugDB.bWantUseUser)
            {
                // Fake user erstellen.
                GlobalVar.ActUser = new User("chp (keine DB)", "", 99); // Sonst kommt die Meldung "Bitte zuerst anmelden"
                btnLoginInMain.Content = GlobalVar.ActUser.sUserName;

                MessageBox.Show("Die Datenbank ist für die Benutzerverwaltung deaktiviert.\n",//Meldung
                                "Datenbank deaktiviert", //Überschrift)
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

            }

            // ***********************************************************************************************
            // ************************************ Init. Timers  ********************************************
            // ***********************************************************************************************
            /// PLC Check Running:
            /// Nachdem die ADS-Verbindung aktiviert wurde, prüft es periodisch, ob sie immer noch aktiv ist.
            //timerPLCCheckRunning.Interval = new TimeSpan(0, 0, 2);
            timerPLCCheckRunning.Interval = new TimeSpan(0, 0, 10);
            timerPLCCheckRunning.Tick += timerPLCCheckRunning_Tick;

            /// PLC Request: 
            /// Wenn die ADS-Verbindung INAKTIV ist, versucht es periodisch die ADS-Verbindung immer wieder aufzubauen.
            /// Wenn es klappt, wird das Event deaktiviert.
            //timerPLCRequest.Interval = new TimeSpan(0, 0, 1);
            timerPLCRequest.Interval = new TimeSpan(0, 0, 10);
            timerPLCRequest.Tick += TimerPLCRequest_Tick; // ??? MBA: ohne Verbinding immer wieder prüfen, ob die Verbindung aufgebaut werden kann

            /// PLC Wait: 
            /// Wenn der obige TimerPLCRequest_Tick die ADS-Verbindung aktivieren konnte, 
            /// wird der vorliegende timerPLCWait Timer aktiviert und läuft 1x (!!!) ab. 
            /// Das EventHandler wird praktisch zeitverzögert nach der ADS-Aktivierung 1x aufgerufen. 
            /// Hier können Aktionen für die Neuinitialisierung ausgeführt werden.   
            //timerPLCWait.Interval = new TimeSpan(0, 0, 1);
            timerPLCWait.Interval = new TimeSpan(0, 0, 10);
            timerPLCWait.Tick += TimerPLCWait_Tick;



            // ***********************************************************************************************
            // ***************************************** ADS *************************************************
            // ***********************************************************************************************
            // Debug-Variablen            
            GlobalVar.debugADS.bWantPLCConnect = true; // false auch, wenn man die viele 0x710 Meldungen (Symol could not be found) beim Starten nicht mit OK bestätigen will

            // Einzelne Connections:
            GlobalVar.debugADS.bWantConnectMainInit = true;
            GlobalVar.debugADS.bWantConnectMsg = true; // kann immer true bleiben
            GlobalVar.debugADS.bWantConnectParam = true;
            GlobalVar.debugADS.bWantConnectLang = true;

            #region MessageBox für ADS
            if (!GlobalVar.debugADS.bWantPLCConnect)
            {
                MessageBox.Show("Die ADS-Verbindung ist im Programmcode deaktiviert.",//Meldung
                                "ADS-Verbindung deaktiviert", //Überschrift)
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else // die einzhelnen Verbindungen prüfen
            {
                string strCaption = "";
                string strText = "Für folgende Komponenten wird ADS nicht aktiviert:\n";

                strText = strText + "";

                if (!GlobalVar.debugADS.bWantConnectMainInit) strText = strText + "- Main Init." + "\n";
                if (!GlobalVar.debugADS.bWantConnectMsg) strText = strText + "- Meldungen" + "\n";
                if (!GlobalVar.debugADS.bWantConnectParam) strText = strText + "- Parameter" + "\n";
                if (!GlobalVar.debugADS.bWantConnectLang) strText = strText + "- Sprache" + "\n";


                if (!GlobalVar.debugADS.bWantConnectMainInit)
                {

                    MessageBox.Show(strText,//Meldung
                                    strCaption, //Überschrift)
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
            }
            #endregion


            //ADSMain.ADSComServer.SetAmsNetId("192.168.50.52.1.1", 851); // SPS: Werkstatt-Rechner, Isolierter Kern
            ADSMain.ADSComServer.SetAmsNetId("", 851); // SPS: Lokal

            bool bTmpPLCConnected = false;
            if (GlobalVar.debugADS.bWantPLCConnect) // MB 7.8.2020
                bTmpPLCConnected = ADSMain.ADSComServer.Connect();

            if (bTmpPLCConnected)
            {
                Init();
                tbNoCon.Visibility = Visibility.Collapsed;
                timerPLCCheckRunning.IsEnabled = true;
                timerPLCCheckRunning.Start();
            }
            else
            {
                tbNoCon.Visibility = Visibility.Visible;
                timerPLCRequest.IsEnabled = true;
                timerPLCRequest.Start();
            }

            // ***********************************************************************************************
            // ************************************** Sonstiges  *********************************************
            // ***********************************************************************************************
            try
            {
                InitUser();
                SetWindowSize();
                LoadDisplays();
                SetLanguage();
                SetCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Konstruktor: " + ex.Message);
            }


            // Uhrzeit in der Kopfzeile
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.tbActTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            }, this.Dispatcher);
        }

        private void Init()
        {
            try
            {
                tbNoCon.Visibility = Visibility.Collapsed;

                timerPLCCheckRunning.IsEnabled = true;
                timerPLCCheckRunning.Start();

                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectMainInit)
                {
                    // ItemList[0]: Sprache
                    ItemList.Add(VarCon.AddItem("GVL_Basic.giLanguage", typeof(Int16)));

                    // ItemList[1]: Betriebsart
                    //ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[" + iModul.ToString() + "].iOP", typeof(Int16)));
                    ItemList.Add(new ADSItem()); // MBA 24.8.2020: falls iOP hier verwendet werden soll, wieder einkommentieren

                    // ItemList[2]: Benutzername
                    ItemList.Add(VarCon.AddItem("GVL_Basic.gstrUserName", typeof(string)));

                    // ItemList[3]: UserLevel
                    ItemList.Add(VarCon.AddItem("GVL_Basic.giUserLevel", typeof(Int16)));

                    // ItemList[4]: Dialog
                    ItemList.Add(VarCon.AddItem("GVL_Popup.gstDialog", typeof(ST_PopUp)));
                    ItemList.Add(new ADSItem()); // MBA 24.8.2020: falls die SPS-Variable angelegt ist, anpassen

                    // ItemList[5].....[end]: SPS Array-Limits               //[end]=[29]
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxModule", typeof(Byte)));    // Typ ist Byte, da USINT ist 8 Bit bei Beckhoff
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxStations", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxMessages", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxMessagesHMI", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxControlSections", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxSafeGuards", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxAxis", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxRobot", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxConverter", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxCylinder", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxSensorBool", typeof(Byte)));

                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxDeviceOnOff", typeof(Byte)));

                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxEthercats", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxProfibus", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxProfinet", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Machine_BOOL", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Machine_DINT", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Machine_LREAL", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Machine_STRING", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Typ_BOOL", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Typ_DINT", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Typ_LREAL", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_MaxParameter_Typ_STRING", typeof(Byte)));
                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxNetworkAdapter", typeof(Byte)));

                    ItemList.Add(VarCon.AddItem("GVL_Limits.gc_maxCounter", typeof(Byte)));

                    ItemList.Add(VarCon.AddItem("GVL_Project.lrMeasurementLH", typeof(double)));
                    ItemList.Add(VarCon.AddItem("GVL_Project.lrMeasurementRH", typeof(double)));
                    ItemList.Add(VarCon.AddItem("GVL_Project.lrMeasurementInsertLH", typeof(double)));
                    ItemList.Add(VarCon.AddItem("GVL_Project.lrMeasurementInsertRH", typeof(double)));
                }

                VarCon.EnableCallbackEvent();
                VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ADS-Aufbau " + ex.Message);
            }
        }

        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            // MBA 1.12.2020: die nachfolgenden 2 for-Schleifen wurden in die BeginInvoke-Abschnitt verschoben. Prüfen, ob die Leistung dadurch besser wird 
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                for (int j = 0; j < e.Item.Count; j++)
                {
                    for (int i = 0; i < ItemList.Count; i++)
                    {
                        if (ItemList[i].iHandle == e.Item[j].iHandle)
                        {
                            ItemList[i].Value = e.Item[j].Value;
                        }
                    }
                }
                tbADSItemCount.Text = e.Item.Count.ToString(); // Registrierten Items als Info anzeigen


                if (ItemList.Count > 0)
                {

                    // *************************************************************************************
                    // ****************************** ItemList[0]: Sprache *********************************
                    // *************************************************************************************
                    #region
                    try
                    {
                        GlobalVar.Language = (Languages)(Int16)(ItemList[0].Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Language Setup with: " + ex.Message);
                    }
                    #endregion

                    // *************************************************************************************
                    // ****************************** ItemList[1]: Betriebsart *****************************
                    // *************************************************************************************
                    #region
                    try
                    {
                        iActOP = Convert.ToInt32(ItemList[1].Value);
                        var brush = new ImageBrush();

                        /*
                        switch (iActOP)
                        {
                            case 0: // System ist AUS und nicht Einschaltbereit
                                //sOPImage = "/CHP_HMI;component/Resources/Not-Aus.png";
                                labelState1.Content = Properties.Resources.StateOff;
                                labelState2.Content = Properties.Resources.StateModul_0;
                                labelState1.Background = Brushes.Red;
                                labelState2.Background = Brushes.Red;
                                break;

                            case 1: // System ist AUS  Einschaltbereit
                                //sOPImage = "/CHP_HMI;component/Resources/System OFF.png";
                                labelState1.Content = Properties.Resources.StateOff;
                                labelState2.Content = Properties.Resources.StateModul_1;
                                labelState1.Background = Brushes.Red;
                                labelState2.Background = Brushes.Red;
                                break;

                            case 2: // Hochlauf
                                //sOPImage = "/CHP_HMI;component/Resources/Start UP.png";
                                labelState1.Content = Properties.Resources.StateOff;
                                labelState2.Content = Properties.Resources.StateModul_2;
                                labelState1.Background = Brushes.Red;
                                labelState2.Background = Brushes.LightGray;
                                break;

                            case 3: // Handfunktion Aktiv
                                //sOPImage = "/CHP_HMI;component/Resources/Manual Mode - Select.png";
                                labelState1.Content = Properties.Resources.StateOn;
                                labelState2.Content = Properties.Resources.StateModul_3;
                                labelState1.Background = Brushes.Green;
                                labelState2.Background = Brushes.Yellow;
                                break;

                            case 4:// Tippen
                                //sOPImage = "/CHP_HMI;component/Resources/Step Mode.png";
                                labelState1.Content = Properties.Resources.StateOn;
                                labelState2.Content = Properties.Resources.StateModul_4;
                                labelState1.Background = Brushes.Green;
                                labelState2.Background = Brushes.SteelBlue;
                                break;

                            case 5: // Automatik AUS
                                //sOPImage = "/CHP_HMI;component/Resources/Automatic OFF.png";
                                labelState1.Content = Properties.Resources.StateOn;
                                labelState2.Content = Properties.Resources.StateModul_5;
                                labelState1.Background = Brushes.Green;
                                labelState2.Background = Brushes.LightGray;
                                break;

                            case 6: // Automatik EIN - Letzter Zylkus
                                //sOPImage = "/CHP_HMI;component/Resources/Automatic ON – Last Cycle.png";
                                labelState1.Content = Properties.Resources.StateOn;
                                labelState2.Content = Properties.Resources.StateModul_6;
                                labelState1.Background = Brushes.Green;
                                labelState2.Background = Brushes.Green;
                                break;

                            case 7: // Automatik EIN
                                //sOPImage = "/CHP_HMI;component/Resources/Automatic ON.png";
                                labelState1.Content = Properties.Resources.StateOn;
                                labelState2.Content = Properties.Resources.StateModul_7;
                                labelState1.Background = Brushes.Green;
                                labelState2.Background = Brushes.Green;
                                break;
                        }
                        //btnOP.Background = brush;
                        if (labelState1.Background == Brushes.Green || labelState1.Background == Brushes.SteelBlue)
                            labelState1.Foreground = Brushes.White;
                        else if (labelState1.Background == Brushes.Red || labelState1.Background == Brushes.Yellow || labelState1.Background == Brushes.LightGray)
                            labelState1.Foreground = Brushes.Black;

                        if (labelState2.Background == Brushes.Green || labelState2.Background == Brushes.SteelBlue)
                            labelState2.Foreground = Brushes.White;
                        else if (labelState2.Background == Brushes.Red || labelState2.Background == Brushes.Yellow || labelState2.Background == Brushes.LightGray)
                            labelState2.Foreground = Brushes.Black;
                        */

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ITEM chANGE " + ex.Message);
                    }
                    #endregion

                    // *************************************************************************************
                    // ****************************** ItemList[2]: Benutzername ****************************
                    // ****************************** ItemList[3]: Passwort     ****************************
                    // *************************************************************************************
                    #region
                    try
                    {
                        // ***** Start
                        if (GlobalVar.ActUser == null)
                        {
                            try
                            {
                                GlobalVar.ActUser = new User((string)ItemList[2].Value, "", (Int16)ItemList[3].Value);
                            }
                            catch
                            {
                                GlobalVar.ActUser = new User("Error", "", 0); // UserLevel auf 0 zurücksetzen
                            }
                        }

                        // ***** GlobalVar.ActUser.sUserName ist nicht "null"               
                        try
                        {
                            // Benutzer ist nicht angemeldet
                            if (GlobalVar.ActUser.sUserName == ""
                                || GlobalVar.ActUser.sUserName == "Error"   // siehe oben
                                || GlobalVar.ActUser.sUserName == "NoUser") // Kommt, wenn das SPS-Programm durch den grünen Play-Button in VisualStudio gestartet wird
                            {
                                //btnLoginInMain.Content = Properties.Resources.Main_BtnLogin; // "Anmelden"
                                //GlobalFunc.ActivateNoUserRect();

                                GlobalVar.ActUser = new User("Default User", "", GlobalVar.Userlevels.Default.iLevel);
                                btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
                                GlobalFunc.DeactivateNoUserRect();

                            }
                            // Benutzer ist angemeldet
                            else
                            {
                                btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
                                GlobalFunc.DeactivateNoUserRect();
                            }

                        }
                        // ***** GlobalVar.ActUser.sUserName ist "null"
                        catch
                        {
                            btnLoginInMain.Content = Properties.Resources.Main_BtnLogin; ; //Rena
                            if ((Int16)ItemList[3].Value > 0)
                                VarCon.WriteItem("GVL_Basic.giUserLevel", 0);  // UserLevel auf 0 zurücksetzen

                            GlobalFunc.ActivateNoUserRect();
                        }

                        //GlobalVar.ActUser = GlobalVar.defUser;
                        //btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
                        //GlobalFunc.DeactivateNoUserRect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("usER :" + ex.Message);
                    }
                    #endregion

                    // *************************************************************************************
                    // ****************************** ItemList[4]: Dialog  *********************************
                    // *************************************************************************************
                    #region
                    try
                    {
                        var temp = ItemList[4].Value as ST_PopUp;
                        if (temp.bShow)
                        {
                            if (!bShowDialog)
                            {
                                GlobalFunc.PopUp_SetMainWBackgrDark();

                                bShowDialog = true;
                                PopUpDialog = new WindowPopUpDialog(temp);
                                PopUpDialog.ShowDialog();

                                if (DialogResult == true)
                                    PopUpDialog.Close();

                                GlobalFunc.PopUp_SetMainWBackgrNormal();
                            }
                        }
                        else
                        {
                            bShowDialog = false;

                            if (PopUpDialog != null)
                            {
                                PopUpDialog.Close(); // Für den Fall, wenn das Popup schon aufgrund des SPS-Programms geschlossen werden soll
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("popuup: " + ex.Message);
                    }
                    #endregion


                    // *************************************************************************************
                    // ****************************** ItemList[5]...[30]: SPS Array-Limits  ***************
                    // *************************************************************************************
                    #region
                    try
                    {
                        if (!GlobalVar.GVL_Limits.bIsInitialized)
                        {
                            int iArrLim_StartInd = 6;
                            int iArrLim_EndInd = ItemList.Count - 1;
                            int iTmpVal = 0;
                            int iCountValReceived = 0;


                            for (int i = iArrLim_StartInd; i <= iArrLim_EndInd; i++)
                            {
                                iTmpVal = 0; // Init.

                                if (ItemList[i].Value != null)
                                {
                                    iTmpVal = (int)Convert.ChangeType(ItemList[i].Value, typeof(int));

                                    switch (i)
                                    {
                                        // Empfangenen Wert in die jeweilige globale Variable schreiben
                                        // Von der ADS-Connection entfernen

                                        case 06: GlobalVar.GVL_Limits.maxModule = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 07: GlobalVar.GVL_Limits.maxStations = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 08: GlobalVar.GVL_Limits.maxMessages = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 09: GlobalVar.GVL_Limits.maxMessagesHMI = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 10: GlobalVar.GVL_Limits.maxControlSections = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 11: GlobalVar.GVL_Limits.maxSafeGuards = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 12: GlobalVar.GVL_Limits.maxAxis = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 13: GlobalVar.GVL_Limits.maxRobot = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 14: GlobalVar.GVL_Limits.maxConverter = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 15: GlobalVar.GVL_Limits.maxCylinder = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 16: GlobalVar.GVL_Limits.maxSensorBool = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 17: GlobalVar.GVL_Limits.maxDeviceOnOff = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 18: GlobalVar.GVL_Limits.maxEthercats = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 19: GlobalVar.GVL_Limits.maxProfibus = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 20: GlobalVar.GVL_Limits.maxProfinet = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 21: GlobalVar.GVL_Limits.MaxParameter_Machine_BOOL = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 22: GlobalVar.GVL_Limits.MaxParameter_Machine_DINT = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 23: GlobalVar.GVL_Limits.MaxParameter_Machine_LREAL = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 24: GlobalVar.GVL_Limits.MaxParameter_Machine_STRING = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 25: GlobalVar.GVL_Limits.MaxParameter_Typ_BOOL = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 26: GlobalVar.GVL_Limits.MaxParameter_Typ_DINT = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 27: GlobalVar.GVL_Limits.MaxParameter_Typ_LREAL = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 28: GlobalVar.GVL_Limits.MaxParameter_Typ_STRING = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 29: GlobalVar.GVL_Limits.maxNetworkAdapter = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                        case 30: GlobalVar.GVL_Limits.gc_maxCounter = iTmpVal; iCountValReceived++; VarCon.RemoveItem(ItemList[i]); break;
                                    }
                                }
                            }

                            // Wenn alle ArrLim-Werte empfangen worden sind, sollten sie nicht mehr von der SPS abgeholt werden () -> diese Items von der ItemList entfernen
                            if (iCountValReceived == (iArrLim_EndInd - iArrLim_StartInd -3)) // Vier Variablen werden nach den Limits im ADS mitgeholt, deshalb -3 anstatt +1

                            //iTmpVal = (int)Convert.ChangeType(ItemList[27].Value, typeof(int));
                            //if (iTmpVal > 0)
                            {
                                ItemList.RemoveRange(iArrLim_StartInd, iCountValReceived);

                                GlobalVar.GVL_Limits.bIsInitialized = true;
                                GlobalFunc.InitPgMessages();
                                InitDB();

                                GlobalVar.bHMIInitialized = true;
                                tbInitNotFinished.Visibility = Visibility.Hidden;
                            }

                        }


                    }
                    catch
                    {
                        MessageBox.Show("Initialisierung HMI fehlgeschlagen",
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                    #endregion

                    // *************************************************************************************
                    // ****************************** ItemList[5]...[30]: SPS Array-Limits  ***************
                    // *************************************************************************************
                    #region
                    try
                    {

                        int iArrLim_StartInd = ItemList.Count - 4;
                        int iArrLim_EndInd = ItemList.Count - 1;
                        double measurementLH = double.Parse(ItemList[iArrLim_StartInd].Value.ToString());
                        double measurementRH = double.Parse(ItemList[iArrLim_StartInd + 1].Value.ToString());
                        measurementLH = Math.Round(measurementLH, 2);
                        measurementRH = Math.Round(measurementRH, 2);
                        MeasurementLH.UCValue = measurementLH.ToString() + "mm";
                        MeasurementRH.UCValue = measurementRH.ToString() + "mm";
                        double measurementInsertLH = double.Parse(ItemList[iArrLim_StartInd + 2].Value.ToString());
                        double measurementInsertRH = double.Parse(ItemList[iArrLim_StartInd + 3].Value.ToString());
                        measurementInsertLH = Math.Round(measurementInsertLH, 2);
                        measurementInsertRH = Math.Round(measurementInsertRH, 2);
                        MeasurementInsertLH.UCValue = measurementInsertLH.ToString() + "mm";
                        MeasurementInsertRH.UCValue = measurementInsertRH.ToString() + "mm";
                    }
                    catch
                    {
                        MessageBox.Show("Initialisierung HMI fehlgeschlagen",
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                    #endregion
                }
            }));
        }
        private void VarCon_ItemChangeEvent250ms(object sender, EventArgs e)
        { }


        // ***********************************************************************************************
        // ******************************** Timer Event-Handlers  ****************************************
        // ***********************************************************************************************
        #region 
        private void timerPLCCheckRunning_Tick(object sender, EventArgs e)
        {
            if (!ADSMain.ADSComServer.Connected) // z.B. das SPS-Programm gestoppt wird (das rote Rechteck wird in Visual Studio gedrückt)
            {
                tbNoCon.Visibility = Visibility.Visible; // MBA 23.6.2020: Text "Warte auf Verbindung zur PLC." anzeigen

                // MBA 21.8.2020:
                timerPLCCheckRunning.IsEnabled = false;
                timerPLCCheckRunning.Stop();

                // MBA 21.8.2020:
                timerPLCRequest.IsEnabled = true;
                timerPLCRequest.Start();
            }
        }

        private void TimerPLCRequest_Tick(object sender, EventArgs e)
        {
            bool bTmpConnected = false;
            if (GlobalVar.debugADS.bWantPLCConnect)
                bTmpConnected = ADSMain.ADSComServer.Connect();

            if (bTmpConnected)
            {
                MessageBox.Show("Die ADS-Verbindung ist wieder aktiv.\nDas HMI startet neu.",//Meldung
                "ADS-Verbindung aktiv", //Überschrift)
                MessageBoxButton.OK,
                MessageBoxImage.Asterisk);

                timerPLCRequest.IsEnabled = false;
                timerPLCRequest.Stop();

                timerPLCWait.IsEnabled = true;
                timerPLCWait.Start();
                // - Den TimerPLCWait_Tick Eventhandler zeitverzögert  NUR 1-MAL aufrufen
                // - Aktionen für Neuinitialisierung dort ausführen
            }
        }
        private void TimerPLCWait_Tick(object sender, EventArgs e)
        {
            tbNoCon.Visibility = Visibility.Hidden;

            timerPLCWait.IsEnabled = false; // nach dem 1. Aufruf des EventHandlers den Timer gleich deaktivieren.
            timerPLCWait.Stop();

            //Init();
            //GlobalFunc.InitPgMessages();
            //GlobalFunc.RestartHMI();

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location); // Ursprünglich in timerPLCCheckRunning_Tick von Riedl
            System.Windows.Application.Current.Shutdown(); // Ursprünglich in timerPLCCheckRunning_Tick von Riedl
        }

        #endregion

        // ***********************************************************************************************
        // ************************************* Init. Datenbank *****************************************
        // ***********************************************************************************************
        public void InitDB()
        {
            if (GlobalVar.debugADS.bWantConnectParam)
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    // Aktuelle Daten
                    db_actual DBAct = new db_actual();
                    try
                    {
                        // Diese Tabelle darf nur einen einzigen Eintrag mit dem ID=1 enthalten.                    
                        DBAct = context.db_actual.Single(d => (d.id == 1));
                    }
                    catch
                    {
                        // Falls es nicht existiert (DB wurde erst installiert), muss es erstellt werden                   
                        MessageBox.Show("Die Datenbank-Tabelle 'db_actual' ist leer und wird initialisiert.\n\n"
                                        + "Anschließend werden Fehlermeldungen auftreten, solange die Parametersätze nicht in die SPS geladen werden.\n"
                                         ,
                                        "'db_actual' leer.",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);

                        #region Alt
                        /*
                        try
                        {
                            db_actual _New = new db_actual();
                            //_New.sTypeParamSetName = "";
                            //_New.sMachineParamSetName = "";

                            // ********************************************************************
                            // ************************* MBA 31.8.2020 *****************************
                            // ********************************************************************

                            List<db_paramset> listParamsetsAll;
                            List<db_paramset> listParamsetsType = new List<db_paramset>();
                            List<db_paramset> listParamsetsMachine = new List<db_paramset>();
                            db_paramset dbParamsetTypeNewest;
                            db_paramset dbParamsetMachineNewest;

                            listParamsetsAll = context.db_paramset.ToList();

                            // Alle Typ- und Maschinenparametersätze separieren
                            for (int i = 0; i < listParamsetsAll.Count; i++)
                            {
                                if ((ParamSetTypes)listParamsetsAll[i].iType == ParamSetTypes.Type)
                                    listParamsetsType.Add(listParamsetsAll[i]);
                                else if ((ParamSetTypes)listParamsetsAll[i].iType == ParamSetTypes.Machine)
                                    listParamsetsMachine.Add(listParamsetsAll[i]);
                            }

                            // Typparameter mit dem höchsten ID auswählen
                            int tmpID = listParamsetsType.Max(x => x.id);
                            dbParamsetTypeNewest = listParamsetsType.Single(x => x.id == tmpID);

                            // MaschinenParameter mit dem höchsten ID auswählen
                            tmpID = 0;
                            tmpID = listParamsetsMachine.Max(x => x.id);
                            dbParamsetMachineNewest = listParamsetsMachine.Single(x => x.id == tmpID);

                            //
                            _New.iTypeparamSetID = dbParamsetTypeNewest.id;
                            _New.iMachineparamSetID = dbParamsetMachineNewest.id;

                            // ********************************************************************
                            // ********************************************************************


                            context.db_actual.Add(_New);
                            context.SaveChanges();


                            DBAct = context.db_actual.Single(d => (d.id == 1));
                        }
                        catch
                        {
                            ;
                        }
                        */
                        #endregion

                        try
                        {
                            db_actual _New = new db_actual();
                            _New.id = 1;
                            _New.iMachineparamSetID = 0;
                            _New.iTypeparamSetID = 0;
                            _New.iManualParamSetId = 0; // MBA: was ist das??

                            context.db_actual.Add(_New);
                            context.SaveChanges();

                        }
                        catch
                        {
                            MessageBox.Show("Die Datenbank-Tabelle 'db_actual' konnte nicht initialisiert werden.\n\n"
                                             ,
                                            "Initialisierung 'db_actual'.",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }

                    // *************************************************************************************
                    // ************************* Aktuelle Parametersätze laden *****************************
                    // *************************************************************************************
                    // MBA: 06.08.2020: müssen diese 2 Befehle aufgerufen werden?         KLAUS
                    try
                    {
                        DBParam.Handler.LoadParamSetToPLC(DBAct.iTypeparamSetID, ParamSetTypes.Type);
                        DBParam.Handler.LoadParamSetToPLC(DBAct.iMachineparamSetID, ParamSetTypes.Machine);
                    }
                    catch
                    {
                        MessageBox.Show("Die laut Datenbank aktuellen Parametersätze konnten nicht in die SPS geladen werden.\n" +
                                        "Entweder wurde der geladene Typ- oder Maschinenparametersatz von der Datenbank entfernt, " +
                                        "oder bisher wurde noch kein Parametersatz in die SPS geladen."
                                        ,
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }

                    // *************************************************************************************
                    // ********************** Aktuelle Warteschlange der Aufträge **************************
                    // *************************************************************************************
                    List<db_orderqueue> listDBOrder = new List<db_orderqueue>();
                    try
                    {
                        listDBOrder = context.db_orderqueue.ToList();
                        listDBOrder = context.db_orderqueue.OrderBy(o => o.iNrInQueue).ToList();

                        GlobalVar.ActOrdersInQueueList = listDBOrder;

                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden der Auftrag-Warteschlange aus der Datenbank.",
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }

                    // *************************************************************************************
                    // ************************** Aktueller Auftrag in der SPS *****************************
                    // *************************************************************************************
                    try
                    {
                        /*
                        if (GlobalVar.ActOrderInPLC == null)
                            GlobalVar.ActOrderInPLC = new OrderHMI();

                        GlobalVar.ActOrderInPLC.iParamSetId     = DBAct.iTypeparamSetID;
                        GlobalVar.ActOrderInPLC.iQuantity       = DBAct.iOrderQuantity;
                        GlobalVar.ActOrderInPLC.sAddedBy        = DBAct.sOrderAddedBy;
                        GlobalVar.ActOrderInPLC.dtAddedOn       = (DateTime)DBAct.dtOrderAddedOn;
                        GlobalVar.ActOrderInPLC.dtSentToPLC     = (DateTime)DBAct.dtOrderSentToPLC;
                        GlobalVar.ActOrderInPLC.sParamSetname   = context.db_paramset.Single(x => x.id == DBAct.iTypeparamSetID).sName;
                        */

                        if (GlobalVar.ActOrderInPLC == null)
                            GlobalVar.ActOrderInPLC = new db_orderarchiv();
                        if (GlobalVar.PrevOrderInPLC == null)
                            GlobalVar.PrevOrderInPLC = new db_orderarchiv();

                        List<db_orderarchiv> listOrderArchiv = context.db_orderarchiv.ToList();
                        db_orderarchiv tmpOrder = new db_orderarchiv();

                        if (listOrderArchiv.Count > 0)
                        {
                            tmpOrder = listOrderArchiv[listOrderArchiv.Count - 1]; // die aktuellste Komponente aus der Datenbank holen -> das ist immer der letzte, da das "id"-Feld (Primary-Key) der Datenbank mit AutoIncrement definiert ist

                            if (tmpOrder.bActiveInPLC) // es gibt einen aktiven Auftrag in der SPS
                            {
                                DBOrder.Handler.ArchivOrder_Copy(GlobalVar.ActOrderInPLC, tmpOrder); // den aktuellen Auftrag der SPS in die globale Variable laden

                                // auch den zuvor aktiven Auftrag der SPS in die globale Variable laden
                                if (listOrderArchiv.Count > 2) // ...es gibt so einen Auftrag
                                {
                                    tmpOrder = listOrderArchiv[listOrderArchiv.Count - 2];
                                    DBOrder.Handler.ArchivOrder_Copy(GlobalVar.PrevOrderInPLC, tmpOrder);
                                }
                                else // ...es gibt noch keinen solchen Auftrag
                                {
                                    DBOrder.Handler.ArchivOrder_MakeEmpty(GlobalVar.PrevOrderInPLC);
                                }

                            }
                            else // es gibt keinen aktiven Auftrag in der SPS
                            {
                                // den aktuellen Auftrag der SPS in die globale Variable laden
                                DBOrder.Handler.ArchivOrder_MakeEmpty(GlobalVar.ActOrderInPLC);

                                // auch den zuvor aktiven Auftrag der SPS in die globale Variable laden
                                DBOrder.Handler.ArchivOrder_Copy(GlobalVar.PrevOrderInPLC, tmpOrder);

                            }

                        }
                        else
                        {
                            DBOrder.Handler.ArchivOrder_MakeEmpty(GlobalVar.ActOrderInPLC);
                            DBOrder.Handler.ArchivOrder_MakeEmpty(GlobalVar.PrevOrderInPLC);
                        }




                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden der aktuellen Auftragsinformationen aus der Datenbank.",
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }

                    // *************************************************************************************
                    // ********************** Ladeart des nächsten Auftrages in die SPS ********************
                    // *************************************************************************************
                    try
                    {
                        if (DBAct.iOrderStartMode == 0) // Fall Initialisierung der Datenbank, wo die Tabelle noch leer ist. Sonst darf hier nie 0 sein.
                        {
                            DBAct.iOrderStartMode = (int)eOrderStartMode.Manually;
                            GlobalVar.eOrderStartMode = eOrderStartMode.Manually;
                            context.SaveChanges();
                        }
                        else
                        {
                            if ((eOrderStartMode)DBAct.iOrderStartMode == eOrderStartMode.Automatically)
                                GlobalVar.eOrderStartMode = eOrderStartMode.Automatically;
                            else if ((eOrderStartMode)DBAct.iOrderStartMode == eOrderStartMode.Manually)
                                GlobalVar.eOrderStartMode = eOrderStartMode.Manually;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden der Auftrags.",
                                        "Datenbank Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
        }



        // ***********************************************************************************************
        // *********************** Event-Handlers Main: Loaded/Unloaded/Closing **************************
        // ***********************************************************************************************
        #region
        private void MainW_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ShowWindow(hwnd, SW_SHOW);
            if (!App.cultureIsBeingChanged)
            {
                ADSMain.ADSComServer.Disconnect();
            }
        }

        private void MainW_Loaded(object sender, RoutedEventArgs e)
        {
            // ******* Btn "Reset all
            //BtnListMain[0].Register("GVL_Buttons.g_arrButtonsRef[" + ButtonEnums.cBu_ResetALL + "]");


            /// ****** Rechteck für Benachrichtigung für "kein Benutzer" erstellen
            /// Es deckt das gesamte MainWindow bis auf die obere Kopfzeile ab
            /// Wenn kein Benutzer angemeldet ist -> Rechteck wird aktiviert  (Visibility=Visible) -> beim klicken (MouseDown Even) kommt das Dialog
            /// Vorsicht, wenn das Rechteck aktiviert ist, ist es weiterhin unsichtbar, da Fill=Brushes.Transparent.
            rectNoUser.Fill = Brushes.Transparent;
            MainGrid.Children.Add(rectNoUser);
            rectNoUser.SetValue(Grid.RowProperty, 1);
            rectNoUser.SetValue(Grid.RowSpanProperty, 10);
            rectNoUser.SetValue(Grid.ColumnSpanProperty, 10);

            rectNoUser.MouseDown += Rect_MouseDown;

            //Test:
            //TheFrame.Navigate( new PgHandmenuLayout());


            /// *******  Rechteck für dunklen Hintergrund bei Popup-Fenstern erstellen 
            rectDarkBackground.Fill = Brushes.Black;
            rectDarkBackground.Opacity = 0.5;
            MainGrid.Children.Add(rectDarkBackground);
            rectDarkBackground.SetValue(Grid.RowProperty, 0);
            rectDarkBackground.SetValue(Grid.RowSpanProperty, 10);
            rectDarkBackground.SetValue(Grid.ColumnSpanProperty, 10);
            rectDarkBackground.Visibility = Visibility.Collapsed;


            //////// FullScreen ///////////////////////
            // HEINRICH
            if (false) // MBA 26.04.2021: so wird die Taskleiste immer angezeigt. Bei Bedarf die ShowWindow-Methode ohne die if-Bedingung anzeigen
                ShowWindow(hwnd, SW_HIDE);

        }

        private void Rect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GlobalFunc.ShowNoUserMessageBox();
        }


        private void MainW_Unloaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BtnListMain.Count; i++)
                BtnListMain[i].Deregister();

        }

        public void DeregisterItems()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            for (int i = 0; i < ItemList.Count; i++)
            {
                VarCon.RemoveItem(ItemList[i]);
            }
            ItemList.Clear();

            GlobalVar.GVL_Limits.bIsInitialized = false; // MBA 20.8.2020
        }
        #endregion


        public void InitUser()
        {
            // ***********************************************************************************************
            // ************************************ Default-User *********************************************
            // ***********************************************************************************************
            #region
            db_user userDefault = new db_user();
            userDefault.sUserName = "Default User";
            userDefault.sPassword = "";
            userDefault.iUserLevel = GlobalVar.Userlevels.Default.iLevel;
            GlobalVar.defUser = new User("Default User", "", GlobalVar.Userlevels.Default.iLevel);

            bool bDefUserExistsInDB = false; // Flag: ist der Standard-Benutzer in der Datenbank?

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                // ***** Prüfen, ob Default-User in der Datebank existiert
                try
                {
                    bDefUserExistsInDB = context.db_user.Any(u => u.sUserName == userDefault.sUserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Looking for the default user in DB", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // ***** Wenn Default-User nicht existiert, erstellen 
                if (!bDefUserExistsInDB)
                {
                    try
                    {
                        context.db_user.Add(userDefault);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Creating default user in DB", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // ***** Default-User einloggen
                try
                {
                    bool bLogInDefaultUser = false;
                    try
                    {
                        if (GlobalVar.ActUser == null)
                            bLogInDefaultUser = true;
                        else if (GlobalVar.ActUser.sUserName == "")
                            bLogInDefaultUser = true;
                    }
                    catch
                    {

                    }

                    if (bLogInDefaultUser)
                    {
                        Window_LogIn winLogIn = new Window_LogIn();
                        winLogIn.userToLogIn = userDefault;
                        winLogIn.CheckEntries();
                        if (GlobalVar.ActUser.sUserName != userDefault.sUserName)
                            MessageBox.Show("Log-in of the default user failed.", "Log-In default user", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Log-In default user", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            #endregion
        }






        // ***********************************************************************************************
        // ************************************* Fenstergröße ********************************************
        // ***********************************************************************************************
        public void SetWindowSize()
        {
            // ******* Window-Size so anpassen, dass das Fenster über der Taskleiste die maximale Größe hat
            Left = SystemParameters.WorkArea.Left;
            Top = SystemParameters.WorkArea.Top;
            Height = SystemParameters.WorkArea.Height;
            Width = SystemParameters.WorkArea.Width;
            WindowState = WindowState.Normal;
        }


        public void LoadDisplays()
        {
            // *******************************************************************************
            // ************************** Horizontale Navigation *****************************
            // *******************************************************************************
            // Info:
            //      Vertikale    Navigation:     Stationen                      
            //      Horizontale  Navigation:     Displays der jeweilige Station  
            // *******************************************************************************

            // *******************************************************************************************************************************
            // ****************************************************** CHP-Displays 1 *********************************************************
            // *******************************************************************************************************************************
            GlobalVar.dataLOET.Maschine.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnMessages, new PgMessages()));
            GlobalVar.dataLOET.Maschine.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnMessageArchiv, new PgMessageArchiv()));
            //GlobalVar.dataLOET.Maschine.ClusterList1stRow.Add(new NavCluster("Safety Übersicht", new Page()));
            GlobalVar.dataLOET.Maschine.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnAllMachinePar, new PgAllMP()));

            //GlobalVar.dataLOET.Handmenu.ClusterList1stRow.Add(new NavCluster("Layout", new PgHandmenuLayout()));

            
            //GlobalVar.dataLOET.Auftrag.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnOrder, new PgOrder()));
            //GlobalVar.dataLOET.Auftrag.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnOrderArchive, new PgOrderArchiv()));
            //GlobalVar.dataLOET.Auftrag.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnTypePar, new PgTypPar()));
            GlobalVar.dataLOET.Krosy.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavVert_btnKROSY, new PgKROSY()));
            //GlobalVar.dataLOET.Krosy.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnTypePar, new PgTypPar()));

            //*******************************************************************************************************************************
            //***************************************************Projekt - Displays * *********************************************************
            //*******************************************************************************************************************************

            GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavVert_btnManualMenu + " 1", new PgSt10_ManFunc()));
            GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnRFID_WT, new PgRFID_WT(1)));
            //GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnRFID_IND,   new PgRFID_IND(1)));
            GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnDVS + " 1", new PgDVS(1, 2)));
            GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnDVS + " 2", new PgDVS(3, 4)));
            GlobalVar.dataLOET.Station1.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnEvaluation, new PgTempPowerDiagramm(1)));


            GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavVert_btnManualMenu + " 2", new PgSt20_ManFunc()));
            GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnRFID_WT, new PgRFID_WT(2)));
            //GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnRFID_IND,   new PgRFID_IND(2)));
            GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnDVS + " 1", new PgDVS(7, 8)));
            GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnDVS + " 2", new PgDVS(9, 10)));
            GlobalVar.dataLOET.Station2.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnEvaluation, new PgTempPowerDiagramm(2)));

            GlobalVar.dataLOET.Global.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavVert_btnGlobal, new PgGlobal()));



            // *******************************************************************************************************************************
            // ****************************************************** CHP-Displays 2 *********************************************************
            // *******************************************************************************************************************************            
            //GlobalVar.dataLOET.SpecialFunctions.ClusterList1stRow.Add(              new NavCluster("Sonderfunktionen",          new PgSpecialFunctions()));        

            //GlobalVar.dataLOET.Zaehler.ClusterList1stRow.Add(new NavCluster("Counter", new PgCounter()));


            GlobalVar.dataLOET.Einstellungen.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnLanguage, new PgLang()));
            GlobalVar.dataLOET.Einstellungen.ClusterList1stRow.Add(new NavCluster(Properties.Resources.NavHoriz_btnUserManagement, new PgUserManagement()));

            //  *******************************************************************************************************************************

            // ***** Init Display
            // (Dieses Display wird angezeigt, wenn das HMI gestartet wird.)           
            GlobalVar.navHoriz.Station_Name = GlobalVar.dataLOET.Maschine.btnVertNav.Content.ToString();
            GlobalVar.dataLOET.Act_Station = GlobalVar.dataLOET.Maschine;

            // ***** Vertikalen Navigationsbereich laden
            int iTmpModul = 0;
            for (int i = 0; i < GlobalVar.dataLOET.listStatVertNav.Count; i++) // Buttons zur Navigation hinfügen
            {

                if (GlobalVar.dataLOET.listStatVertNav[i].iModulNr > 0
                    && GlobalVar.dataLOET.listStatVertNav[i].iModulNr != iTmpModul)
                {
                    // TextBlocks für Modul 1,2,3... als Separator zur Navigation-StackPanel hinzufügen
                    iTmpModul = GlobalVar.dataLOET.listStatVertNav[i].iModulNr;

                    TextBlock textBlock = new TextBlock();
                    textBlock.Margin = new Thickness(2, 10, 0, 0);
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.FontSize = 12;

                    if (iTmpModul < 99)
                        textBlock.Text = "Modul " + iTmpModul.ToString();
                    else
                        textBlock.Text = "";// Bereich für allgemeine Buttons, unabhängig von Moudl

                    GlobalVar.navVert.Children.Add(textBlock);
                }

                Button button_i = GlobalVar.dataLOET.listStatVertNav[i].btnVertNav;
                button_i.Style = (Style)FindResource("RENA_NavButtonStyle");
                button_i.Click += BtnNavigationClick;
                GlobalVar.navVert.Children.Add(button_i);
            }

            // ***** Die Displays mit dem horizontalen Navigationsbereich verknüpfen
            GlobalVar.navHoriz.LinkWithStDisplays(GlobalVar.dataLOET.Maschine); // Beim Laden diese anzeigen


        }

        // ***********************************************************************************************
        // **************************************** Sprache **********************************************
        // ***********************************************************************************************
        public void SetLanguage()
        {

            // Sprache in der SPS abhängig von Windows-Sprache einstellen (das HMI startet C#-standardmäßig mit der Windows-Sprache)
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                GlobalVar.Language = Languages.englisch; // Sprachumschaltung bei Messages im HMI
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.englisch); // Sprachumschaltung SPS
            }
            else if (CultureInfo.CurrentCulture.Name == "de-DE")
            {
                GlobalVar.Language = Languages.englisch; // Sprachumschaltung bei Messages  im HMI      //MBA 31.05.2021: wir haben keine englische Texte, beim deutshcen Culture sollten auch die Systemtexte von Translated.csv als Englisch angezeigt werden
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.englisch); // Sprachumschaltung SPS               
            }
            else if (CultureInfo.CurrentCulture.Name == "zh-CN")
            {
                GlobalVar.Language = Languages.chinesisch; // Sprachumschaltung bei Messages  im HMI
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.chinesisch); // Sprachumschaltung SPS               
            }
            else if (CultureInfo.CurrentCulture.Name == "sk-SK")
            {
                GlobalVar.Language = Languages.slovakian; // Sprachumschaltung bei Messages  im HMI
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.slovakian); // Sprachumschaltung SPS               
            }
            else if (CultureInfo.CurrentCulture.Name == "fr-FR")
            {
                GlobalVar.Language = Languages.french; // Sprachumschaltung bei Messages  im HMI
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.french); // Sprachumschaltung SPS               
            }
            else if (CultureInfo.CurrentCulture.Name == "ar-AE")
            {
                GlobalVar.Language = Languages.arabisch;
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.arabisch);
            }
            else if (CultureInfo.CurrentCulture.Name == "es-ES")
            {
                GlobalVar.Language = Languages.spanish;
                if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectLang)
                    ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.spanish);
            }
            // ***********************************************************************************************
            // ***************************** Translate Datei einlesen ****************************************
            // ***********************************************************************************************
            // MBA 7.8.2020: diesen Abschnitt evt in SetLanguage() verschieben

            GlobalVar.bUseTranslater = true;
            GlobalVar.Language = Languages.englisch; // MBA 15.3.2020 TEST

            try // falls das Programm am SPS ausgeführt wird
            {
                CHPTrans.CHPTranslate.ReadFile("c:\\CHP\\Translate\\Translated.csv", GlobalVar.bUseTranslater);  // Pfad  SPS
            }
            catch // falls das Programm am Laptop ausgeführt wird
            {
                CHPTrans.CHPTranslate.ReadFile("d:\\001_Projekte\\Translated.csv", GlobalVar.bUseTranslater); // Pfad lokal am Laptop
            }
        }

        #region ButtonClicks
        private void BtnNavigationClick(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                Button button_clicked = sender as Button;
                if (button_clicked.Content.ToString() == "Einstellungen")
                {
                    if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
                    {
                        GlobalFunc.RefreshNavVert(button_clicked);
                    }
                }
                else
                {
                    GlobalFunc.RefreshNavVert(button_clicked);
                }
            }
        }


        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            VarCon.WriteItem("GVL_Panel.bPANStart", true);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            VarCon.WriteItem("GVL_Panel.bPANStop", true);
        }


        private void BtnLoginClick(object sender, RoutedEventArgs e)
        {

            //GlobalFunc.PopUp_SetMainWBackgrDark();
            //
            //Window_LogIn LogInWindow = new Window_LogIn();
            //LogInWindow.ShowDialog();
            //if (LogInWindow.DialogResult == true)
            //{
            //    btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
            //
            //    if (GlobalVar.ActUser.sUserName == "")
            //        btnLoginInMain.Content = Properties.Resources.Main_BtnLogin;
            //}
            //
            //GlobalFunc.PopUp_SetMainWBackgrNormal();


            //***********************************************************


            GlobalFunc.PopUp_SetMainWBackgrDark();

            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Window_LogIn LogInWindow = new Window_LogIn();
                    LogInWindow.ShowDialog();
                    if (LogInWindow.DialogResult == true)
                    {
                        if (GlobalVar.ActUser.sUserName == "")
                        {
                            db_user defaultUser = null;
                            using (CHP_HMIEntities elements = new CHP_HMIEntities())
                            {
                                defaultUser = elements.db_user.Where(u => u.iUserLevel == GlobalVar.Userlevels.Default.iLevel).FirstOrDefault();
                            }
                            User defUser = new User(defaultUser.sUserName, defaultUser.sPassword, defaultUser.iUserLevel);
                            GlobalVar.ActUser = defUser;
                        }
                        btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
                    }
                }
                ), DispatcherPriority.Normal, null);

            GlobalFunc.PopUp_SetMainWBackgrNormal();


            //*******************************************************

            //Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPointUser));
            //newWindowThread.SetApartmentState(ApartmentState.STA); // Singlethreaded Apartment
            //                                                       //newWindowThread.IsBackground = true;
            //newWindowThread.Start(); // Thread starten
            //newWindowThread.Join();  // Warten auf Thread ("This method blocks the calling thread until the thread represented by this instance terminates while continuing to perform standard COM and SendMessage pumping." Quelle: https://www.geeksforgeeks.org/joining-threads-in-c-sharp/#:~:text=In%20C%23%2C%20Thread%20class%20provides,it%20joins%20completes%20its%20execution. )


        }


        private void ThreadStartingPointUser()
        {

            //GlobalFunc.PopUp_SetMainWBackgrDark();

            Window_LogIn LogInWindow = new Window_LogIn();
            LogInWindow.ShowDialog();
            if (LogInWindow.DialogResult == true)
            {
                btnLoginInMain.Content = GlobalVar.ActUser.sUserName;

                if (GlobalVar.ActUser.sUserName == "")
                {
                    db_user defaultUser = null;
                    using (CHP_HMIEntities elements = new CHP_HMIEntities())
                    {
                        defaultUser = elements.db_user.Where(u => u.iUserLevel == GlobalVar.Userlevels.Default.iLevel).FirstOrDefault();
                    }
                    User defUser = new User(defaultUser.sUserName, defaultUser.sPassword, defaultUser.iUserLevel);
                    GlobalVar.ActUser = defUser;
                }
                btnLoginInMain.Content = GlobalVar.ActUser.sUserName;
                //btnLoginInMain.Content = Properties.Resources.Main_BtnLogin;
            }
            //GlobalFunc.PopUp_SetMainWBackgrNormal();
        }


        /// <summary>
        /// Fenster Schließen - CustomButton 
        /// Nur Verfügbar ab Berechtigungsstufe E-Service und Admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnESC_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                ((MainWindow)Application.Current.MainWindow).DeregisterItems();
                GlobalFunc.DeregisterPgMessages();
                GlobalFunc.DeregisterPgOrder();

                ShowWindow(hwnd, SW_SHOW);
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Fenster minimieren - CustomButton
        /// Nur Verfügbar ab Berechtigungsstufe E-Service und Admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGoToWindows_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                ShowWindow(hwnd, SW_SHOW);
                this.WindowState = WindowState.Minimized;
            }

        }
        
        /// <summary>
        /// Shutdown computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShutDown_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }
        #endregion

        private void MainW_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
            {
                if (false)// MBA 26.04.2021: so wird die Taskleiste immer angezeigt. Bei Bedarf die ShowWindow-Methode ohne die if-Bedingung anzeigen
                    ShowWindow(hwnd, SW_HIDE);
            }
            else
            {
                ShowWindow(hwnd, SW_SHOW);
            }
        }

        private void MainW_Closing_1(object sender, CancelEventArgs e)
        {

        }

        private void BtnEvaluation1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnNavigationClick(GlobalVar.dataLOET.Station1.btnVertNav, null);
                GlobalVar.navHoriz.BtnRow1_Click(GlobalVar.navHoriz.ClusterListRow1.Single(c => (string)c.ButtonOfDispl.Content == "Evaluation").ButtonOfDispl, null);

                //GlobalVar.navHoriz.ClusterListRow1[6].ButtonOfDispl.Content
            }
            catch
            {
                ;
            }

        }

        private void BtnEvaluation2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnNavigationClick(GlobalVar.dataLOET.Station2.btnVertNav, null);
                GlobalVar.navHoriz.BtnRow1_Click(GlobalVar.navHoriz.ClusterListRow1.Single(c => (string)c.ButtonOfDispl.Content == "Evaluation").ButtonOfDispl, null);
            }
            catch
            {
                ;
            }
        }

        #region Kategorie-/Datei-Dropdowns (Dokumentenanzeige: PDF / Excel / Video)
        /// <summary>Fuellt das Kategorie-Dropdown (links) mit "Videos" + den Dokument-Kategorien.</summary>
        private void SetCategories()
        {
            categoryCB.Items.Clear();
            foreach (string category in DocumentationPaths.Categories)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = category;
                item.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66509B"));
                item.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
                categoryCB.Items.Add(item);
            }
            categoryCB.SelectedIndex = -1;
            fileCB.Items.Clear();
            fileCB.IsEnabled = false;
        }

        /// <summary>Kategorie gewaehlt -> Datei-Dropdown (rechts) passend fuellen.</summary>
        private void CategoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = categoryCB.SelectedItem as ComboBoxItem;
            if (item == null)
            {
                return;
            }
            SetFilesForCategory(item.Content.ToString());
        }

        /// <summary>Fuellt das Datei-Dropdown mit den Dateien der Kategorie (Anzeige ohne Endung).</summary>
        private void SetFilesForCategory(string category)
        {
            fileCB.Items.Clear();
            fileCB.IsEnabled = true;
            try
            {
                string path = DocumentationPaths.GetCategoryPath(category);
                if (!Directory.Exists(path))
                {
                    return;
                }

                bool isVideo = DocumentationPaths.IsVideoCategory(category);
                foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
                {
                    string ext = file.Extension.ToLowerInvariant();
                    bool supported = isVideo ? (ext == ".mov")
                                             : (ext == ".pdf" || ext == ".xls" || ext == ".xlsx");
                    if (!supported)
                    {
                        continue;
                    }

                    ComboBoxItem cbI = new ComboBoxItem();
                    cbI.Content = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                    cbI.Tag = file.FullName; // vollstaendiger Pfad zum Oeffnen
                    cbI.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66509B"));
                    cbI.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
                    fileCB.Items.Add(cbI);
                }
            }
            catch
            {
                // Ordner ggf. leer/nicht lesbar -> Dropdown bleibt leer.
            }
            fileCB.SelectedIndex = -1;
        }

        /// <summary>Datei gewaehlt -> je nach Dateityp den passenden Viewer oeffnen.</summary>
        private void FileCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = fileCB.SelectedItem as ComboBoxItem;
            if (item == null || item.Tag == null)
            {
                return;
            }

            string fullPath = item.Tag.ToString();
            string ext = System.IO.Path.GetExtension(fullPath).ToLowerInvariant();

            try
            {
                if (ext == ".pdf")
                {
                    new WindowPopUpPdfReader(fullPath).Show();
                }
                else if (ext == ".xls" || ext == ".xlsx")
                {
                    new WindowPopUpExcelReader(fullPath).Show();
                }
                else if (ext == ".mov")
                {
                    WindowPopUpVideoPlayer player = new WindowPopUpVideoPlayer(fullPath);
                    player.Show();
                    player.Closed += Player_Closed;
                    player.Loaded += Player_Loaded;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Datei konnte nicht geoeffnet werden:\n" + ex.Message,
                    "Dokumentenanzeige", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Auswahl zuruecksetzen, damit dieselbe Datei erneut gewaehlt werden kann.
                fileCB.SelectedIndex = -1;
                GlobalFunc.PopUp_SetMainWBackgrNormal();
            }
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
        }

        private void Player_Closed(object sender, EventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }
        #endregion

        private void RectLogo_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowPopUpLogo dialog = new WindowPopUpLogo();
            dialog.Show();
        }

       
    }
}

