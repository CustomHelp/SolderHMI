using LOET_HMI.PLC_Com_Classes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;



namespace LOET_HMI
{

    public class ProjectData : INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // *******************************************************************************
        // ************************ Deklaration: Vertikale Navigation  *******************
        // *******************************************************************************
        private StationVertNav _Act_Station;
        public StationVertNav Act_Station
        {
            get { return _Act_Station; }
            set { _Act_Station = value; }
        }

        // Stationen

        // ***************************************************************
        // *********************** CHP-Displays 1 ************************
        // ***************************************************************
        public StationVertNav Maschine                     { get; set; }
        public StationVertNav Handmenu                     { get; set; } // Handmenü
        public StationVertNav Auftrag                      { get; set; }
        public StationVertNav Krosy                        { get; set; }

        // ***************************************************************
        // ********************** Projekt-Displays ***********************
        // ***************************************************************
        public StationVertNav Station1                  { get; set; }
        public StationVertNav Station2                  { get; set; }
        public StationVertNav Global                    { get; set; }
        // ***************************************************************
        // *********************** CHP-Displays 2 ************************
        // ***************************************************************
        public StationVertNav SpecialFunctions             { get; set; }
        //public StationVertNav Zaehler                      { get; set; }
        public StationVertNav Einstellungen                { get; set; }

        // Liste:
        public List<StationVertNav> listStatVertNav;               // Liste der Stationen für die Navigation

        // *******************************************************************************
        // *********************** Deklaration:  StationID Meldungen *******************
        // *******************************************************************************
        // Liste:
        public List<StationMsg> listStatMsgModul1;
        public List<StationMsg> listStatMsgModul2;
        public List<StationMsg> listStatMsgModul3;

        // *******************************************************************************
        // *************************** Deklaration: Sonstiges ****************************
        // *******************************************************************************
        public DisplayDimensions TouchDisplay { get; set; }


        public ProjectData()
        {
            // *******************************************************************************
            // ***************** Vertikale Navigation: Einträge einzeln erstellen ************
            // *******************************************************************************
            // Allgemein vorne
            Maschine                        = new StationVertNav(0, Properties.Resources.NavVert_btnMachine,            eStationsHMI._NoID);
            Handmenu                        = new StationVertNav(0, Properties.Resources.NavVert_btnManualMenu,         eStationsHMI._NoID);// Properties.Resources.
            Auftrag                         = new StationVertNav(0, Properties.Resources.NavVert_btnOrder,              eStationsHMI._NoID);
            Krosy                           = new StationVertNav(0, Properties.Resources.NavVert_btnKROSY, eStationsHMI._NoID);
            // Modul 1:
            Station1                        = new StationVertNav(1, Properties.Resources.Proj_StNameStation1,           eStationsHMI.M1_01_Station1);
            Station2                        = new StationVertNav(1, Properties.Resources.Proj_StNameStation2,           eStationsHMI.M1_02_Station2);
            Global                          = new StationVertNav(1, Properties.Resources.NavVert_btnGlobal,             eStationsHMI.M1_03_Global);                   
            // Allgemein unten
            SpecialFunctions                = new StationVertNav(99, Properties.Resources.NavVert_btnSpecialFunctions,  eStationsHMI._NoID);
            //Zaehler                       = new StationVertNav(99, Properties.Resources.NavVert_btnCounter,           eStationsHMI._NoID);
            Einstellungen                   = new StationVertNav(99, Properties.Resources.NavVert_btnSettings,          eStationsHMI._NoID);

            // *******************************************************************************
            // **************** Vertikale Navigation: Einträge zur Liste hinzufügen **********
            // *******************************************************************************
            // Liste
            listStatVertNav = new List<StationVertNav>();

            listStatVertNav.Add(Maschine);
            //listStatVertNav.Add(Handmenu);
            //listStatVertNav.Add(Auftrag);
            listStatVertNav.Add(Krosy);

            // Modul 1:   
            listStatVertNav.Add(Station1);
            listStatVertNav.Add(Station2);
            listStatVertNav.Add(Global);

            //listStatVertNav.Add(SpecialFunctions);       
            //listStatVertNav.Add(Zaehler);
            listStatVertNav.Add(Einstellungen);

            // *******************************************************************************
            // **************** StationID Meldungen: Einträge zur Liste hinzufügen ***********
            // *******************************************************************************
            listStatMsgModul1 = new List<StationMsg>();
            listStatMsgModul2 = new List<StationMsg>(); // bei der Lötanlage nicht verwendet
            listStatMsgModul3 = new List<StationMsg>(); // bei der Lötanlage nicht verwendet

            // Modul 1 
            listStatMsgModul1.Add(new StationMsg(_sStationNameDB: "Modul 1",         _sStationNameTransl: Properties.Resources.Proj_StNameModul1,   _iGVLid:  0)); 
            listStatMsgModul1.Add(new StationMsg(_sStationNameDB: "Station 1 left",  _sStationNameTransl: Properties.Resources.Proj_StNameStation1, _iGVLid:  1));
            listStatMsgModul1.Add(new StationMsg(_sStationNameDB: "Station 2 right", _sStationNameTransl: Properties.Resources.Proj_StNameStation2, _iGVLid:  2));


            // *******************************************************************************
            // ********************************** Sonstiges **********************************
            // *******************************************************************************
            TouchDisplay = new DisplayDimensions();
        }
    }

    // *******************************************************************************
    // ************************ Klassen:  Stationen - Meldungen **********************
    // *******************************************************************************
    public class StationVertNav
    {
        public int iModulNr { get; set; }
        public eStationsHMI eStationID { get; set; }

        //public string sBtnText; // Name der Station
        public Button btnVertNav { get; set; }

        public List<NavCluster> ClusterList1stRow;   
        

        public StationVertNav(int _iModulNr, string _sBtnText, eStationsHMI _eStationID)
        {
            iModulNr            = _iModulNr;
            eStationID          = _eStationID;
            //sBtnText            = _sBtnText;
            btnVertNav          = new Button();
            btnVertNav.Content = _sBtnText;
            ClusterList1stRow   = new List<NavCluster>(); // UcNavWorksp verwendet diese Liste            
        }

        /// <summary>
        /// Dieser Konstruktor wird lediglich für Act_Modul verwendet. 
        /// Da dies immer auf eins von den anderen Modulen zeigt (wie ein Pointer), 
        /// müssen die Struktur-Felder von Modul nicht erstellt werden.  
        /// Wenn z.B. Act_Modul auf Modul1 zeigt, und z.B. der Befehl "rena_HMI.Act_Modul.listDatagrid"
        /// wird "rena_HMI.Modul1.listDatagrid" angesprochen.
        /// </summary>
        public StationVertNav() 
        {
            //ClusterList = new List<NavCluster>(); // UcNavWorksp verwendet diese Liste
            //SettingsToHMIlist = new List<SettingToHMIclass>();
            //listDatagrid = new List<IStPLCSettingWithDB>();
        }
        
    }

    public class NavCluster // Navigation Cluster = Display + dazugehöriges Button
    {
        //1. Zeile:
        public Button ButtonOfDispl { get; set; }
        public Page   Display{ get; set; }

        //2. Zeile:
        public List<NavCluster> ClusterNextRow { get; set; }

        public NavCluster(string btnname, Page page)
        {
            ButtonOfDispl = new Button();
            ButtonOfDispl.Content = btnname;
            Display = page;

            ClusterNextRow = new List<NavCluster>();
        }
        public NavCluster()
        { }
    }

    // *******************************************************************************
    // ************************** Klassen: Stationen - Meldungen *********************
    // *******************************************************************************
    /// <summary>
    /// Die Stations-ID (Globale Konstante der GVL-Liste im SPS-Projekt) und den Stationsnamen zusammen ablegen.
    /// Diese werden dann bei den Meldungen verwendet.
    /// </summary>
    public class StationMsg
    {
        public string sStationNameDB { get; set; } // dieser Name wird in die Datenbank geschrieben
        public string sStationNameTransl { get; set; } // dieser Name wird übersetzt (wenn die Translate-Methode in UcMessage aufgerufen wird) und im HMI angezeigt
        public int iGVL_ID { get; set; } // z.B. gc 1, 2, 3,....

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_sStationNameDB">Diese Variable wird NICHT überesetzt und wird in die Station-Spalte der Datenbenk (db_message) der jeweiligen Meldung geschreiben.</param>
        /// <param name="_sStationNameTransl">Diese Variable wird übersetzt. Sie wird für die Anzeige der jeweiligen Meldung im HMI verwendet</param>
        /// <param name="_iGVLid"></param>
        public StationMsg(string _sStationNameDB, string _sStationNameTransl, int _iGVLid)
        {
            sStationNameDB     = _sStationNameDB;
            sStationNameTransl = _sStationNameTransl;
            iGVL_ID            = _iGVLid;
        }
    }







    // *******************************************************************************
    // ****************************** Klassen: Sonstiges *****************************
    // *******************************************************************************
    static public class DisplayDimensionsStatic
    {
        static private double _width_mm = 476.64;
        static public double width_mm { get { return _width_mm; } }

        static private double _height_mm = 268.11;
        static public double height_mm { get { return _height_mm; } }

        static public double width_pix { get { return 9.6 / 2.54 * _width_mm; } }
        static public double height_pix { get { return 9.6 / 2.54 * _height_mm; } }

        static DisplayDimensionsStatic()
        {
        }
    }


    public class DisplayDimensions
    {
        private double _win_width_mm = 476.64;
        public double win_width_mm { get { return _win_width_mm; } }

        private double _win_height_mm = 268.11;
        public double win_height_mm { get { return _win_height_mm; } }

        //public double width_pix { get { return 9.6 / 2.54 * _width_mm; } }
        //public double height_pix { get { return 9.6 / 2.54 * _height_mm; } }

        private double _win_width_pix = 1920;
        public double win_width_pix { get { return _win_width_pix; } }
        private double _win_height_pix = 1080;
        public double win_height_pix { get { return _win_height_pix; } }

        private double _frame_width_pix = 200;
        public double frame_width_pix { get { return _frame_width_pix; } }
        private double _frame_height_pix = 200;
        public double frame_height_pix { get { return _frame_height_pix; } }

        public DisplayDimensions()
        {
        }
    }



    
    public class UserLevelsProject
    {
        public class UserLevel
        {
            public string sName { get; set; }
            public int iLevel { get; set; }

            public UserLevel(string _sName, int _iLevel)
            {
                sName = _sName;
                iLevel = _iLevel;
            }
        }


        public readonly UserLevel Default;
        public readonly UserLevel Operator;
        public readonly UserLevel Service;

        public readonly UserLevel Maintenance;
        public readonly UserLevel Supervisor;
        public readonly UserLevel Administrator;
        public readonly UserLevel Quality;

        public readonly List<UserLevel> list;

        public readonly UserlevelMessages Msg;

        public UserLevelsProject()
        {
            // ***** Userlevels erstellen
            Default = new UserLevel(Properties.Resources.UserLevel_DefaultUser, 0);
            Operator = new UserLevel(Properties.Resources.UserLevel_Operator, 10);
            Service = new UserLevel(Properties.Resources.UserLevel_Service, 30);

            Maintenance     = new UserLevel(Properties.Resources.UserLevel_Maintenance,      40);
            Quality = new UserLevel(Properties.Resources.UserLevel_Quality, 50);
            Supervisor      = new UserLevel(Properties.Resources.UserLevel_Supervisor ,      60);
            Administrator   = new UserLevel(Properties.Resources.UserLevel_Administrator,    99);

            // ***** Userlevels auch zur Liste hinzufügen
            list = new List<UserLevel>();

            list.Add(Default);
            list.Add(Operator);
            //list.Add(Service);
            list.Add(Quality);
            list.Add(Maintenance);
            list.Add(Supervisor);
            list.Add(Administrator);

            // ***** Nachrichten für Mesaggebox erstellen
            Msg = new UserlevelMessages();
        }


    }




    public class UserlevelMessages
    {
        public string sAccesDeniedText      = Properties.Resources.MsgBoxUserLev_AccesDeniedText;       //"You don't have permission to perform this operation";
        public string sAccesDeniedCaption   = Properties.Resources.MsgBoxUserLev_AccesDeniedCaption;    //"Acces denied";

        public string sNoSelectionText      = Properties.Resources.MsgBoxUserLev_NoSelectionText;       //"No entry is selected."; // "Kein Datensatz ausgewählt"
        public string sNoSelectionCaption   = Properties.Resources.MsgBoxUserLev_NoSelectionCaption;    //"Selection";
    }
}

