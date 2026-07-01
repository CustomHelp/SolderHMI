using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace LOET_HMI.UserControls
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_MessagePLC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string strMsgTxt = "";
    }

    public partial class HMIMessage
    {
        public int iType { get; set; }
        public eMsgType eType { get; set; }
        public string sStation { get; set; }
        public string sBMK { get; set; }
        public string sMsgText1 { get; set; }
        public string sMsgText2 { get; set; }
        public string sMsgText3 { get; set; }
        public string sMsgTextAllHMI { get; set; } // Die einzelnen Texte sind mit einem LESBAREN      Trennzeichen (normalerweise mit Space) separiert. Dies wird im DataGrid angezeigt
        public string sMsgTextAllDB { get; set; } // Die einzelnen Texte sind mit einem EINZIGARTIGEN Trennzeichen (normalerweise mit "§")   separiert, wie in der Datanbank. Dies ist für die Übersetzung wichtig.
        public DateTime? dtCome { get; set; }
        public DateTime? dtGone { get; set; }
        public DateTime? dtQuit { get; set; }

        // Nur für die Puffer-Listen relevant
        public bool bRXFoundInBuf { get; set; } // Dieses Bit nur beim Suchen der neuen Meldung in der Puffer-Liste verwenden. TRUE, wenn die neue Meldung in der Puffer-Liste gefunden wurde. FALSE, wenn nicht.
        public bool bGoneNow { get; set; } // TRUE, wenn eine Meldung vom Typ "MT_ErrorQuit" im AKTUELLEN ADS-Zyklus auf den Typ "MT_ErrorGone" ändert. Sonst FALSE. (Dieses Bit wird nur beim Typ "MT_ErrorQuit"/"MT_ErrorGone" verwendet!!! )
    }

    public enum eMsgType // MBA 13.8.2020
    {
        MT_NoType = 0,
        MT_ErrorQuit = 1,    // Fehler der Quittiert werden muss
        MT_ErrorGone = 2,    // Fehler der schon quittiert ist			
        MT_ErrorMess = 3,    // Fehler der wie eine Meldung behandelt wird
        MT_Warning = 4,    // Warnung (Beispiel: Wartung Roboter)
        MT_Waiting = 5,    // Wartemeldung (Beispiel: Teile Auffüllen)
        MT_Message = 6,    // Meldung (Beispiel: Aktueller Typ ist: )
        MT_Manual = 7,    // Meldung das Handfunktion aktiv ist
        MT_ModulMess = 8		// Meldung vom Modul 
    }

    public enum eOperationMode
    {
        // OperationMode
        OP_0_Off = 0,   // 0 - System ist Aus und Sicherheitsfunktion ist Aktiv
        OP_1_Ready = 1,   // 1 - System ist Aus und Einschaltbereit
        OP_2_StartUp = 2,   // 2 - Hochlauf
        OP_3_Manual = 3,   // 3 - Handbetrieb Aktiv
        OP_4_StepMode = 4,   // 4 - Tippen Aktiv
        OP_5_AutOff = 5,   // 5 - Automatik AUS
        OP_6_AutoON = 6,   // 6 - Automatik EIN 
        OP_7_AutoLast = 7	   // 7 - Autoamtik EIN - Letzter Zyklus
    }


    /// <summary>
    /// Interaktionslogik für UcMessage.xaml
    /// </summary>
    public partial class UcMessage : UserControl, INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // ADS Verbindung 
        IADSConnection VarCon = new ADSService();
        private List<ADSItem> ItemList = new List<ADSItem>();


        /// <summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Überischt der Listen-Suffixe:
        ///     - Err: Meldungen vom Typ    MT_ErrorQuit
        ///                                 MT_ErrorGone
        ///                                 MT_ErrorMess  
        ///                                 MT_Warning
        ///                             
        ///     - Msg: Meldungen vom Typ    MT_Waiting   
        ///                                 MT_Message   
        ///                                 MT_Manual    
        ///                                 MT_ModulMess 
        ///         
        ///     - All: Meldungen von allen Typen (Zusammenfassung der Listen "Err" und "Msg")    
        /////////////////////////////////////////////////////////////////////////////////////////////////////    
        /// Begriffe:
        ///     - Aktive Meldung:   Wenn eine Meldung in der SPS in GVL_Messages.garrMesWindowHMI drin ist, also aktuell von der SPS zum HMI gesendet wird, wird als aktiv gekennzeichnet.
        ///     
        ///                         Sobald eine Meldung durch das HMI empfangen wird, wird es zur Datenbank hinzugefügt. 
        ///                         Solange sie von der SPS ans HMI gesendet wird, ist die "bActive"-Eigenschaft der Meldung in der Datenbank TRUE. 
        ///                         Sobald die Meldung von der SPS nicht mehr ans HMI gesendet wird, wird die "bActive"-Eigenschaft der Meldung in der Datenbank auf FALSE gesetzt.
        ///     
        /// </summary>
        /// 
        List<string> listRawPLCAll = new List<string>(); // Die "rohe" Meldungs-Liste, die direkt von der SPS empfangen wird. Alle Attributen einer Meldung sind noch in einem String, die durch §-Zeichen separiert sind. 

        List<HMIMessage> bufListHMIMsg = new List<HMIMessage>();  // Puffer-Listen zur Zwischenspeicherung der (klassifizierten) Meldungen. Die Meldungen dieser Listen wurden im VORHERIGEN ADS-Zyklus empfangen.
        List<HMIMessage> bufListHMIErr = new List<HMIMessage>();
        List<HMIMessage> bufListHMIAll = new List<HMIMessage>();

        List<HMIMessage> listHMIMsg = new List<HMIMessage>(); // Die klassifizierten Meldungslisten. Die Meldungen dieser Listen werden im AKTUELLEN ADS-Zyklus empfangen.
        List<HMIMessage> listHMIErr = new List<HMIMessage>();
        List<HMIMessage> listHMIAll = new List<HMIMessage>();

        List<HMIMessage> listHMIMsgTransl = new List<HMIMessage>(); // Die klassifizierten UND übersetzten Meldungslisten. Die Meldungen dieser Listen werden im AKTUELLEN ADS-Zyklus empfangen.
        List<HMIMessage> listHMIErrTransl = new List<HMIMessage>();

        List<db_message> listDBActiveAll = new List<db_message>(); // Die Liste mit allen aktuell aktiven Meldungen (Err+Msg) aus der Datenbank. Bei diesen Meldungen ist die "bActive"-Eigenschaft noch TRUE.


        public CollectionViewSource itemCollectionViewSourceMessages = new CollectionViewSource();
        public CollectionViewSource itemCollectionViewSourceErrors = new CollectionViewSource();


        // Betriebsart
        int iActOP;
        public eOperationMode eOP { get; set; }

        private string _sOPImage = "/CHP_HMI;component/Resources/Not-Aus.png";
        public string sOPImage
        {
            get
            {
                return _sOPImage;
            }

            set
            {
                _sOPImage = value;
                OnPropertyChanged();
            }
        }

        // Tanslation
        ICHPTranslate Tanslater = new CHPTransService();

        // DependencyProperties Instanzierung - Eingaben im XAML-Code
        public static DependencyProperty _iModulNrProperty = DependencyProperty.Register("iModulNrProperty", typeof(int), typeof(UcMessage), new PropertyMetadata());
        public int iModulNrProperty
        {
            get { return (int)GetValue(_iModulNrProperty); }
            set { SetValue(_iModulNrProperty, value); }
        }

        public static DependencyProperty _bFilterByStationActive = DependencyProperty.Register("bFilterByStationActive", typeof(bool), typeof(UcMessage), new PropertyMetadata());
        public bool bFilterByStationActive // Optional!!!   //z.B. bei der Lötanlage werden 3 UcMessage-Controls verwendet: 1.: Modul, 2: Station1, 3.: Station2. Das Modul darf nur durch das 1. angesteuert werden, bei 2. und 3. müssen die Buttons deaktivier
        {
            get { return (bool)GetValue(_bFilterByStationActive); }
            set { SetValue(_bFilterByStationActive, value); }
        }

        public static DependencyProperty _iStationNrProperty = DependencyProperty.Register("iStationNrProperty", typeof(int), typeof(UcMessage), new PropertyMetadata());
        public int iStationNrProperty // Optional!!!
        {
            get { return (int)GetValue(_iStationNrProperty); }
            set { SetValue(_iStationNrProperty, value); }
        }

        public static DependencyProperty _strNameProperty = DependencyProperty.Register("strNameProperty", typeof(string), typeof(UcMessage), new PropertyMetadata());
        public string strNameProperty
        {
            get { return (string)GetValue(_strNameProperty); }
            set { SetValue(_strNameProperty, value); }
        }

        // Meldungstabelle aus-/zuklappen
        public int iInitDataGridHeight { get; set; }
        public bool bToggleExpand { get; set; }

        // Statusvariablen
        public bool bUcMessageInitialized { get; set; } // TRUE, wenn der UserControl "UcMessage" initialisiert wurde. Hierfür wird die Methode RegisterOPAndMsg() beim Programmstart durch Main aufgerufen.      Solange die Initialisierung nicht durchgeführt wird, ist das Bit FALSE.
        public bool bUcMessageADSRegistered { get; set; } // TRUE, wenn der UserControl "UcMessage" für die ADS-Kommunikation registriert wurde.
        public bool bFirstDBUpdate { get; set; } // TRUE, wenn die Datenbank nach der UserControl-Initialisierung zum 1. Mal aktualisiert wird (1. Aufruf der Methode UpdateDatabase() ). Sonst FALSE.
        public bool bAnErrHasGoneNow { get; set; } // TRUE, wenn während der Meldungsaktualisierung mindestens eine Fehlermeldung (vom Typ MT_ErrorQuit) geganen ist. Dieses Bit muss am Anfang jeder Meldungsaktualisierung zurückgesetzt werden. 
        public bool bBufListSecondCompareDone { get; set; } // TRUE, wenn die Methode SplitAndSeparateRawPLCList() nach der UserControl-Initialisierung 2-Mal aufgerufen wurde. In den Puffer-Listen stehen dann vernünftige Daten zur Verfügung und die Puffer-Listen können bei der Meldungsarchivierung verwendet werden.

        // Hilfsvariablen
        public int iBufListCompareCount { get; set; }


        public UcMessage()
        {
            // ***** Statusvariablen anpassen:
            bUcMessageInitialized = false;
            bUcMessageADSRegistered = false;
            bBufListSecondCompareDone = false;
            bFirstDBUpdate = true;
            iBufListCompareCount = 0;

            InitializeComponent();
        }

        // *****************************************************************
        // ************************ Load / Unload **************************
        // *****************************************************************
        #region
        private void UcMsg_Loaded(object sender, RoutedEventArgs e)
        {
            if (iStationNrProperty > 0)
            {
                btnOP.Visibility = Visibility.Collapsed;
                repbtnStart.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Collapsed;
                btnQuit.Visibility = Visibility.Collapsed;
            }

            tbModulName.Text = strNameProperty; //"Modul " + iModulNrProperty.ToString() + " - " + strNameProperty;
            iInitDataGridHeight = (int)dGMeldungen.ActualHeight;


            if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectMsg)
            {
                /// Hinweise zum 1. Aufruf beim Programmstart:
                ///     Beim Programmstart wird UcMsg_Loaded() aufgerufen, bevor "maxMessagesHMI" von der SPS abgeholt wurde. Daher darf RegisterOPAndMsg() hier noch nicht aufgerufen werden.
                ///     Stattdessen wird RegisterOPAndMsg() beim Programmstart durch Main aufgerufen, nachdem maxMessagesHMI verfügbar ist. Danach wird bUcMessageInitialized auf TRUE gesetzt.
                if (bUcMessageInitialized && !bUcMessageADSRegistered)
                {
                    RegisterOPAndMsg();
                }
            }
        }

        private void UcMsg_Unloaded(object sender, RoutedEventArgs e)
        {
            //DeregisterOPAndMsg(); MBA 17.05.2021: VORSICHT!!! hier darf es nicht deregistriert werden! Die Meldungen müssen im Hintergrund empfangen und in der Datenbank archiviert werden, auch wenn gerade nicht die Meldungsseite geladen ist!!!
            CollapseDatagrids();
        }

        public void RegisterOPAndMsg()
        {
            try
            {
                // OP:
                ItemList.Add(VarCon.AddItem("GVL_Modul.gModul[" + iModulNrProperty.ToString() + "].stPanel.PLC_TO_HMI.eOP", typeof(Byte))); // ENUM mit 9 Optionen -> Byte reicht aus

                // Messages:
                for (int i = 1; i <= GlobalVar.GVL_Limits.maxMessagesHMI; i++)
                    ItemList.Add(VarCon.AddItem("GVL_Messages.garrMesWindowHMI[" + iModulNrProperty.ToString() + "][" + i.ToString() + "].PLC_TO_HMI.strMessage", typeof(ST_MessagePLC)));

                // Event:
                VarCon.EnableCallbackEvent();
                VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
            }
            catch
            {
                MessageBox.Show("ADS-Registrierung fehlgeschlagen",
                                "UcMessage: RegisterOPAndMsg()",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // ***** Statusvariablen anpassen:
            bUcMessageADSRegistered = true;
        }

        public void DeregisterOPAndMsg()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;

            for (int i = 0; i < ItemList.Count; i++)
            {
                VarCon.RemoveItem(ItemList[i]);
                ItemList[i] = null;
            }
            ItemList.Clear();

            // ***** Statusvariablen anpassen:
            bUcMessageADSRegistered = false;
        }
        #endregion



        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
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

            // *****************************************************************
            // ************** Ständige Aktualisierung - Betriebsart ************
            // *****************************************************************
            #region ItemList[0] - OP:
            try
            {
                if (ItemList[0].Value != null)
                {
                    iActOP = Convert.ToInt32(ItemList[0].Value);

                    var brush = new ImageBrush();
                    switch (iActOP)
                    {
                        case 0:
                            eOP = eOperationMode.OP_0_Off;
                            sOPImage = "/LOET_HMI;component/Resources/OP0NotAus.png";
                            break;

                        case 1:
                            eOP = eOperationMode.OP_1_Ready;
                            sOPImage = "/LOET_HMI;component/Resources/OP1SystemOFF.png";
                            break;

                        case 2:
                            eOP = eOperationMode.OP_2_StartUp;
                            sOPImage = "/LOET_HMI;component/Resources/OP2StartUP.png";
                            break;

                        case 3:
                            eOP = eOperationMode.OP_3_Manual;
                            sOPImage = "/LOET_HMI;component/Resources/OP3Manual.png";
                            break;

                        case 4:
                            eOP = eOperationMode.OP_4_StepMode;
                            sOPImage = "/LOET_HMI;component/Resources/OP4StepMode.png";
                            break;

                        case 5:
                            eOP = eOperationMode.OP_5_AutOff;
                            sOPImage = "/LOET_HMI;component/Resources/OP5AutomaticOFF.png";
                            break;

                        case 6:
                            eOP = eOperationMode.OP_6_AutoON;
                            sOPImage = "/LOET_HMI;component/Resources/OP7AutomaticON.png";
                            break;

                        case 7:
                            eOP = eOperationMode.OP_7_AutoLast;
                            sOPImage = "/LOET_HMI;component/Resources/OP6AutomaticONLC.png";
                            break;
                    }
                    btnOP.Background = brush;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Log("UcMessage.ItemChanged", ex);
            }
            #endregion

            // *****************************************************************
            // ************** Ständige Aktualisierung - Meldungen **************
            // *****************************************************************
            #region ItemList[1..GVL_maxMessage] - Meldungen:
            try // in den ersten Paar Abläufen existiert nur ItemList[0], aber ItemList[1] noch nicht -> man kriegt ein Exception, wenn man auf ItemList[1] zugreifen will
            {
                listRawPLCAll.Clear();
                for (int i = 1; i <= GlobalVar.GVL_Limits.maxMessagesHMI; i++)
                {
                    if (ItemList[i].Value != null) // MBA 11.12.2020
                    {
                        ST_MessagePLC tmp = (ST_MessagePLC)ItemList[i].Value; //VORSICHT: wenn evtl. wieder ein einziges VarCon und ItemList verwendet wird (mit gc_maxMessagesHMI zusammen), dann muss hier [i] stehen
                        listRawPLCAll.Add(tmp.strMsgTxt);
                    }
                }

                // ***** Empfangene Strings zerspalten
                try
                {
                    SplitAndSeparateRawPLCList(listRawPLCAll); // Rohe PLC-Daten verarbeiten: Substrings auslesen, Fehler/Meldung differenzieren und in unterschiedlichen Listen ablegen
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message,
                                    "UcMessage - " + nameof(SplitAndSeparateRawPLCList),
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //// ***** Datenbank aktualisieren
                //try
                //{
                //    UpdateDatabase();
                //}
                //catch (Exception exc)
                //{
                //    MessageBox.Show(exc.Message,
                //                    "UcMessage - " + nameof(UpdateDatabase),
                //                    MessageBoxButton.OK, MessageBoxImage.Error);
                //}

                // ***** Übersetzungen
                try
                {
                    if (GlobalVar.bUseTranslater)
                    {
                        listHMIMsgTransl = TranslateMsgList(listHMIMsg);
                        listHMIErrTransl = TranslateMsgList(listHMIErr);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message,
                                    "UcMessage - " + nameof(TranslateMsgList),
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Log("UcMessage.ItemChanged", ex);
            }


            // ***** Anzeige im DataGrid
            itemCollectionViewSourceMessages.Source = null;
            itemCollectionViewSourceErrors.Source = null;
            itemCollectionViewSourceMessages = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages"));
            itemCollectionViewSourceErrors = (CollectionViewSource)(FindResource("itemCollectionViewSourceErrors"));

            if (GlobalVar.bUseTranslater)
            {
                itemCollectionViewSourceMessages.Source = listHMIMsgTransl;
                itemCollectionViewSourceErrors.Source = listHMIErrTransl;
            }
            else
            {
                itemCollectionViewSourceMessages.Source = listHMIMsg;
                itemCollectionViewSourceErrors.Source = listHMIErr;
            }
            #endregion

        }

        public void SplitAndSeparateRawPLCList(List<string> listRawPLC)
        {
            //bufListHMIMess.Clear();
            //bufListHMIErr.Clear();

            bufListHMIMsg = new List<HMIMessage>();
            bufListHMIErr = new List<HMIMessage>();
            bufListHMIAll = new List<HMIMessage>();

            foreach (HMIMessage msg in listHMIMsg)
            {
                bufListHMIMsg.Add(msg);
            }
            foreach (HMIMessage msg in listHMIErr)
            {
                msg.bRXFoundInBuf = false;
                bufListHMIErr.Add(msg);
            }
            foreach (HMIMessage msg in listHMIAll)
            {
                msg.bRXFoundInBuf = false;
                bufListHMIAll.Add(msg);
            }


            // Aktuelle Listen zurücksetzen
            listHMIMsg.Clear();
            listHMIErr.Clear();
            listHMIAll.Clear();
            bAnErrHasGoneNow = false;

            // Temporäre Hilfsvariablen
            string sTmpRawPLCMsg;

            eMsgType eTmpType = eMsgType.MT_NoType;
            int iTmpType = 0;
            //string   sTmpStat;
            int iTmpStat;
            string sTmpBMK;
            string sTmpMsgText1;
            string sTmpMsgText2;
            string sTmpMsgText3;

            List<string[]> tmpListOfStrArr = new List<string[]>(); // nur Ansatz 2

            bool bFilterConditionMet = false;

            for (int i = 0; i < listRawPLC.Count; i++)
            {
                if (listRawPLC[i] != "")
                {
                    // *******************************************************************************
                    // **** Den empfangen String zerspalten und in temporären Variablen speichern ****
                    // *******************************************************************************
                    tmpListOfStrArr.Add(listRawPLC[i].Split('§'));

                    // ***** StringArray[0]: "Priorität", "Typ", "Station"      -> muss weiter zerspaltet werden
                    sTmpRawPLCMsg = tmpListOfStrArr[i][0];

                    //      -> "Priorität" in StringArray[0]:
                    //      im HMI unrelevant

                    //      -> "Typ" in StringArray[0]:
                    iTmpType = Convert.ToInt32(sTmpRawPLCMsg[1].ToString()); // Zeichen bei Index 1 (char) -> in String konvertieren -> in Int konvertieren

                    //      -> "Station" in StringArray[0]:
                    iTmpStat = Convert.ToInt32(sTmpRawPLCMsg.Substring(2)); // Substring: ab Startposition 2 bis Ende des Strings

                    // ***** StringArray[1]: BMK
                    sTmpBMK = tmpListOfStrArr[i][1];

                    // ***** StringArray[2]: Variable
                    // --- noch fehlt ---

                    // ***** StringArray[3]: Meldetext 1
                    sTmpMsgText1 = tmpListOfStrArr[i][3];

                    // ***** StringArray[4]: Meldetext 2
                    sTmpMsgText2 = tmpListOfStrArr[i][4];

                    // ***** StringArray[5]: Meldetext 3
                    sTmpMsgText3 = tmpListOfStrArr[i][5];

                    // ***** Message-Typ
                    switch (iTmpType)
                    {
                        case 0:
                            eTmpType = eMsgType.MT_NoType;
                            break;
                        case 1:
                            eTmpType = eMsgType.MT_ErrorQuit;
                            break;
                        case 2:
                            eTmpType = eMsgType.MT_ErrorGone;
                            break;
                        case 3:
                            eTmpType = eMsgType.MT_ErrorMess;
                            break;
                        case 4:
                            eTmpType = eMsgType.MT_Warning;
                            break;
                        case 5:
                            eTmpType = eMsgType.MT_Waiting;
                            break;
                        case 6:
                            eTmpType = eMsgType.MT_Message;
                            break;
                        case 7:
                            eTmpType = eMsgType.MT_Manual;
                            break;
                        case 8:
                            eTmpType = eMsgType.MT_ModulMess;
                            break;

                    }

                    // *******************************************************************************
                    // ************************** Filter nach Station ********************************
                    // *******************************************************************************
                    if (bFilterByStationActive) // Prüfung: alle Modul-Meldungen anzeigen oder die Meldungen der Modul nach einer Station filtern?
                    {
                        if (iTmpStat == iStationNrProperty)
                            bFilterConditionMet = true;
                        else
                            bFilterConditionMet = false;
                    }
                    else
                    {
                        bFilterConditionMet = true;
                    }

                    if (bFilterConditionMet)
                    {
                        // *******************************************************************************
                        // ************************** Meldungsobjekt erstellen ***************************
                        // *******************************************************************************
                        HMIMessage msgRX = new HMIMessage(); //  Die aktuell empfangene Fehlermeldung (RX: receive)
                        msgRX.iType = iTmpType;
                        msgRX.eType = eTmpType;
                        msgRX.sBMK = sTmpBMK;
                        msgRX.sMsgText1 = sTmpMsgText1;
                        msgRX.sMsgText2 = sTmpMsgText2;
                        msgRX.sMsgText3 = sTmpMsgText3;
                        //msgRX.sMsgTextAllHMI = sTmpMsgText1 + " " + sTmpMsgText2 + " " + sTmpMsgText3;
                        //msgRX.sMsgTextAllDB  = sTmpMsgText1 + "§" + sTmpMsgText2 + "§" + sTmpMsgText3;
                        msgRX.sMsgTextAllHMI = GlobalFunc.ConcatenateMessage(sTmpMsgText1, sTmpMsgText2, sTmpMsgText3, GlobalFunc.eConcatenateMessageMode.ForHMI);
                        msgRX.sMsgTextAllDB = GlobalFunc.ConcatenateMessage(sTmpMsgText1, sTmpMsgText2, sTmpMsgText3, GlobalFunc.eConcatenateMessageMode.ForDB);
                        msgRX.dtCome = DateTime.Now; // Init.Wert, wird ggf. gleich überschrieben
                        msgRX.dtGone = null;         // Init.Wert, wird ggf. gleich überschrieben
                        msgRX.dtQuit = null;         // Init.Wert, wird ggf. gleich überschrieben

                        // *******************************************************************************
                        // ************ Zeiteigenschaften des Meldungsobjekts vom Puffer holen ***********
                        // *******************************************************************************
                        // nach der empfangenen Meldung in der Puffer-Liste suchen -> falls gefunden, den Empfangszeitpunkt von der Puffer-Liste immer wieder übernehmen
                        for (int j = 0; j < bufListHMIAll.Count; j++)
                        {
                            if ((bufListHMIAll[j].sBMK == msgRX.sBMK)
                                && (bufListHMIAll[j].sMsgText1 == msgRX.sMsgText1)
                                && (bufListHMIAll[j].sMsgText2 == msgRX.sMsgText2)
                                && (bufListHMIAll[j].sMsgText3 == msgRX.sMsgText3)
                                && (bufListHMIAll[j].sMsgTextAllDB == msgRX.sMsgTextAllDB)) // die empfangene Meldung in der der Puffer-Liste gefunden -> die empfangene Meldung ist KEINE neue Meldung 
                            {
                                bufListHMIAll[j].bRXFoundInBuf = true;

                                // Zeit "gekommen":
                                msgRX.dtCome = bufListHMIAll[j].dtCome; // da die Meldung in der Puffer-Liste schon drin war, die Ankunftszeit immer wieder von der Puffer-Liste abholen

                                // Zeit "gegangen":
                                if (bufListHMIAll[j].eType == eMsgType.MT_ErrorQuit
                                   && msgRX.eType == eMsgType.MT_ErrorGone)
                                {
                                    msgRX.dtGone = DateTime.Now;
                                    msgRX.bGoneNow = true;
                                    bAnErrHasGoneNow = true;
                                }
                                else
                                {
                                    msgRX.dtGone = bufListHMIAll[j].dtGone;
                                    msgRX.bGoneNow = false;
                                }

                                // Zeit "quit":
                                //      Wird direkt in der Datenbank gespeichert, hier nicht (siehe UpdateDatabase())


                                // Sonderfall: der Fehler ist gegangen, wurde noch nicht quittiert und kam wieder
                                if (bufListHMIAll[j].eType == eMsgType.MT_ErrorGone
                                    && msgRX.eType == eMsgType.MT_ErrorQuit
                                    && msgRX.dtQuit == null)
                                {
                                    msgRX.dtCome = DateTime.Now;
                                    msgRX.dtGone = null;
                                    msgRX.dtQuit = null;
                                }
                                break;
                            }
                        }

                        msgRX.sStation = GlobalFunc.GetStationName_DB(iModulNrProperty, iTmpStat);

                        // *******************************************************************************
                        // **************** Meldungsobjekt zur jeweiligen Liste hinzufügen ***************
                        // *******************************************************************************
                        listHMIAll.Add(msgRX);

                        if (msgRX.eType <= eMsgType.MT_Warning)
                            listHMIErr.Add(msgRX); // für die Anzeige
                        else
                            listHMIMsg.Add(msgRX); // für die Anzeige
                    }
                    else
                    {
                        ;
                    }

                }
                else
                {
                    break;
                }
            }


            // Aufruf der aktuellen Methode (SplitAndSeparateRawPLCList) bis zum 2. Aufruf nach der UserControl-Initialisierung zählen
            if (iBufListCompareCount <= 2)
            {
                iBufListCompareCount++;
                if (iBufListCompareCount == 2)
                {
                    bBufListSecondCompareDone = true; // Statusbit setzen
                }
            }

        }

        public enum eStatNameReturnMode
        {
            StNameForDB = 10,
            StNameForTranslate = 20,
        }






        private void UpdateDatabase()
        {

            int index = -1;
            bool bFound = false;

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                string strTempStationName = GlobalFunc.GetStationName_DB(iModulNrProperty, iStationNrProperty);

                listDBActiveAll = new List<db_message>();
                listDBActiveAll = context.db_message.Where(m => (m.bActive == true) && (m.iModul == iModulNrProperty) && (m.sStation == strTempStationName)).ToList();

                // ***** DB: check active msg   -> Aktive Meldungen in der DB prüfen: sind sie in der SPS immer noch aktiv?
                #region
                if (bFirstDBUpdate)
                {
                    try
                    {
                        for (int i = 0; i < listDBActiveAll.Count; i++) // zu jedem DB-Eintrag die gleiche Meldung in der HMI-Liste finden
                        {
                            int iFoundInd = -1;

                            for (int k = 0; k < listHMIAll.Count; k++)
                            {
                                if (listDBActiveAll[i].sMessage == listHMIAll[k].sMsgTextAllDB && 
                                    listDBActiveAll[i].iModul == iModulNrProperty && 
                                    listDBActiveAll[i].sStation == listHMIAll[k].sStation)
                                {
                                    bFound = true; // gefunden
                                    iFoundInd = k;
                                    break;
                                }
                            }

                            if (bFound) // gefunden
                            {
                                listHMIAll[iFoundInd].dtCome = listDBActiveAll[i].dtCome; // Zeit von der Datenbank übernehmen
                            }
                            else // nicht gefunden
                            {
                                listDBActiveAll[i].bActive = false; // die DB-Einträge, die noch als "aktiv" gepeichert sind, nun als "nicht aktiv" speichern
                                listDBActiveAll[i].dtGone = DateTime.Now;
                                context.SaveChanges();
                            }
                            bFound = false;
                            iFoundInd = -1;
                        }
                        listDBActiveAll = new List<db_message>();
                        listDBActiveAll = context.db_message.Where(m => m.bActive == true && m.iModul == iModulNrProperty).ToList();

                    }
                    catch
                    {
                        MessageBox.Show("The messages that are stored in the database as active, couldn't be loaded.", // "Die Meldungen, die in der Datenbank als aktiv gespeichert sind, konnten nicht geladen werden", // Text
                                        "db_message - Initialization", // Überschrift 
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                    bFirstDBUpdate = false;
                }
                #endregion

                // ***** DB: add new msg        -> Neue Meldungen zur DB hinzufügen  
                #region
                for (int i = 0; i < listHMIAll.Count; i++)
                {
                    index = listDBActiveAll.FindIndex(m => m.sMessage == listHMIAll[i].sMsgTextAllDB && m.iModul == iModulNrProperty && m.sStation == listHMIAll[i].sStation);

                    if (index < 0) // der Eintrag der HMI-Liste wurde in der DB-Aktivliste nicht gefunden -> hinzufügen
                    {
                        try
                        {
                            db_message newDBmessage = new db_message();
                            newDBmessage.iModul = iModulNrProperty;
                            //newDBmessage.sMessage   = listHMIAll[i].sMsgTextAll;
                            newDBmessage.sMessage = listHMIAll[i].sMsgTextAllDB; //MBA 26.05.2021
                            newDBmessage.dtCome = (DateTime)listHMIAll[i].dtCome;
                            DateTime dtGone = new DateTime();
                            if (listHMIAll[i].dtGone != null)
                            {
                                dtGone = (DateTime)listHMIAll[i].dtGone;
                            }
                            DateTime dtQuit = new DateTime();
                            if (listHMIAll[i].dtQuit != null)
                            {
                                dtQuit = (DateTime)listHMIAll[i].dtQuit;
                            }
                            newDBmessage.dtGone = dtGone;
                            newDBmessage.dtQuit = dtQuit;
                            newDBmessage.sBMK = listHMIAll[i].sBMK;
                            newDBmessage.sStation = listHMIAll[i].sStation;
                            newDBmessage.iType = (int)listHMIAll[i].eType;
                            newDBmessage.sUserName = GlobalVar.ActUser.sUserName;
                            newDBmessage.bActive = true; //!!!

                            context.db_message.Add(newDBmessage);
                            context.SaveChanges();

                            listDBActiveAll = new List<db_message>();
                            listDBActiveAll = context.db_message.Where(m => m.bActive == true && m.iModul == iModulNrProperty).ToList();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "\n\n" + listHMIAll[i].sMsgTextAllHMI,
                                            "db_message - Fehler bei neuen Meldungen",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                #endregion

                // ***** DB: update dtGone      -> Gegangene Fehler: Zeit und Typ in der DB anpassen, wenn der Typ "MT_ErrorQuit" sich auf "MT_ErrorGone" geändert hat
                #region
                if (bAnErrHasGoneNow) // Irgendeine Fehlermeldung "ist gegangen" ("MT_ErrorQuit" -> "MT_ErrorGone"). In der FOR-Schleife wird es geschaut, welche genau.
                {
                    for (int i = 0; i < listDBActiveAll.Count; i++)
                    {
                        try
                        {
                            long tmpID_DB = listDBActiveAll[i].id;
                            db_message msgDBToUpdate = context.db_message.Single(m => m.id == tmpID_DB); // Hinweis: "id" ist long und LINQ hat Probleme damit, wenn hier Single(m => m.id == listDBActiveError[i].id) stehen würde. Deswegen wird es auf 2 Befehle aufgeteilt

                            index = listHMIAll.FindIndex(m => m.sMsgTextAllDB == listDBActiveAll[i].sMessage && listDBActiveAll[i].iModul == iModulNrProperty && m.sStation == listDBActiveAll[i].sStation); //

                            if (index >= 0)  // der zu aktualisierende DB-Eintrag wurde in der aktuellen HMI-Liste gefunden
                            {
                                HMIMessage msgHMI = listHMIAll[index];

                                if (msgHMI.bGoneNow) // Einzeln prüfen, ob die jeweilige Fehlermeldung "gegangen ist". (Das Bit bGoneNow wird nur beim "MT_ErrorQuit"/"MT_ErrorGone" Übergang verwendet! Bei den anderen Typen nicht.)
                                {
                                    msgDBToUpdate.dtGone = (DateTime)msgHMI.dtGone;
                                    msgDBToUpdate.iType = (int)eMsgType.MT_ErrorGone;
                                    context.SaveChanges();

                                    listDBActiveAll = new List<db_message>();
                                    listDBActiveAll = context.db_message.Where(m => m.bActive == true && m.iModul == iModulNrProperty).ToList();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "\n\n" + listDBActiveAll[i].sMessage,
                                            "db_message: updating dtGone, iType ('MT_ErrorQuit' -> 'MT_ErrorGone')", //"db_message - Fehler bei gegangenen Fehlern (Typ 'MT_ErrorGone')",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }


                    }
                }
                #endregion

                // ***** DB: update bActive, dtQuit/dtQGone -> Nach Meldungen suchen, die nicht mehr aktiv sind (von der SPS nicht mehr ans HMI gesendet werden) und bei diesen bActive und dtQuit (beim Typ "MT_ErrorGone") bzw. dtQGone (bei allen anderen Typen) anpassen
                #region
                for (int i = 0; i < bufListHMIAll.Count; i++)
                {
                    if (!bufListHMIAll[i].bRXFoundInBuf)
                    {
                        // Fehler wurde gerade quittiert
                        if (bufListHMIAll[i].eType == eMsgType.MT_ErrorGone && bBufListSecondCompareDone) // Quittierte Fehler (Typ in der Puffer-Liste ist noch "MT_ErrorGone", aber in der aktuellen Liste es nicht mehr drin)
                        {
                            try
                            {
                                // MBA 26.05.2021:  "listDBActiveAll[i].iModul == iModulNrProperty"   geändert auf   "m.iModul == iModulNrProperty"
                                //long idQuitedErr = listDBActiveAll.Single(m => m.sMessage == bufListHMIAll[i].sMsgTextAll        &&      listDBActiveAll[i].iModul == iModulNrProperty   &&      m.sStation == bufListHMIAll[i].sStation).id;
                                long idQuitedErr = listDBActiveAll.Single(m => m.sMessage == bufListHMIAll[i].sMsgTextAllDB && m.iModul == iModulNrProperty && m.sStation == bufListHMIAll[i].sStation).id;
                                context.db_message.Single(m => m.id == idQuitedErr).dtQuit = DateTime.Now;
                                context.db_message.Single(m => m.id == idQuitedErr).bActive = false;

                                // MBA 21.05.2021
                                if (context.db_message.Single(m => m.id == idQuitedErr).dtGone == null)
                                    context.db_message.Single(m => m.id == idQuitedErr).dtGone = context.db_message.Single(m => m.id == idQuitedErr).dtQuit;

                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message + "\n\n" + bufListHMIAll[i].sMsgTextAllDB,
                                                "db_message: updating dtQuit, bActive (MT_ErrorGone, acknowledging)", //"db_message - Fehler bei quittierten Fehlern",
                                                MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        // Selbstquittierende Fehler/Meldungen
                        else if (bufListHMIAll[i].iType >= (int)eMsgType.MT_ErrorMess && bBufListSecondCompareDone) // Alle andere Meldungen, die ohne Quittieren von sich selbst weggehen können
                        {
                            try
                            {
                                // Hallo Philipp, hier fix für Fehlermeldung bei UpdateDatabase Quittieren Fehlermedlung
                                List<db_message> msgs = listDBActiveAll.Where(m => m.sMessage == bufListHMIAll[i].sMsgTextAllDB && m.iModul == iModulNrProperty && m.sStation == bufListHMIAll[i].sStation).ToList();
                                if (msgs.Count > 0)
                                {
                                    foreach (db_message msg in msgs)
                                    {
                                        long idMsgGone = msg.id;
                                        context.db_message.Single(m => m.id == idMsgGone).dtGone = DateTime.Now;
                                        context.db_message.Single(m => m.id == idMsgGone).bActive = false;
                                        context.SaveChanges();
                                    }
                                }
                                //long idMsgGone = listDBActiveAll.Single(m => m.sMessage == bufListHMIAll[i].sMsgTextAllDB && m.iModul == iModulNrProperty && m.sStation == bufListHMIAll[i].sStation).id;
                                //context.db_message.Single(m => m.id == idMsgGone).dtGone = DateTime.Now;
                                //context.db_message.Single(m => m.id == idMsgGone).bActive = false;
                                //context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message + "\n\n" + bufListHMIAll[i].sMsgTextAllDB,
                                                "db_message: updating dtGone, bActive (>=MT_ErrorMess, self-acknowledging)", //"db_message - Fehler bei gegangenen Meldungen (selbstquittierend)",
                                                MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                #endregion
            }
        }



        public List<HMIMessage> TranslateMsgList(List<HMIMessage> listToTransl)
        {
            List<HMIMessage> listTranslated = new List<HMIMessage>();

            for (int i = 0; i < listToTransl.Count; i++)
            {
                HMIMessage hmiMessage = new HMIMessage();

                hmiMessage.eType = listToTransl[i].eType;
                hmiMessage.sBMK = listToTransl[i].sBMK;
                hmiMessage.dtCome = listToTransl[i].dtCome;
                hmiMessage.dtGone = listToTransl[i].dtGone;
                hmiMessage.sStation = GlobalFunc.GetStationName_Translate(iModulNrProperty, listToTransl[i].sStation);

                hmiMessage.sMsgText1 = Tanslater.TransTxt(listToTransl[i].sMsgText1, eFBType.fb_Message);
                hmiMessage.sMsgText2 = Tanslater.TransTxt(listToTransl[i].sMsgText2, eFBType.fb_Message);
                hmiMessage.sMsgText3 = Tanslater.TransTxt(listToTransl[i].sMsgText3, eFBType.fb_Message);

                //hmiMessage.sMsgTextAllHMI = hmiMessage.sMsgText1 + " " + hmiMessage.sMsgText2 + " " + hmiMessage.sMsgText3;
                //hmiMessage.sMsgTextAllDB  = hmiMessage.sMsgText1 + "§" + hmiMessage.sMsgText2 + "§" + hmiMessage.sMsgText3;
                hmiMessage.sMsgTextAllHMI = GlobalFunc.ConcatenateMessage(hmiMessage.sMsgText1, hmiMessage.sMsgText2, hmiMessage.sMsgText3, GlobalFunc.eConcatenateMessageMode.ForHMI);
                hmiMessage.sMsgTextAllDB = GlobalFunc.ConcatenateMessage(hmiMessage.sMsgText1, hmiMessage.sMsgText2, hmiMessage.sMsgText3, GlobalFunc.eConcatenateMessageMode.ForDB);


                listTranslated.Add(hmiMessage);
            }

            return listTranslated;
        }



        // *****************************************************************
        // ****************************  Buttons ***************************
        // *****************************************************************
        #region
        private void BtnOP_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                //VarCon.WriteItem("GVL_Module.gBedienstellen[" + iModulNrProperty.ToString() + "].bSelectStepMode", !(iActOP == 4));
                VarCon.WriteItem("GVL_Modul.gModul[" + iModulNrProperty.ToString() + "].stPanel.HMI_TO_PLC.bStepMode_Choose", true);
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }



        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                //VarCon.WriteItem("GVL_Panel.bPANM" + iModulNrProperty.ToString() + "Start", true); // "GVL_Panel.bPANM1Start" 
                VarCon.WriteItem("GVL_Modul.gModul[" + iModulNrProperty.ToString() + "].stPanel.HMI_TO_PLC.bStart", true);

            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }


        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                //VarCon.WriteItem("GVL_Panel.bPANM" + iModulNrProperty.ToString() + "Stop", true); 
                VarCon.WriteItem("GVL_Modul.gModul[" + iModulNrProperty.ToString() + "].stPanel.HMI_TO_PLC.bStop", true);
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Default.iLevel)
            {
                //VarCon.WriteItem("GVL_Panel.bPANM" + iModulNrProperty.ToString() + "Quit", true); 
                VarCon.WriteItem("GVL_Modul.gModul[" + iModulNrProperty.ToString() + "].stPanel.HMI_TO_PLC.bQuit", true);

            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }
        #endregion



        // *****************************************************************
        // ******************* Meldungen  Aus- / Zuklappen *****************
        // *****************************************************************
        #region
        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            bToggleExpand = !bToggleExpand;

            if (bToggleExpand) // Ausklappen
            {
                ExpandDatagrids();
            }
            else // Zuklappen
            {
                CollapseDatagrids();
            }
        }

        private void ExpandDatagrids()
        {
            dGFehler.Height = 2 * iInitDataGridHeight;
            dGMeldungen.Height = 2 * iInitDataGridHeight;
            btnExpand.Content = "-"; // Taster für den nächsten Zuklappen vorbereiten
        }
        private void CollapseDatagrids()
        {
            dGFehler.Height = 1 * iInitDataGridHeight;
            dGMeldungen.Height = 1 * iInitDataGridHeight;
            btnExpand.Content = "+"; // Taster für den nächsten Ausklappen vorbereiten
        }
        #endregion


    }
}
