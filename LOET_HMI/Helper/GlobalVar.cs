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
using LOET_HMI.UserControls;
using LOET_HMI.Displays;

namespace LOET_HMI
{
    public class GlobalVar
    {
        #region Ordner-Pfade Anleitungen
        public static string videoPath = "";
        public static string manualPath = "";
        public static bool manualExists = true;
        public static bool videoExists = true;
        #endregion
        static public bool bUseTranslater { get; set; } // TRUE, wenn die Daten von der SPS (Meldungen, Popup, Zylinder, Maschienen- und Typ-Parameter usw) übersetzt werden sollen
        static public Languages Language {get; set;}

        static private User _ActUser;
        static public User ActUser
        {
            get { return _ActUser; }
            set
            {
                _ActUser = value;
                if (_ActUser.sUserName == "")
                    GlobalFunc.ActivateNoUserRect();
                else
                    GlobalFunc.DeactivateNoUserRect();
            }
        }

        public static User defUser = null;

        static public db_paramset ActRecipe { get; set; }
        static public List<db_parameter> ActRecipeParamList { get; set; }


        static public db_paramset ActTypeParamSet { get; set; }
        static public List<db_parameter> ActTypeParamList { get; set; }

        static public db_paramset ActMachineParamSet { get; set; }
        static public List<db_parameter> ActMachineParamList { get; set; }

        //static public OrderHMI ActOrderInPLC { get; set; }
        static public db_orderarchiv ActOrderInPLC { get; set; } // MBA 15.10.2020: der aktuell aktive Auftrag
        static public db_orderarchiv PrevOrderInPLC { get; set; } // MBA 16.10.2020: "Previous Order", der zuvor aktive Auftrag in der SPS
        static public List<db_orderqueue> ActOrdersInQueueList { get; set; }
        static public eOrderStartMode eOrderStartMode { get; set; }

        static public bool bHMIInitialized { get; set; }

        // MBA 15.10.2020:
        static public void UpdateActOrderInPLC()
        {
            UpdateOrder(eUpdateGlobOrder.ActOrderInPLC);
        }

        static public void UpdatePrevOrderInPLC()
        {
            UpdateOrder(eUpdateGlobOrder.PrevOrderInPLC);
        }

        public enum eUpdateGlobOrder
        {
            ActOrderInPLC   = 1,
            PrevOrderInPLC  = 2
        }

        static private void UpdateOrder(eUpdateGlobOrder eOrderToUpdate)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                List<db_orderarchiv> listOrdersDB = context.db_orderarchiv.ToList();

                bool bDoUpdate = false;
                string sOrderToUpdateName = "";

                try
                {
                    if (listOrdersDB.Count > 0) // Order-Tabelle ist nicht leer
                    {                       
                        db_orderarchiv tmpOrderDB = new db_orderarchiv();
                        db_orderarchiv refOrderToUpdate = new db_orderarchiv() ;


                        if (eOrderToUpdate == eUpdateGlobOrder.ActOrderInPLC)
                        {
                            refOrderToUpdate   = GlobalVar.ActOrderInPLC;
                            sOrderToUpdateName = nameof(GlobalVar.ActOrderInPLC);

                            if (listOrdersDB[listOrdersDB.Count - 1].bActiveInPLC) // letzer Eintrag der
                            {
                                bDoUpdate  = true;
                                tmpOrderDB = listOrdersDB[listOrdersDB.Count - 1];
                            }                               
                            else
                            {
                                bDoUpdate  = false;
                            }
                               
                            
                        }                            
                        else if (eOrderToUpdate == eUpdateGlobOrder.PrevOrderInPLC)
                        {
                            refOrderToUpdate   = GlobalVar.PrevOrderInPLC;
                            sOrderToUpdateName = nameof(GlobalVar.PrevOrderInPLC);

                            if (listOrdersDB[listOrdersDB.Count - 1].bActiveInPLC   &&  listOrdersDB.Count>=2)
                            {
                                bDoUpdate  = true;
                                tmpOrderDB = listOrdersDB[listOrdersDB.Count - 2];
                            }
                            else if(!listOrdersDB[listOrdersDB.Count - 1].bActiveInPLC && listOrdersDB.Count >= 2)
                            {
                                tmpOrderDB = listOrdersDB[listOrdersDB.Count - 1];
                                bDoUpdate  = true;
                            }
                            else
                                bDoUpdate  = false;

                            
                        } 
                        
                        if(bDoUpdate)
                        {
                            if (refOrderToUpdate != null)   //Heinrich
                            {
                                refOrderToUpdate.sParamsetName = tmpOrderDB.sParamsetName;
                                refOrderToUpdate.iQuantityTarget = tmpOrderDB.iQuantityTarget;
                                refOrderToUpdate.iCartonQuantProPallet = tmpOrderDB.iCartonQuantProPallet;
                                refOrderToUpdate.sAddedByToQueue = tmpOrderDB.sAddedByToQueue;
                                refOrderToUpdate.dtAddedOnToQueue = tmpOrderDB.dtAddedOnToQueue;
                                refOrderToUpdate.dtLoadedToPLCOn = tmpOrderDB.dtLoadedToPLCOn;
                                refOrderToUpdate.iState = tmpOrderDB.iState;
                                refOrderToUpdate.bActiveInPLC = tmpOrderDB.bActiveInPLC;

                                refOrderToUpdate.iCartonDone = tmpOrderDB.iCartonDone;
                                refOrderToUpdate.iQuantityDone = tmpOrderDB.iQuantityDone;
                                refOrderToUpdate.dtStartedOn = tmpOrderDB.dtStartedOn as DateTime? ?? default(DateTime); // https://stackoverflow.com/questions/1772025/sql-data-reader-handling-null-column-values
                                refOrderToUpdate.dtFinishedOn = tmpOrderDB.dtFinishedOn as DateTime? ?? default(DateTime);
                                refOrderToUpdate.dtCancelledOn = tmpOrderDB.dtCancelledOn as DateTime? ?? default(DateTime);
                            }
                        }
                        else
                        {
                            DBOrder.Handler.ArchivOrder_MakeEmpty(refOrderToUpdate);
                        }

                    }
                    else
                    {
                        DBOrder.Handler.ArchivOrder_MakeEmpty(ActOrderInPLC);
                        DBOrder.Handler.ArchivOrder_MakeEmpty(PrevOrderInPLC);
                    }
                }
                catch
                {
                    MessageBox.Show("Aktualisierung der Variable '" + sOrderToUpdateName + "' fehlgeschlagen", // Text
                                    "Fehler", // Überschrift
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }


            }
        }


        //******************************************************


        /// <summary>
        /// Vorteil von diesem Ansatz: 
        ///     - Alle Userlevels sind direkt durch Namen ansprechbar
        ///     - Alle Userlevels werden zu einer Liste hinzufegüt, also für die Methode List.Single() auch nutzbar
        ///     - Im Konstruktor können schnell die vorhandenen Userlevels bearbeitet werden (sName, iLevel) und weitere Userlevels erstellt werden
        /// </summary>
        static public UserLevelsProject  Userlevels { get; set; }


        static public ProjectData      dataLOET { get; set; }
        static public UcNavWorksp   navHoriz { get; set; }
        static public StackPanel    navVert  { get; set; }

        // ***********************************************************
        // ********************** Debug-Klassen **********************
        // ***********************************************************
        static public ADSDebug debugADS { get; set; }
        static public DBDebug debugDB { get; set; }

        //static public bool bPLCConnected { get; set; } //MBA 10.8.2020: von Main hierher verschoben


        // ***********************************************************
        // *********************** GVL Limits ************************
        // ***********************************************************
        static public class GVL_Limits
        {
            static public bool bIsInitialized               { get; set; }       // TRUE, wenn alle Limits einen Wert>0 haben
            //***********
            static public int maxModule                     { get; set; }       // Maximale Anzahl an Modulen
            static public int maxStations                   { get; set; }       // Maximale Anzahl an Stationen
            static public int maxMessages                   { get; set; }       // Maximale Anzahl an Meldungen je Station
            static public int maxMessagesHMI                { get; set; }       // Maximale Anzahl an Meldungen je Modul für HMi
            static public int maxControlSections            { get; set; }       // Maximale Anzahl an Bedienstellen mit Notaus oder Freigabetaster
            static public int maxSafeGuards                 { get; set; }       // Maximale Anzahl an Schutzvorrichtungen wie Lichtgitter oder Schutztüren
            static public int maxAxis                       { get; set; }       // Maximale Anzahl an Achsen
            static public int maxRobot                      { get; set; }       // Maximale Anzahl an Roboter
            static public int maxConverter                  { get; set; }       // Maximale Anzahl an Freuquenzumrichter
            static public int maxCylinder                   { get; set; }       // Maximale Anzahl an Cylindern
            static public int maxSensorBool                 { get; set; }       // Maximale Anzhal an Sensoren die Angezeigt werden können
            static public int maxDeviceOnOff		        { get; set; }		// Maximale Anzahl an Geräte
            static public int maxEthercats                  { get; set; }       // Maximale Anzahl an Ethercatsbussen
            static public int maxProfibus                   { get; set; }       // Maximale Anzahl an Profibussen
            static public int maxProfinet                   { get; set; }       // Maximale Anzahl an Profinets
            static public int MaxParameter_Machine_BOOL     { get; set; }
            static public int MaxParameter_Machine_DINT     { get; set; }
            static public int MaxParameter_Machine_LREAL    { get; set; }
            static public int MaxParameter_Machine_STRING   { get; set; }
            static public int MaxParameter_Typ_BOOL         { get; set; }
            static public int MaxParameter_Typ_DINT         { get; set; }
            static public int MaxParameter_Typ_LREAL        { get; set; }
            static public int MaxParameter_Typ_STRING       { get; set; }
            static public int maxNetworkAdapter             { get; set; }
            static public int gc_maxCounter                 { get; set; }
        }


        // ***********************************************************
        // *************** Taster-Hilfsvariablen Maus/Touch **********
        // ***********************************************************
        /// Hilfsvariablen um den Tasterdruck sowohl durch Maus als auch durch Touch-Display durchführen zu können
        /// Problem 1:
        ///     Im Main-Fenster kann ein Taster immer sowohl durch Maus als auch durch Touch-Display gedrückt werden und die Maus-Events (Click/MouseDown/Preview-MouseDown/PreviewMouseUp) werden ausgeführt.
        ///     In neuen Fenstern (z.B. Num.Input-Fenster, Virtual-Keyboard, Login, Neuen Parametersatz erstellen, Neuen Auftrag erstellen, Parameter setzen usw.) werden diese Maus-Events folgenderweise ausgelöst:
        ///         - Die ersten 9 Touch-Inputs lösen kein Click/MouseDown usw. Events aus. Ab dem 10. Touch-Input werden diese Events jedoch ausgelöst.
        ///         - Die Touch-Events (TouchDown/TouchUp/TouchPreviewDown/TouchEnter) werden bereits ab dem 1. Touch-Inputs ausgelöst. 
        ///         - Wenn sowohl die Touch-Events als auch die Mouse-Events abonniert werden, werden bis zum 9. Eingabe nur die Maus-Events ausgelöst. 
        ///           Ab dem 10. Eingabe werden sowohl die Maus- als auch die Touch-Events ausgelöst....
        /// 
        /// Lösung:
        ///         - Es wird mitgezählt, wievielte Eingabe (Maus- oder Touch-"Click") erfolgt. 
        ///       
        /// Anmerkung:
        ///     Bei den meisten Fenstern (Login, Neuen Parametersatz erstellen, Neuen Auftrag erstellen, Parameter setzen usw) muss man nur 1x in den TextBlock klicken
        ///     und dann erscheint ein nächstes Fenster: Num.Input-Fenster oder VirtualKeyboard-Fenster.
        ///     Das heißt, das 10. Tasterdruck wird normalerweise nur bei diesen 2 Fenstern erreicht.
        ///         
        static public int iCountBtnPressedByTouch;

        static private int _iLimBtnPressedByTouch = 10;
        static public int iLimBtnPressedByTouch
        {
            get { return _iLimBtnPressedByTouch; }
            set
            {
                _iLimBtnPressedByTouch = value;
            }
        }

        /// <summary>
        /// Problem 2:
        ///     Wenn
        /// </summary>
        static public bool bInputTextBlockIsClickedMoreThanOnce { get; set; } // Hilfsvariable, um das Problem mit Touch/Mouseingabe überbrücken zu können. 

        static public bool bTouchEventFired { get; set; }
        // ***********************************************************
        // ************************* Order ***************************
        // ***********************************************************
        public class Orders
        {
            static private string _sCartonProPallet_ADSName = "GVL_TP.gTPdi_arr[2]"; // Init. Variable
            static public string sCartonProPallet_ADSName
            {
                get
                {
                    return _sCartonProPallet_ADSName;
                }
                set
                {
                    _sCartonProPallet_ADSName = value;
                }
            }
        }
    
        public static string sDateFormat = "dd.MM.yyyy HH:mm";




    }

    // ***********************************************************
    // ********************** Debug-Klassen **********************
    // ***********************************************************
    public class ADSDebug
    {
        /// <summary>
        /// true: die ADS-Kommunikation wird aufgebaut.   
        /// false: die ADS-Kommunikation wird nicht aufgebaut 
        ///             - dann kommen folgende ADS-Meldungen nicht: "Ads-Error 0x710 : Symbol could not be found"
        ///             - damit es funktioniert, sollte dieses Bit vor jedem Aufruf der WriteItem()-Methode geprüft werden
        /// </summary>
        public  bool bWantPLCConnect { get; set; }

        // Einzelne Connections
        public bool bWantConnectMainInit     { get; set; }
        public bool bWantConnectParam       { get; set; }
        public bool bWantConnectMsg         { get; set; }
        public bool bWantConnectLang        { get; set; }
    }


    public class DBDebug
    {
        public bool bWantUseParam { get; set; }
        public bool bWantUseUser { get; set; }
    }




}
