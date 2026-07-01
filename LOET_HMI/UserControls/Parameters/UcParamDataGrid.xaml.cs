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
using System.Globalization;
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;
using System.Windows.Threading;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;


namespace LOET_HMI
{
    public partial class UcParamDataGrid : UserControl
    {
        // Balog 6.2.2020: die folgenden Variablen werden nicht verwendet: 
        // List<db_paramset> ListParamSetsAll = new List<db_paramset>(); 

        // ***********************************************************************************************
        // ********************************** Parametersätze *********************************************
        // ***********************************************************************************************
        List<db_paramset> listDBParamSets = new List<db_paramset>();
        db_paramset SelectedParamSet = new db_paramset();

        public static DependencyProperty _ParameterSetType = DependencyProperty.Register("ParameterSetType", typeof(ParamSetTypes), typeof(UcParamDataGrid), new PropertyMetadata());
        public ParamSetTypes ParameterSetType
        {
            get { return (ParamSetTypes)GetValue(_ParameterSetType); }
            set { SetValue(_ParameterSetType, value); }
        }

        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // MBA 2.9.2020:   
        //      VORSICHT: die Kommentare zu folgenden Listen wurden vor mehreren Monaten erstellt. 
        //      Die passen wahrscheinlich nicht mehr zum aktuellen Stand
        //      Diese Beschreibungen müssen noch unbedingt aktualisiert werden
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        // *************************************************************************************************
        // ********************************** Parameter-Listen *********************************************
        // *************************************************************************************************
        // ************************************ 1: Aktuelle SPS-Werte **************************************
        /// <summary>
        /// - Typ der Komponenten:   StPLCSettingWithDB<T> (generischer Typ) 
        /// - Typ der Liste:         IStParamPLCDB -> Interface, damit auf die generischen Komponenten zugegriffen werden kann 
        /// - Beschreibung:          Diese Liste wird für die ADS-Kommunikation verwendet: die aktuellen SPS-Werte werden hier abgelegt und anschließend in listDatagrid geladen. 
        ///                          Jeder Eintrag enthält NUR den ADS-Namen und den PLCValue. Alle andere Felder bleiben bei jedem Eintrag dieser Liste Null 
        /// - ADS-Registrierung:     Im UserControl_Loaded EventHandler  
        /// </summary>
        //List<IStPLCSettingWithDB> listPLC = new List<IStPLCSettingWithDB>(); //KLAUS: unten wird  StPLCSettingWithDB zur Liste hinzugefügt....ANPASSEN, PRÜFEN WARUM!!!
        //List<StParamPLCDB<IStPLCSettingWithDB>> listAllPLCParam = new List<StParamPLCDB<IStPLCSettingWithDB>>(); //KLAUS: unten wird  StPLCSettingWithDB zur Liste hinzugefügt....ANPASSEN, PRÜFEN WARUM!!!
        List<IStParamPLCDB> listAllParam = new List<IStParamPLCDB>(); //KLAUS: unten wird  StPLCSettingWithDB zur Liste hinzugefügt....ANPASSEN, PRÜFEN WARUM!!!


        // *************************************************************************************************
        // ************************************ 1,5: Aktuelle SPS-Werte *************************************
        // MB 5.8.2020:
        List<IStParamPLCDB> listFilteredParam = new List<IStParamPLCDB>();


        // *************************************************************************************************
        // ************************************ 2: Geladen aus der Datenbank *******************************
        /// <summary>
        /// - In der Parameterset-Tabelle (links) wird ein Parameterset ausgewählt
        /// - Das SelectedCellChanged Event-Handler sucht in der Datenbank nach die Parameterliste, 
        ///   die zum ausgewählten Parameterset gehört (GetParamLisOverID) 
        /// - Erstellung der Klasse db_parameter: im MySQL Workbanch das Modell erstellen -> Datenbank generieren -> Modell zu VS hinzufügen
        ///   D.h., die Definition dieser Klasse wird in VS nicht angefasst!!!
        /// </summary>
        List<db_parameter> listDBParameter = new List<db_parameter>();

        // *************************************************************************************************
        // ************************************ 3: Im HMI anzeigen *****************************************
        /// <summary>
        /// - Typ der Komponenten:   StPLCSettingWithDB<T>  (generischer Typ)
        /// - Typ der Liste:         IStPLCSettingWithDB -> Interface, damit auf die generischen Komponenten zugegriffen werden kann
        /// - Beschreibung:          Diese Parameterliste wird direkt ins Datagrid geladen und im HMI angezeigt (dgParameter.ItemsSource = listDatagrid; )                     
        ///                          Die Liste listPLC wird auch in diese geladen (listDatagrid[i].SetPLCValue(listPLC[i].GetPLCValue()); )      
        /// - Aktualisierung:        Erfolgt in RefreshParameter()
        /// - DB-Einträge laden:     In einer Schleife wird der Typ jeder Komponente von listDBParameter abgefragt
        ///                          In jeder Wiederholung wird zu listDatagrid dem Typ entsprechend eine NEUE StPLCSettingWithDB<T>-Variable hinzugefügt 
        ///                          Für jede StPLCSettingWithDB<T>-Variable wird die RegisterDB()-Methode aufgerufen. 
        ///                          Diese kopiert jedes Feld der entsprechenden db_parameter-Variable einzeln in die StPLCSettingWithDB<T>-Variable: sMin, sMax, sValue, sHMIName(sSettingName), sUnit, iUserLevel
        /// - SPS-Werte laden:       Nachdem die DB-Einräge geladen sind, wird die RefreshPLCValues()-Methode aufgerufen und alle SPS-Werte in listDatagrid geladen.
        ///                          Dies erfolgt auch über eine for-Schleife, wobei jede i. Komponente der listPLC-Liste in die i. Komponente der listDatagrid-Liste kopiert wird.        ///                          
        /// </summary>
        /*
        public static readonly DependencyProperty _listDatagrid = DependencyProperty.Register(
            "listDatagrid", typeof(List<IStPLCSettingWithDB>), typeof(UcParamDataGrid), new PropertyMetadata(new List<IStPLCSettingWithDB>()));

        public List<IStPLCSettingWithDB> listDatagrid
        {
            get { return (List<IStPLCSettingWithDB>)GetValue(_listDatagrid); }
            set { SetValue(_listDatagrid, value); }
        }   
        */
        // *************************************************************************************************
        // *************************************************************************************************

        public CollectionViewSource itemCollectionViewSourceSet = new CollectionViewSource(); //KLAUS: werden diese verwendet??
        public CollectionViewSource itemCollectionViewSourceParameter = new CollectionViewSource();

        // *************************************************************************************************
        // **************************************** Filter *************************************************
        // *************************************************************************************************
        public static DependencyProperty _FilterByStation = DependencyProperty.Register("FilterByStation", typeof(StationIDs), typeof(UcParamDataGrid), new PropertyMetadata());
        public StationIDs FilterByStation
        {
            get { return (StationIDs)GetValue(_FilterByStation); }
            set { SetValue(_FilterByStation, value); }
        }

        public static DependencyProperty _FilterByModul = DependencyProperty.Register("FilterByModul", typeof(ModulIDs), typeof(UcParamDataGrid), new PropertyMetadata());
        public ModulIDs FilterByModul
        {
            get { return (ModulIDs)GetValue(_FilterByModul); }
            set { SetValue(_FilterByModul, value); }
        }


        public dynamic paramSelected { get; set; } //MBA 11.9.2020


        private bool bWinOpen = false;

        public UcParamDataGrid()
        {
            //listDatagrid = new List<IStPLCSettingWithDB>();
            InitializeComponent();


        }


        //***************************************************************************************************************
        //*******************************************  Loaded-Eventhandlers *********************************************
        //***************************************************************************************************************
        #region Loaded-Eventhandlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // ***** Taster 
            if (ParameterSetType == ParamSetTypes.Type)
            {
                //BtnLoadParamSetToPLC.IsEnabled = false;
                btnAddToOrder.Visibility = Visibility.Visible;
            }
            else if (ParameterSetType == ParamSetTypes.Machine)
            {
                //BtnLoadParamSetToPLC.IsEnabled = true;
                btnAddToOrder.Visibility = Visibility.Hidden;
            }


            // ***** ParamSet
            RefreshParamSets();

            // ***** PLC-Liste
            listAllParam = new List<IStParamPLCDB>();
            listFilteredParam = new List<IStParamPLCDB>();
            List<SettingCluster> tmpListAllPLCParam = new List<SettingCluster>();
            switch (ParameterSetType)
            {
                case ParamSetTypes.Type:
                    //listPLC = DBParam.Handler.SetTypeParameter(); // KLAUS
                    listAllParam = DBParam.Handler.SetTypeParameter();
                    break;

                case ParamSetTypes.Machine:
                    listAllParam = DBParam.Handler.SetMachineParameter();
                    break;
            }


            // ***** Filtern
            #region Info MB 5.8.2020: 
            //
            //Wenn die Filter-Methode hier normal aufgerufen wäre, 
            //wäre die gefilterte Liste null, weil die zu filternde Liste noch keine Werte hat.
            //Hierfür der Grund: ADS hat noch keine Werte von der SPS geholt.
            //
            //Mit DispatcherOperation funktioniert es. Die ersten ADS-Werte werden geholt 
            //und die Filter-Methode wird erst danach aufgerufen.
            //Keine Ahnung warum, aber es funktioniert.
            #endregion
            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() =>
            {
                while (DBParam.Handler.iCountFirstADSItemChanged < (listAllParam.Count))
                {
                    ;
                }

                if (ParameterSetType == ParamSetTypes.Machine)
                {
                    if (FilterByModul != ModulIDs.NoModul && FilterByStation == StationIDs.NoStation)
                    {
                        //listFilteredParam = DBParam.Handler.FilterParameter(listAllParam, FilterByModul);
                        MessageBox.Show("Das Station-ID wurde zu einer Seite nicht hinzugefügt.\nDie Parameter können daher nicht nach Station und Modul gefiltert werden.", // Text
                                        "Parameterseite", // Überschrift
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Asterisk);
                    }
                    else if (FilterByModul == ModulIDs.NoModul && FilterByStation != StationIDs.NoStation)
                    {
                        //listFilteredParam = DBParam.Handler.FilterParameter(listAllParam, FilterByStation);
                        MessageBox.Show("Das Modul-ID wurde zu einer Seite nicht hinzugefügt.\nDie Parameter können daher nicht nach Station und Modul gefiltert werden.", // Text
                                        "Parameterseite", // Überschrift
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Asterisk);
                    }
                    else if (FilterByModul != ModulIDs.NoModul && FilterByStation != StationIDs.NoStation)
                    {
                        listFilteredParam = DBParam.Handler.FilterMachineParameter(listAllParam, FilterByStation, FilterByModul);
                    }
                    else
                    {
                        listFilteredParam = listAllParam; //gefilterte Liste ist das gleiche wie die urpsrüngliche Liste
                    }
                }
                else if (ParameterSetType == ParamSetTypes.Type)
                {
                    listFilteredParam = DBParam.Handler.FilterTypeParameter(listAllParam);
                }

            }
            ), DispatcherPriority.Background, null); //DispatcherPriority.ContextIdle, null); 


            // ***** Text-Formattierung
            if (ParameterSetType == ParamSetTypes.Type)
                gbParam.Header = "Parameter";
            else if (ParameterSetType == ParamSetTypes.Machine)
                gbParam.Header = Properties.Resources.NavHoriz_btnParameters;

            /// ***** Code weiter:
            ///     - DGParamSet_Loaded
            ///     - 

        }

        public void DGParamSet_Loaded(object sender, RoutedEventArgs e) // Balog 16.12.2019: public gemacht für Transport Unit, Teach-Display
        {
            #region Info 1 zu Debugging (MB)
            // #############################
            // Mit folgendem EINZIGEN Befehl und dem Breakpoint danach hat es funktioniert. 
            // DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() => SelectRowOfParamSetLoadedToPLC()), DispatcherPriority.ContextIdle, null);
            // #############################
            #endregion
            if (GlobalVar.ActTypeParamSet != null)
            {
                System.Threading.Thread.Sleep(600); //
                DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SelectRowOfParamSetLoadedToPLC();
                    }
                ), DispatcherPriority.ContextIdle, null);
            }
            #region Info 2 zu Debugging (MB)
            // !!!!!!!!!!!!! HINWEIS 1:
            // Wenn nach dieser Zeile ein Breakpoint gesetzt wird, wird die jeweilige Zeile gefunden. 
            // Wenn der Breakpoint nicht da ist, wird die Zeile nicht gefunden und "Error" erscheint.
            // 

            // !!!!!!!!!!!!! HINWEIS 2:
            // Manchmal hat es mit dem Sleep-Befehl funktioniert, manchmal ohne....
            #endregion
        }


        public void SelectRowOfParamSetLoadedToPLC() //Balog 16.12.2019: public wegen Transport Unit, Teach-Display
        {
            int index = -1;
            int idToFind = 0;

            try
            {
                if (ParameterSetType == ParamSetTypes.Type)
                    idToFind = GlobalVar.ActTypeParamSet.id;
                else if (ParameterSetType == ParamSetTypes.Machine)
                    idToFind = GlobalVar.ActMachineParamSet.id;
            }
            catch (Exception ex)
            {
                AppLogger.Log("UcParamDataGrid.ResolveParamSetId", ex);
            }

            string message = "";
            MessageBoxImage image = MessageBoxImage.Information;

            for (int i = 0; i < dGParamSet.Items.Count; i++)
            {
                var tmp_item = dGParamSet.Items[i] as db_paramset;
                if (tmp_item.id == idToFind)
                {   // In der Datenbank sind Parametersäzte vorhanden - SPS-Param.satz wurde gefunden
                    index = i;
                    message = Properties.Resources.ParamMsgBox_InPLCFound + "\nName:\n\n"; //"The parameterset loaded to the PLC is found in the database.\nName:\n\n";            
                    image = MessageBoxImage.Information;
                    break;
                }
                else
                {   // In der Datenbank sind Parametersäzte vorhanden - SPS-Param.satz wurde NICHT gefunden
                    index = dGParamSet.Items.Count - 1;
                    message = Properties.Resources.ParamMsgBox_InPLCNotFound + "\n\n" + Properties.Resources.ParamMsgBox_Selected + "\n\n"; // "The parameterset loaded to the PLC is NOT found in the database.\n\nThe currently selected parameterset is:\n\n";
                    image = MessageBoxImage.Warning;
                }
            }
            dGParamSet.UnselectAll();   // Balog 16.22.2019
            dGParamSet.SelectedIndex = index;

            GlobalFunc.PopUp_SetMainWBackgrDark();
            try // In der Datenbank sind Parametersäzte vorhanden 
            {
                MessageBox.Show(message + SelectedParamSet.sName.ToString(),
                                    Properties.Resources.ParamMsgBox_InPLCHeader,
                                    MessageBoxButton.OK,
                                    image); // MessageBox.Show(message + ActParamSet.sName.ToString(), "Parameterset in PLC", MessageBoxButton.OK, image); 

            }

            catch // In der Datenbank sind KEINE Parametersäzte vorhanden 
            {
                MessageBox.Show(Properties.Resources.ParamMsgBox_NoParSetAtAll,
                                    Properties.Resources.ParamMsgBox_InPLCHeader,
                                    MessageBoxButton.OK,
                                    image); // MessageBox.Show("No parameterset was found in the database.", "Parameterset in PLC", MessageBoxButton.OK, image);  
            }

            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }
        #endregion

        //***************************************************************************************************************
        //******************************************  Refresh-Methoden **************************************************
        //***************************************************************************************************************
        #region Refresh-Methoden
        private void RefreshParamSets()
        {
            //listDBParamSets = new List<db_paramset>();
            listDBParamSets = DBParam.Handler.GetParmSetList(ParameterSetType);


            itemCollectionViewSourceSet.Source = null;
            itemCollectionViewSourceSet = (CollectionViewSource)(FindResource("itemCollectionViewSourceSet"));// siehe auch den XAML-Code
            itemCollectionViewSourceSet.Source = listDBParamSets;
        }

        public void RefreshParameter() // Balog 16.12.2019: public gemacht, damit es vom Teach-Display aus aufgerufen werden kann.
        {
            // INFO MB 4.8.2020: 
            // die Datenbank-Liste "listDBParameter" wird in DGParamSet_SelectedCellsChanged()-EventHandler aktualisiert

            if (SelectedParamSet != null)
            {

                // ***********************************************************************************************
                // ************************* Die Werte von der DB-Liste holen ************************************
                // *******************  und zu listAllParam hinzufügen *******************************
                // ***********************************************************************************************
                #region  
                for (int i = 0; i < listAllParam.Count; i++)
                {
                    // Zu jeder Komponente der "listAllParam"-Liste den dazugehörigen Eintrag in der "listDBParameter"-Liste finden:
                    db_parameter db_param_tmp = listDBParameter.Find(p => p.sADSName == listAllParam[i].GetADSNameProperty());

                    // ***** IN DB GEFUNDEN
                    if (db_param_tmp != null) // der gesuchte Eintrag wurde in der DB im akt. Param.Satz gefunden
                    {
                        listAllParam[i].SetDBProperties(db_param_tmp.sValue, db_param_tmp.iParamSetId); // DB-Wert und Parametersatz-ID zur Liste hinzufügen
                    }
                    // ***** IN DB NICHT GEFUNDEN
                    else // der gesuchte Eintrag wurde in der DB im akt. Param.Satz NICHT gefunden  -> Zur DB hinzufügen:
                    {
                        // ... aber zur DB nur dann hinzufügen, wenn der jeweilige Parameter im DataGrid nach der Filterung angezeigt werden soll.
                        bool bAddToDB = false; // ...hierfür diese Hilfsvariable verwendet

                        if (FilterByStation > StationIDs.NoStation && listAllParam[i].GetStationIDProperty() == (int)FilterByStation)
                            bAddToDB = true;
                        else if (FilterByModul > ModulIDs.NoModul && listAllParam[i].GetModulIDProperty() == (int)FilterByModul)
                            bAddToDB = true;
                        else if (FilterByModul == ModulIDs.NoModul && FilterByStation == StationIDs.NoStation)
                            bAddToDB = true;

                        // ***** ZUR DB HINZUFÜGEN
                        if (bAddToDB)
                        {
                            // Hierfür die nötigen Felder auslesen:
                            string tmp_ADSName = listAllParam[i].GetADSNameProperty();
                            string tmp_Name = listAllParam[i].GetNameProperty(); // nur für die nachfolgende Benachrichtigung relevant
                            dynamic tmp_ActPLCVal = listAllParam[i].GetPLCValueProperty();
                            string sTmp_ActPLCVal = (string)Convert.ChangeType(tmp_ActPLCVal, typeof(string), CultureInfo.InvariantCulture); // PLCValue soll für die DB in string umgewandelt werden

                            // Neuen Eintrag zum aktuellen Param.satz hinzufügen:
                            DBParam.Handler.AddNotFoundParamToParamSet(SelectedParamSet.id, tmp_ADSName, sTmp_ActPLCVal);

                            // Benachrichtigung:
                            MessageBox.Show("Der Parameter '" + tmp_Name + "' (" + tmp_ADSName + ") wird in der Datenbank im aktuellen Parametersatz '" + SelectedParamSet.sName + "' nicht gefunden.\n\n" +
                                            "Er wird zu '" + SelectedParamSet.sName + "' hinzugefügt.\n" +
                                            "Neuer Wert: '" + sTmp_ActPLCVal + "' (aktueller Wert in der SPS)",

                                            "Parameter zum aktuellen Parametersatz hinzugefügt", // Kopfzeile
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                            ;

                            // ***** DB-PROPERTIES IN DER LISTE INITIALISIEREN                           
                            // An dieser Stelle kann der DB-Wert noch nicht aus der Datenbank geholte werden. 
                            // Dafür sollte die ganze DB-Liste neu ausgelesen werden, damit der neu hinzugefügte Eintrag in der DB-Liste schon enthalten ist. 
                            // DESWEGEN:     "DB-Wert der Listenelemente"  = "SPS-Wert"
                            listAllParam[i].SetDBProperties(tmp_ActPLCVal, SelectedParamSet.id);
                        }


                    }
                }
                #endregion


                // MBA 19.8.2020: verschoben nach einem "}"
                //***********************           
                //dgParameter.ItemsSource = listAllParam;
                dgParameter.ItemsSource = listFilteredParam;
                dgParameter.CanUserAddRows = false; // Ohne diesen Befehl erscheint eine zusätzliche leere Zeile

                OnListDatagridUpdated();
            }
        }


        //**************************
        public delegate void SettingListUpdatedEventHandler();  // Quelle: https://stackoverflow.com/questions/14500559/create-custom-wpf-event
        public event SettingListUpdatedEventHandler ListDatagridUpdatedEvent;

        public void OnListDatagridUpdated()
        {
            // Your logic

            if (ListDatagridUpdatedEvent != null)
            {
                ListDatagridUpdatedEvent();
            }

        }
        //***********************************
        #endregion


        //***************************************************************************************************************
        //******************************  New, Copy, Rename, Delete, Load to PLC     ************************************
        //***************************************************************************************************************
        #region Button_Click: New, Copy, Rename, Delete, Edit all param., Load to PLC
        private void BtnNewParamSet_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();

                Window_NewParamSet window = new Window_NewParamSet(ParameterSetType);
                window.Closing += PopUpWindow_Closing;
                window.ShowDialog();

                RefreshParameter();

                if (window.DialogResult == true)
                {
                    dGParamSet.SelectedIndex = dGParamSet.Items.Count - 1; // Sonst ist das ParamSet der 1. Zeile ausgewählt und nicht das neu erstellte                    
                }

                GlobalFunc.PopUp_SetMainWBackgrNormal();
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


        private void BtnCopyParamSet_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();

                Window_CopyParamSet window = new Window_CopyParamSet(SelectedParamSet.sName, ParameterSetType);
                window.Closing += PopUpWindow_Closing;
                window.ShowDialog();

                if (window.DialogResult == true)
                    dGParamSet.SelectedIndex = dGParamSet.Items.Count - 1; // Ansonsten ist das ParamSet der 1. Zeile ausgewählt und nicht das neu erstellte

                GlobalFunc.PopUp_SetMainWBackgrNormal();
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

        private void BtnRenameParamSet_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();

                int ind = dGParamSet.SelectedIndex;

                Window_RenameParamSet window = new Window_RenameParamSet(SelectedParamSet.sName, ParameterSetType);
                window.Closing += PopUpWindow_Closing;
                window.ShowDialog();

                if (window.DialogResult == true)
                    dGParamSet.SelectedIndex = ind; // Ansonsten ist das ParamSet der 1. Zeile ausgewählt und nicht das neu erstellte

                GlobalFunc.PopUp_SetMainWBackgrNormal();
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

        private void BtnDeleteParamSet_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                bool bInOrderQueue = DBParam.Handler.ParamsetIsInOrderQueue(SelectedParamSet);

                if (bInOrderQueue) // kann nicht gelöscht werden
                {
                    MessageBox.Show("Der Parametersatz ist in der Auftragsliste enthalten. Daher kann es nicht gelöscht werden.", //Text 
                                    "Löschen nicht möglich", //Caption
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);

                }
                else if (SelectedParamSet.id == GlobalVar.ActTypeParamSet.id)
                {
                    MessageBox.Show("Der Parametersatz ist aktuell in die SPS geladen. Daher kann es nicht gelöscht werden.", //Text 
                                    "Löschen nicht möglich", //Caption
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
                else // kann gelöscht werden
                {
                    GlobalFunc.PopUp_SetMainWBackgrDark();

                    Window_DeleteParamSet window = new Window_DeleteParamSet(SelectedParamSet.sName, ParameterSetType);
                    window.Closing += PopUpWindow_Closing;
                    window.ShowDialog();

                    if (window.DialogResult == true)
                    {
                        //System.Threading.Thread.Sleep(500);
                        DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() => SelectRowOfParamSetLoadedToPLC()), DispatcherPriority.ContextIdle, null);
                    }

                    GlobalFunc.PopUp_SetMainWBackgrNormal();
                }
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

        // Public gemacht, damit es auch vom TransporUnit-Teach Display aufgerufen werden kann
        public void BtnLoadParamSetToPLC_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {

                if (ParameterSetType == ParamSetTypes.Type) // Ein Typparametersatz darf nur gesendet werden, aktuell kein Auftrag aktiv ist
                {
                    bool bCanBeLoaded = false;

                    switch (GlobalVar.ActOrderInPLC.iState)
                    {
                        case (int)eOrderArchivStates.OA_10_Sent:
                            bCanBeLoaded = false;
                            break;
                        case (int)eOrderArchivStates.OA_20_Started:
                            bCanBeLoaded = false;
                            break;
                        case (int)eOrderArchivStates.OA_30_Finished:
                            bCanBeLoaded = true;
                            break;
                        case (int)eOrderArchivStates.OA_90_Cancelled:
                            bCanBeLoaded = true;
                            break;
                    }

                    if (bCanBeLoaded)
                    {
                        if (GlobalVar.ActOrderInPLC.bActiveInPLC)
                        {
                            using (CHP_HMIEntities context = new CHP_HMIEntities())
                            {
                                try
                                {
                                    List<db_orderarchiv> listOrderArchiv = context.db_orderarchiv.ToList();
                                    db_orderarchiv orderInPLC = listOrderArchiv[listOrderArchiv.Count - 1];
                                    orderInPLC.bActiveInPLC = false;
                                    context.SaveChanges();

                                    GlobalVar.UpdateActOrderInPLC();
                                    GlobalVar.UpdatePrevOrderInPLC();
                                }
                                catch
                                {
                                    MessageBox.Show("Der Typparametersatz wurde in die SPS geladen.\n\n" +
                                                    "Jedoch konnte die Statusvariable 'ActiveInPLC' des bisher aktiven Auftrages in der Datenbank nicht auf 'false' gesetzt werden.",//Text
                                                    "Aktualisierung der Datenbank fehlgeschlagen",//Caption,
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                                }

                            }
                        }
                        LoadParamSetToPLC();
                    }
                    else
                    {
                        MessageBox.Show("Kein Parametersatz darf aktuell in die SPS geladen werden. Ein Auftrag wird bearbeitet.",//Text
                                        "Laden nicht erlaubt",//Caption,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                    }

                }
                else if (ParameterSetType == ParamSetTypes.Machine)
                {
                    LoadParamSetToPLC();
                }

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

        public void LoadParamSetToPLC()
        {

            DBParam.Handler.LoadParamSetToPLC(SelectedParamSet.id, ParameterSetType);

            RefreshParameter();
            RefreshParamSets();

            //System.Threading.Thread.Sleep(500); 
            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() => SelectRowOfParamSetLoadedToPLC()), DispatcherPriority.ContextIdle, null);
        }

        private void BtnSelectCurrentPLCRecipe_Click(object sender, RoutedEventArgs e)
        {
            SelectRowOfParamSetLoadedToPLC();
        }
        #endregion



        //***************************************************************************************************************
        //************************************** Tabelle LINKS: ParameterSET  *******************************************
        //***************************************************************************************************************
        #region dataGrid ParameterSet      
        // ParameterSET
        private void DGParamSet_GotFocus(object sender, RoutedEventArgs e)
        {
            dGParamSet.SelectedItem = null;
        }

        //  ParameterSET
        private void DGParamSet_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (dGParamSet.SelectedItem != null)
            {

                int index = dGParamSet.SelectedIndex;
                var row = dGParamSet.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;


                try
                {
                    db_paramset TmpParamSet = dGParamSet.SelectedItem as db_paramset;
                    SelectedParamSet = DBParam.Handler.GetParamSetOverId(TmpParamSet.id);
                    listDBParameter = DBParam.Handler.GetParamListOverId(TmpParamSet.id);
                }
                catch (Exception ex)
                {
                    AppLogger.Log("UcParamDataGrid.ParamSetSelectionChanged", ex);
                }

                RefreshParameter();
            }
        }

        private void DGParamSet_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                BtnDeleteParamSet_Click(sender, e);
        }
        #endregion


        //***************************************************************************************************************
        //*************************************** Tabelle RECHTS: Parameter  ********************************************
        //***************************************************************************************************************
        #region dataGrid Parameter

        private Thread newWindowThread;

        //  Parameter
        private void DgParameter_GotFocus(object sender, RoutedEventArgs e)
        {
            dgParameter.SelectedItem = null;
        }

        // Parameter
        private void DgParameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (dgParameter.SelectedItem != null && !bWinOpen)
            {
                e.Handled = true;
                int indTmp_ParamSet = dGParamSet.SelectedIndex; // Index der ParameterSet-Tabelle Zwischenspeichern.


                // ***********************************************************************
                // ************** Hintergrundfarbe der geklickten Zeile ******************
                // ***********************************************************************
                // Farbe der geklickte Zeile manuell ändern "highlihted" machen, während das Dialog-Fenster geöffnet ist. 
                // Sonst wäre die ausgewählte Zeile erst "highlighted", nachdem das DialogFenster geschlossen ist.
                int index = dgParameter.SelectedIndex;
                var row = dgParameter.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                row.Background = Brushes.SkyBlue;
                // MBA 22.9.2020: AKTUELL FUNKTIONIERT ES GAR NICHT!!!
                // ***********************************************************************


                // ************************************************************************
                // ********************* Typ zurückgewinnen *******************************
                // ***********************************************************************
                // Den Typ des geklickten Eintrags zurückgewinnen (die Liste im DataGrid ist generisch, wo die Einträge unterschiedliche Typen haben dürfen)
                Type _type = GlobalFunc.FindTypeOfGenericSettingItem(dgParameter.SelectedItem);
                dynamic selectedItemCasted = Convert.ChangeType(dgParameter.SelectedItem, _type); // "dynamic", weil die Felder sSettingName, ValDB, ValPLC usw. sonst nicht ansprechbar sind. Als "var" oder "object" auch nicht.
                                                                                                  // ************************************************************************

                // ***********************************************************************
                // ****************** Hintergrundfarbe dunkkel machen ********************
                // ***********************************************************************
                /*
                DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        GlobalFunc.PopUp_SetMainWBackgrDark();
                    }
                    ), DispatcherPriority.Send, null);
                

                GlobalFunc.PopUp_SetMainWBackgrDark();
                */


                // ***********************************************************************
                // *********************** Eingabefenster öffnen *************************
                // ***********************************************************************
                /*
                GlobalFunc.PopUp_SetMainWBackgrDark();

                WindowPopUpSetValue window_setting;
                window_setting = new WindowPopUpSetValue(selectedItemCasted);
                e.Handled = true;
                window_setting.ShowDialog();

                GlobalFunc.PopUp_SetMainWBackgrNormal();
                */

                //***************
                WindowCollection col = Application.Current.Windows;
                System.Windows.Window window = new System.Windows.Window();
                foreach (System.Windows.Window win in col)
                {
                    if (win.Name == "MainW")
                    {
                        window = win;
                    }
                }
                var main = window as MainWindow;
                main.IsEnabled = false;
                GlobalFunc.PopUp_SetMainWBackgrDark();
                // Quelle: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/threading-model?view=netframeworkdesktop-4.8
                // MBA: 11.9.2020:
                paramSelected = selectedItemCasted;
                newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.Priority = ThreadPriority.Highest; // MBA 30.11.2020
                newWindowThread.SetApartmentState(ApartmentState.STA); // Singlethreaded Apartment
                //newWindowThread.IsBackground = true;
                newWindowThread.Start(); // Thread starten
                newWindowThread.Join();  // Warten auf Thread ("This method blocks the calling thread until the thread represented by this instance terminates while continuing to perform standard COM and SendMessage pumping." Quelle: https://www.geeksforgeeks.org/joining-threads-in-c-sharp/#:~:text=In%20C%23%2C%20Thread%20class%20provides,it%20joins%20completes%20its%20execution. )

                GlobalFunc.PopUp_SetMainWBackgrNormal();
                main.IsEnabled = true;
                //************************************************************************


                // ************************************************************************
                // *************** Index von ParamSet-Datagrid zurücksetzen ***************
                // ************************************************************************
                // Wegen der Aktualisierung der Listen bleibt die angeklickte Zeile der ParamSet-Tabelle normalerweise nicht ausgewählt
                // Anhand des zwischengespeicherten Indexes wird diese Zeile hier zurückgestellt
                // System.Threading.Thread.Sleep(600); // Wenn der Param.Set aktiv ist, wird der geänderte Parameter in die SPS-geladen -> dann von der SPS ausgelesen -> warten, da es über ADS länger dauert..
                DispatcherOperation dispOperation2 = Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        dGParamSet.UnselectAll();
                        dGParamSet.SelectedIndex = indTmp_ParamSet; // Nachdem der neue "Value PLC" geladen ist, den aktuellen Parametersatz auswählen -> Dann wird "Value PLC" der geänderte Zeile aktualisiert

                        GlobalFunc.PopUp_SetMainWBackgrNormal();
                    }
                    ), DispatcherPriority.ContextIdle, null);

                //************************************************************************

                row.ClearValue(DataGridCell.BackgroundProperty);

                // MBA 22.9.2020:
                GlobalVar.dataLOET.Act_Station.ClusterList1stRow[0].ButtonOfDispl.Focus(); // Focus auf einen anderen Control setzen, damit der ParamGrid bei wiederholtem Anklicken der gleichen Zeile das "GotFocus" Event auslöst.
                dgParameter.SelectedIndex = index;
                //RefreshParameter();
                //dGParamSet.UnselectAll();
                //dGParamSet.SelectedIndex = indTmp_ParamSet; // Nachdem der neue "Value PLC" geladen ist, den aktuellen Parametersatz auswählen -> Dann wird "Value PLC" der geänderte Zeile aktualisiert
            }
            else
            {
                e.Handled = true;
                return;
            }
        }

        private void ThreadStartingPoint()
        {
            try
            {
                if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
                {
                    //GlobalFunc.PopUp_SetMainWBackgrDark();
                    bWinOpen = true;

                    WindowPopUpSetValue window_setting;
                    window_setting = new WindowPopUpSetValue(paramSelected);
                    window_setting.Topmost = true;
                    window_setting.Closed += OnWindowSettingClosed;
                    window_setting.ShowDialog();
                    
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
            catch (ThreadAbortException)
            {
                // Ladethread wurde bewusst abgebrochen -> erwartetes Verhalten, kein Logging noetig
            }
            //GlobalFunc.PopUp_SetMainWBackgrNormal();

            // MBA 22.9.2020 ???????????????????????????? Ein ode auskommentiert?????
            //System.Windows.Threading.Dispatcher.Run();
        }

        private void OnWindowSettingClosed(object sender, EventArgs e)
        {
            bWinOpen = false;
            bool test = newWindowThread.IsAlive;
        }

        #endregion












        private void PopUpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RefreshParamSets();
            RefreshParameter();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Deregister();
        }

        public void Deregister()
        {
            for (int i = 0; i < listAllParam.Count; i++)
            {
                listAllParam[i].Deregister();
            }

            listAllParam.Clear(); // Balog 6.2.2020

        }

        private void BtnEditAllParam_Click(object sender, RoutedEventArgs e)
        {
            //EditAllParameters(dGParamSet.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddToOrder_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                db_paramset TmpParamSet = dGParamSet.SelectedItem as db_paramset;


                //***************************************************************************
                //GlobalFunc.PopUp_SetMainWBackgrDark();
                //WindowPopUpNewOrder winNewOrder = new WindowPopUpNewOrder(TmpParamSet.id);
                //winNewOrder.ShowDialog();
                //
                //GlobalFunc.PopUp_SetMainWBackgrNormal();


                //***************
                DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        GlobalFunc.PopUp_SetMainWBackgrDark();

                        WindowPopUpNewOrder winNewOrder = new WindowPopUpNewOrder(TmpParamSet.id);
                        winNewOrder.ShowDialog();

                        GlobalFunc.PopUp_SetMainWBackgrNormal();
                    }
                    ), DispatcherPriority.ContextIdle, null);
                //************************************************************************


            }
            catch
            {
                MessageBox.Show("Fehler");
                GlobalFunc.PopUp_SetMainWBackgrNormal();
            }
        }




        //***************************************************************************************************************
        //************************************************ CSV-Export ***************************************************
        //***************************************************************************************************************
        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            string date = DateTime.Now.ToString("yyyy.MM.dd HH-mm"); //Mit Sekunde: yyyy.MM.dd HH_mm_ss"

            // Save File Dialog
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = "C:\\CHP\\DB Export\\";
            dialog.FileName = date + ' ' + SelectedParamSet.sName; // Default file name
            dialog.DefaultExt = ".csv"; // Default file extension
                                        //dialog.Filter         = "Text documents (.txt)|*.txt"; // Filter files by extension


            // Show save file dialog box
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Nullable<bool> result = dialog.ShowDialog();

            // Process save file dialog box results
            bool bCanWrite = false;
            if (result == true)
            {
                if (File.Exists(dialog.FileName) == true)
                {
                    try
                    {
                        File.Delete(dialog.FileName);
                        bCanWrite = true;
                    }
                    catch
                    {
                        MessageBox.Show("File is open. Please close it and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        bCanWrite = false;
                    }
                }
                else
                    bCanWrite = true;


                if (bCanWrite)
                {
                    for (int i = 0; i < listFilteredParam.Count; i++)
                    {

                        // Parameter
                        Type _type = GlobalFunc.FindTypeOfGenericSettingItem(listFilteredParam[i]);
                        dynamic tmp = Convert.ChangeType(listFilteredParam[i], _type); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar sind. Als Typ "var" oder "object" auch nicht.

                        // Konvertierung in String durch Zwischenvariablen durchführen. Somit bleibt das Dezimalzeichen immer Punkt (unnabhängig von der HMI-Sprache. Wenn die HMI-Sprache Deutsch ist, wäre das Dezimalzeichen eine Komma)
                        string sTmpValDB = (string)Convert.ChangeType(tmp.ValDB, typeof(string), CultureInfo.InvariantCulture);
                        string sTmpValPLC = (string)Convert.ChangeType(tmp.Val, typeof(string), CultureInfo.InvariantCulture); //KLAUS: prüfen, ob es Val oder ValPLC ist
                        string sTmpMin = (string)Convert.ChangeType(tmp.Min, typeof(string), CultureInfo.InvariantCulture);
                        string sTmpMax = (string)Convert.ChangeType(tmp.Max, typeof(string), CultureInfo.InvariantCulture);

                        // CSV-Operationen
                        // https://www.youtube.com/watch?v=gqDFD3TIrHY
                        StringBuilder csvcontent = new StringBuilder();



                        if (i == 0) // Kopfzeile:
                        {

                            csvcontent.AppendLine("Name" + ';' + "Value DB" + ';' + "Value PLC" + ';' + "Unit" + ';' + "Min" + ';' + "Max");
                        }
                        else // Daten:
                        {
                            csvcontent.AppendLine(tmp.strName + ';' +
                                                    sTmpValDB + ';' +
                                                    sTmpValPLC + ';' +
                                                    tmp.strUnit + ';' +
                                                    sTmpMin + ';' +
                                                    sTmpMax);
                        }

                        File.AppendAllText(dialog.FileName, csvcontent.ToString());
                    }
                }



            }
            GlobalFunc.PopUp_SetMainWBackgrNormal();
        }


    }


}
