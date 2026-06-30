using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LOET_HMI.PLC_Com_Classes;
using System.Windows.Threading;
using System.Threading;



namespace LOET_HMI
{
    static public class DBParam
    {
        static private DBParamHandler _DBParamHandler;
        static public DBParamHandler Handler
        {
            get { return _DBParamHandler; }
        }

        static DBParam()
        {
            _DBParamHandler = new DBParamHandler();
        }
    }

    public class DBParamHandler
    {
        private int _iCountFirstADSItemChanged;
        public int iCountFirstADSItemChanged
        {
            get
            {
                return _iCountFirstADSItemChanged;
            }

            set
            {
                _iCountFirstADSItemChanged = value;
                //OnPropertyChanged();
            }
        }


        public List<IStParamPLCDB> listPar { get; set; }


        public List<IStParamPLCDB> SetTypeParameter() //KLAUS: in db_parameter sollte später nurch noch der ADS-Name stehen -> NewRecipeParameter anpassen
        {
            // ***** Temporäre Cluster-Liste erstellen (Cluster = SPS-Struktur + ADS-Name) -> nur für die Benutzerfreundlichkeit: Strukturtyp + ADS-Name sind direkt nebeneinander anzugeben
            List<SettingCluster> listTmpCluster = new List<SettingCluster>();

            if (GlobalVar.debugADS.bWantConnectParam)
            {

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Typ_BOOL; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<bool>(), "GVL_TP.gTPb_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Typ_DINT; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<Int32>(), "GVL_TP.gTPdi_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Typ_LREAL; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<double>(), "GVL_TP.gTPlr_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Typ_STRING; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<string>(), "GVL_TP.gTPstr_arr[" + i.ToString() + "]"));
            }

            CreatListFromCluster(listTmpCluster);
            RegisterList(listTmpCluster);

            return listPar;
        }

        public List<IStParamPLCDB> SetMachineParameter() // MB 3.8.2020:
        {
            // ***** Temporäre Cluster-Liste erstellen (Cluster = SPS-Struktur + ADS-Name) -> nur für die Benutzerfreundlichkeit: Strukturtyp + ADS-Name sind direkt nebeneinander anzugeben
            List<SettingCluster> listTmpCluster = new List<SettingCluster>();

            if (GlobalVar.debugADS.bWantConnectParam)
            {
                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Machine_BOOL; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<bool>(), "GVL_MP.gMPb_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Machine_DINT; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<Int32>(), "GVL_MP.gMPdi_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Machine_LREAL; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<double>(), "GVL_MP.gMPlr_arr[" + i.ToString() + "]"));

                for (int i = 1; i <= GlobalVar.GVL_Limits.MaxParameter_Machine_STRING; i++)
                    listTmpCluster.Add(new SettingCluster(new StParamPLCDB<string>(), "GVL_MP.gMPstr_arr[" + i.ToString() + "]"));
            }

            CreatListFromCluster(listTmpCluster);
            RegisterList(listTmpCluster);

            return listPar;
        }


        public void CreatListFromCluster(List<SettingCluster> _listTmpCluster)
        {
            listPar = new List<IStParamPLCDB>();

            for (int i = 0; i < _listTmpCluster.Count; i++)
            {
                Type _typeOfGeneric = GlobalFunc.FindTypeOfGenericSettingItem(_listTmpCluster[i].SettingStruct);
                dynamic selectedItemCasted = Convert.ChangeType(_listTmpCluster[i].SettingStruct, _typeOfGeneric); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar. Als var oder object auch nicht.

                listPar.Add(selectedItemCasted);
                listPar[i].SetADSNameProperty(_listTmpCluster[i].RegistrationName);   // ADS-Name gleich im Parameter als Property ablegen             
            }

        }
        public void RegisterList(List<SettingCluster> _listTmpCluster)
        {
            // ***** Register Parameter-Liste
            for (int i = 0; i < _listTmpCluster.Count; i++)
            {
                listPar[i].Register(listPar[i].GetADSNameProperty());
            }
        }




        #region "WaitForReceiveADSData" Hat nicht funktioniert
        /*
        public void WaitForReceiveADSData(List<IStParamPLCDB> listToReg)
        {
            bool bItemFirstChanged;
            for (int i=0; i<listToReg.Count; i++)
            {
                bItemFirstChanged = false;
                while (true) // Endlosschleife
                {
                    bItemFirstChanged = listToReg[i].GetItemFirstChanged();

                    
                    //DispatcherOperation dispOperation = Dispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    bItemFirstChanged = listToReg[i].GetItemFirstChanged(); 
                    //}
                    //), DispatcherPriority.ContextIdle, null);
                    

                    if (bItemFirstChanged)
                    {
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    
                }
            }
        }
        */
        #endregion

        #region Asyncrhon, nicht verwednet
        /*
        //Quelle: https://www.youtube.com/watch?v=2moh18sh5p4
        public async Task WaitForReceiveADSDataAsync(List<IStParamPLCDB> listToReg) 
        // 15:30, Return Type: 
        //      - Don't return void for an async-Method. 
        //      - if you don't have anything to return, just say: Task
        //      - If you have something to return, e.g. a string: Task<stringA 
        {

            await Task.Run(()=>            
            {
                bool bItemFirstChanged;
                for (int i = 0; i < listToReg.Count; i++)
                {
                    bItemFirstChanged = false;
                    while (true) // Endlosschleife
                    {
                        bItemFirstChanged = listToReg[i].GetItemFirstChanged();

                        if (bItemFirstChanged)
                        {
                            break;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }

                    }
                }

            } );

        }
        */
        #endregion

        public List<IStParamPLCDB> FilterParameter(List<IStParamPLCDB> listToFilter, ModulIDs modulID) // Parameterlist nach FilterByModul filtern
        {   
            return DoFilter(listToFilter, (int)modulID, modulID.GetType());
        }

        public List<IStParamPLCDB> FilterParameter(List<IStParamPLCDB> listToFilter, StationIDs statID) // Parameterlist nach FilterByStation filtern
        {
            return DoFilter(listToFilter, (int)statID, statID.GetType());
        }

        private List<IStParamPLCDB> DoFilter(List<IStParamPLCDB> _listToFilt, int _iTargetID, Type _IDType)
        {
            List<IStParamPLCDB> _listFiltered = new List<IStParamPLCDB>();
            int iListItemID = 0;

            for (int i = 0; i < _listToFilt.Count; i++)
            {
                if (_IDType == typeof(StationIDs))
                    iListItemID = _listToFilt[i].GetStationIDProperty();
                else if (_IDType == typeof(ModulIDs))
                    iListItemID = _listToFilt[i].GetModulIDProperty();


                if (iListItemID == _iTargetID)
                {
                    Type _typeOfGeneric = GlobalFunc.FindTypeOfGenericSettingItem(_listToFilt[i]);
                    dynamic selectedItemCasted = Convert.ChangeType(_listToFilt[i], _typeOfGeneric); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar. Als var oder object auch nicht.

                    _listFiltered.Add(selectedItemCasted);
                }
            }
            return _listFiltered;
        }

        public List<IStParamPLCDB> FilterMachineParameter(List<IStParamPLCDB> _listToFilt, StationIDs _statID, ModulIDs _modulID)
        {
            List<IStParamPLCDB> _listFiltered = new List<IStParamPLCDB>();
            int iItemStatID  = 0;
            int iItemModulID = 0;

            for (int i = 0; i < _listToFilt.Count; i++)
            {
                iItemStatID     = _listToFilt[i].GetStationIDProperty();
                iItemModulID    = _listToFilt[i].GetModulIDProperty();

                if(iItemStatID==(int)_statID    &&  iItemModulID==(int)_modulID)
                {
                    Type _typeOfGeneric = GlobalFunc.FindTypeOfGenericSettingItem(_listToFilt[i]);
                    dynamic selectedItemCasted = Convert.ChangeType(_listToFilt[i], _typeOfGeneric); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar. Als var oder object auch nicht.

                    _listFiltered.Add(selectedItemCasted);
                }
            }

            return _listFiltered;
        }

        public List<IStParamPLCDB> FilterTypeParameter(List<IStParamPLCDB> _listToFilt)
        {
            List<IStParamPLCDB> _listFiltered = new List<IStParamPLCDB>();
            for (int i = 0; i < _listToFilt.Count; i++)
            {
                if (_listToFilt[i].GetNameProperty() != "") // Der Typparameter soll nur angezeigt werden, wenn es einen Namen hat. Wenn
                {
                    Type _typeOfGeneric         = GlobalFunc.FindTypeOfGenericSettingItem(_listToFilt[i]);
                    dynamic selectedItemCasted  = Convert.ChangeType(_listToFilt[i], _typeOfGeneric); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar. Als var oder object auch nicht.

                    _listFiltered.Add(selectedItemCasted);
                }
            }

            return _listFiltered;
        }


        /// <summary>
        /// Leeren Rezept erstellen
        /// </summary>
        public void NewParamSet(string sName, ParamSetTypes paramSet)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_paramset NewParamSet = new db_paramset();
                NewParamSet.sName = sName;
                NewParamSet.iType = (int)paramSet;
                NewParamSet.dtCreatedOn = DateTime.Now;
                NewParamSet.sCreatedBy = GlobalVar.ActUser.sUserName;
                NewParamSet.dtLastModified = NewParamSet.dtCreatedOn;

                List<db_parameter> list_db_parameter = new List<db_parameter>();
                List<IStParamPLCDB> list_AllPLCParam = new List<IStParamPLCDB>();


                if (ParamSetTypes.Type == paramSet)
                {
                    list_AllPLCParam = SetTypeParameter();
                    list_db_parameter = InitDBListBasedOnPLCList(list_AllPLCParam);
                }
                else if (ParamSetTypes.Machine == paramSet)
                {
                    list_AllPLCParam  = SetMachineParameter();
                    list_db_parameter = InitDBListBasedOnPLCList(list_AllPLCParam);
                }

                for (int i = 0; i < list_db_parameter.Count; i++)
                {
                    NewParamSet.db_parameter.Add(list_db_parameter[i]);
                }
                context.db_paramset.Add(NewParamSet);
                context.SaveChanges();
            }
        }



        public List<db_parameter> InitDBListBasedOnPLCList(List<IStParamPLCDB> listPLC)
        {
            List<db_parameter> listDBParam = new List<db_parameter>();

            for(int i=0; i<listPLC.Count; i++)
            {
                listDBParam.Add(new db_parameter());

                Type _type = GlobalFunc.FindTypeOfGenericSettingItem(listPLC[i]);
                dynamic selectedItemCasted = Convert.ChangeType(listPLC[i], _type); // dynamic, weil die Felder sSettingName, ValDB, ValPLC usw. ansonsten nicht ansprechbar. Als var oder object auch nicht.

                // ***** Init.wert 
                if(_type == typeof(StParamPLCDB<bool>))
                {
                    listDBParam[i].sValue = "false";
                }
                else if(_type == typeof(StParamPLCDB<Int32>) || _type == typeof(StParamPLCDB<double>)   )
                {
                    listDBParam[i].sValue = "0";
                }
                else if(_type == typeof(StParamPLCDB<string>))
                {
                    listDBParam[i].sValue = "-";
                }

                // ***** ADS-Name
                listDBParam[i].sADSName = listPLC[i].GetADSNameProperty();
            }

            return listDBParam;
        }


























        /// <summary>
        /// Prüft ob die Parameter im Rezept mit den aktuell deklarierten übereinstimmen
        /// </summary>
        public void CheckRecipeParameter(string sName, ParamSetTypes paramSet)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                //Prüfen
                db_paramset ParamSet = context.db_paramset.Single(r => ((r.sName == sName) && ((r.iType == (int)paramSet))));


                List<db_parameter> ListAllParameter = new List<db_parameter>();
                if (ParamSetTypes.Type == paramSet)
                    //ListAllParameter = SetTypeParameter();
                    ;
                else if (ParamSetTypes.Machine == paramSet)
                    ;
                //ListAllParameter = SetMachineParameter(); //Anpassen, MB 3.8.2020

            }
        }

        /// <summary>
        /// Lädt die Parameter eines Rezepts in die PLC und in das globale Rezept
        /// MBA 28.8.2020: Eingabeparameter geändert. 
        ///     - Alt: LoadParamSetToPLC(string sName, ParamSetTypes paramSet)
        ///     - Neu: LoadParamSetToPLC(int    iID,   ParamSetTypes paramSet)
        /// </summary>
        #region Alte Version
        /*
        public void LoadParamSetToPLC(string sName, ParamSetTypes paramSet)
        {
            if(GlobalVar.debugDB.bWantUseDB) // MBA 10.8.2020
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    db_paramset ParamSet;
                    try
                    {
                        ParamSet = context.db_paramset.First(r => ((r.sName == sName) && ((r.iType == (int)paramSet))));
                    }
                    catch
                    {
                        ParamSet = null;
                    }
                    if (ParamSet != null)
                    {
                        //CheckRecipeParameter(sName, paramSet); MB 6.8.2020

                        // Laden
                        ParamSet = context.db_paramset.Single(r => ((r.sName == sName) && ((r.iType == (int)paramSet))));

                        List<db_parameter> ListParameter = ParamSet.db_parameter.ToList(); // FR
                        IADSConnection VarCon = new ADSService();

                        for (int i = 0; i < ListParameter.Count; i++)
                        {
                            VarCon.WriteItem(ListParameter[i].sADSName + ".HMI_TO_PLC.val", ListParameter[i].sValue); // MB 4.8.2020
                        }


                        // Aktueller Typ in DB speichern
                        db_actual DBAct = context.db_actual.Single(d => (d.id == 1));

                        if (ParamSetTypes.Type == paramSet)
                            DBAct.sTypeParamSetName = sName;
                        else if (ParamSetTypes.Machine == paramSet)
                            DBAct.sMachineParamSetName = sName;

                        context.SaveChanges();
                    }
                    else
                    {
                        // Aktueller Typ in DB speichern
                        db_actual DBAct = context.db_actual.Single(d => (d.id == 1));

                        if (ParamSetTypes.Type == paramSet)
                            DBAct.sTypeParamSetName = ""; // 6.8.2020 MB: ???   -> KlAUS
                        else if (ParamSetTypes.Machine == paramSet)
                            DBAct.sMachineParamSetName = "";

                        context.SaveChanges();
                    }
                    LoadParamSetToGlobalVar(sName, paramSet);
                }
            }
            else
            {
                MessageBox.Show("Der Parameterset kann nicht in die SPS geladen werden\n" +
                                "weil die ADS-Verbindung nicht aktiv ist.",
                                "Keine ADS-Verbindung", //Caption
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }

        }
        */
        #endregion

        public void LoadParamSetToPLC(int iID, ParamSetTypes paramSetType)
        {
            if (GlobalVar.debugDB.bWantUseParam) // MBA 10.8.2020
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    db_paramset ParamSet;
                    try
                    {
                        //ParamSet = context.db_paramset.First(r => ((r.id == iID) && ((r.iType == (int)paramSetType))));
                        ParamSet = context.db_paramset.First(r => (r.id == iID)); // MBA 31.8.2020
                    }
                    catch
                    {
                        ParamSet = null;
                    }
                    if (ParamSet != null)
                    {
                        //CheckRecipeParameter(sName, paramSet); MB 6.8.2020

                        // Laden
                        //ParamSet = context.db_paramset.Single(r => ((r.id == iID) && ((r.iType == (int)paramSetType))));
                        ParamSet = context.db_paramset.Single(r => (r.id == iID));// MBA 31.8.2020

                        List<db_parameter> ListParameter = ParamSet.db_parameter.ToList(); // FR
                        IADSConnection VarCon = new ADSService();

                        for (int i = 0; i < ListParameter.Count; i++)
                        {

                            string text = ListParameter[i].sValue;
                            if((ListParameter[i].sADSName.Contains("gMPlr_arr") ==true) || (ListParameter[i].sADSName.Contains("gTPlr_arr")==true))
                            {
                                if (text.Contains('.'))
                                {
                                    text = text.Replace('.', ',');
                                }
                            }
                            VarCon.WriteItem(ListParameter[i].sADSName + ".HMI_TO_PLC.val", text);
                        }

                            // Den aktuellen Parametersatz-ID in DB speichern
                          db_actual DBAct = context.db_actual.Single(d => (d.id == 1));

                        if (ParamSetTypes.Type == paramSetType)
                        {
                            DBAct.iTypeparamSetID = ParamSet.id;
                        }                            
                        else if (ParamSetTypes.Machine == paramSetType)
                            DBAct.iMachineparamSetID = ParamSet.id;

                        context.SaveChanges();
                    }
                    else
                    {
                        // Den aktuellen Parametersatz-ID in DB speichern
                        db_actual DBAct = context.db_actual.Single(d => (d.id == 1));

                        if (ParamSetTypes.Type == paramSetType)
                            //DBAct.sTypeParamSetName = ""; // 6.8.2020 MB: ???   -> KlAUS
                            DBAct.iTypeparamSetID = 0; // 31.8
                        else if (ParamSetTypes.Machine == paramSetType)
                            //DBAct.sMachineParamSetName = "";
                            DBAct.iMachineparamSetID = 0;

                        context.SaveChanges();
                    }
                    //LoadParamSetToGlobalVar(ParamSet.sName, paramSetType);
                    if(ParamSet != null)
                    {
                        LoadParamSetToGlobalVar(ParamSet.id, paramSetType);
                    }
                    else
                    {
                        string strTemp = "";

                        if (ParamSetTypes.Type == paramSetType)
                            strTemp = "Typparameter";
                        else if (ParamSetTypes.Machine == paramSetType)
                            strTemp = "Maschinenparameter";

                        MessageBox.Show("Die Variable 'ParamSet' ist null und kann nicht in die globale Variable geladen werden.",
                                        strTemp,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }


                }
            }
            else
            {
                MessageBox.Show("Der Parameterset kann nicht in die SPS geladen werden\n" +
                                "weil die ADS-Verbindung nicht aktiv ist.",
                                "Keine ADS-Verbindung", //Caption
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
            }

        }


        /// <summary>
        /// Speichert den Parameter in der Datenbank und ggf. in die globale Struktur
        /// </summary>
        public void SaveParameterInDB(int iParmSetId, string sParamAdsName, string sParamValue)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_paramset _ParamSet = new db_paramset(); // MBA 14.20.2020

                try
                {
                    // Aktuellen Satz aus DB holen
                    _ParamSet = context.db_paramset.Single(r => (r.id == iParmSetId));
                    _ParamSet.dtLastModified = DateTime.Now; // Balog 22.10.2019

                    // Parameter holen
                    db_parameter _Parameter = _ParamSet.db_parameter.Single(p => p.sADSName == sParamAdsName);
                    // neuen Wert übergeben
                    _Parameter.sValue = sParamValue;
                    // speichern in DB 
                    context.SaveChanges();
                }
                catch
                {
                    MessageBox.Show("Error at SaveOrderToDB", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                if(GlobalVar.ActTypeParamSet == null || GlobalVar.ActMachineParamSet == null) // n.i.O
                {
                    MessageBox.Show("Die Methode 'LoadParamSetToGlobalVar' wird nicht aufgerufen.", 
                                    "",  
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Warning);
                }
                else // i.O.
                {
                    // global Rezept wieder aus der DB holen (aktualisieren) und in die PLC schreiben
                    if ((GlobalVar.ActTypeParamSet.id == _ParamSet.id) || (GlobalVar.ActMachineParamSet.id == _ParamSet.id))
                        //LoadParamSetToGlobalVar(_ParamSet.sName, (ParamSetTypes)_ParamSet.iType);
                        LoadParamSetToGlobalVar(_ParamSet.id, (ParamSetTypes)_ParamSet.iType);
                }
            }
        }


        public void AddNotFoundParamToParamSet(int iParmSetId, string sParamAdsName, string sParamValue) // MB 6.8.2020: 
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                // Aktuellen Satz aus DB holen
                db_paramset _ParamSet = context.db_paramset.Single(r => (r.id == iParmSetId));
                _ParamSet.dtLastModified = DateTime.Now; 

                // Neuen Parameter erstellen
                db_parameter _Parameter = new db_parameter();
                _Parameter.sADSName = sParamAdsName;
                _Parameter.sValue   = sParamValue;

                // speichern in DB 
                _ParamSet.db_parameter.Add(_Parameter);
                context.SaveChanges();

                // global Rezept wieder aus der DB holen (aktualisieren) und in die PLC schreiben
                try
                {
                    if ((GlobalVar.ActTypeParamSet.id == _ParamSet.id) || (GlobalVar.ActMachineParamSet.id == _ParamSet.id))
                        //LoadParamSetToGlobalVar(_ParamSet.sName, (ParamSetTypes)_ParamSet.iType);
                        LoadParamSetToGlobalVar(_ParamSet.id, (ParamSetTypes)_ParamSet.iType);
                }
                catch
                {
                    ;
                }

            }

        }




        /// <summary>
        /// FRI: Lädt einen Parametersatz aus Datenbank in die globale Struktur
        /// 
        /// MBA 28.8.2020: Eingabeparameter geändert. 
        ///     - Alt: LoadParamSetToGlobalVar(string sName, ParamSetTypes paramSet)
        ///     - Neu: LoadParamSetToGlobalVar(string sName, ParamSetTypes paramSet)
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        #region alt
        /*
        public void LoadParamSetToGlobalVar(string sName, ParamSetTypes paramSet)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                if (ParamSetTypes.Type == paramSet)
                {
                    if (sName != "")
                    {
                        GlobalVar.ActTypeParamSet = context.db_paramset.Single(r => ((r.sName == sName) && ((r.iType == (int)paramSet))));
                        GlobalVar.ActTypeParamList = GlobalVar.ActTypeParamSet.db_parameter.ToList();
                    }
                    else
                    {
                        GlobalVar.ActTypeParamSet = new db_paramset();
                        //GlobalVar.ActTypeParamList = SetTypeParameter(); KLAUS
                    }
                }
                else if (ParamSetTypes.Machine == paramSet)
                {
                    if (sName != "")
                    {
                        GlobalVar.ActMachineParamSet = context.db_paramset.Single(r => ((r.sName == sName) && ((r.iType == (int)paramSet))));
                        GlobalVar.ActMachineParamList = GlobalVar.ActMachineParamSet.db_parameter.ToList();
                    }
                    else
                    {
                        GlobalVar.ActMachineParamSet = new db_paramset();
                        //GlobalVar.ActMachineParamList = SetMachineParameter(); // KLAUS Anpassen MB 3.8.2020
                    }
                }
            }
        }
        */
        #endregion
        public void LoadParamSetToGlobalVar(int iID, ParamSetTypes paramSetType)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                if (paramSetType == ParamSetTypes.Type)// Typparameter
                {
                    if (iID != 0)
                    {
                        //GlobalVar.ActTypeParamSet = context.db_paramset.Single(r => ((r.id == iID) && ((r.iType == (int)paramSet))));
                        GlobalVar.ActTypeParamSet = context.db_paramset.Single(r => (r.id == iID)); // MBA 31.8.2020
                        GlobalVar.ActTypeParamList = GlobalVar.ActTypeParamSet.db_parameter.ToList();
                    }
                    else
                    {
                        GlobalVar.ActTypeParamSet = new db_paramset();
                        //GlobalVar.ActTypeParamList = SetTypeParameter(); KLAUS
                    }
                }
                else if (paramSetType == ParamSetTypes.Machine) // Maschinenparameter
                {
                    if (iID != 0)
                    {
                        //GlobalVar.ActMachineParamSet = context.db_paramset.Single(r => ((r.id == iID) && ((r.iType == (int)paramSet))));
                        GlobalVar.ActMachineParamSet = context.db_paramset.Single(r => (r.id == iID) );// MBA 31.8.2020
                        GlobalVar.ActMachineParamList = GlobalVar.ActMachineParamSet.db_parameter.ToList();
                    }
                    else
                    {
                        GlobalVar.ActMachineParamSet = new db_paramset();
                        //GlobalVar.ActMachineParamList = SetMachineParameter(); // KLAUS Anpassen MB 3.8.2020
                    }
                }
            }
        }





        /// <summary>
        /// Holt die Liste der ParameterSets aus der Datenbank
        /// </summary>
        public List<db_paramset> GetParmSetList(ParamSetTypes paramSet)
        {
            List<db_paramset> ListParamSets = new List<db_paramset>();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    ListParamSets = context.db_paramset.Where(r => (r.iType == (int)paramSet)).ToList();
                }
                catch
                {

                }
                return ListParamSets;
            }
        }

        /// <summary>
        /// Holt die Liste der ParameterSets aus der Datenbank
        /// </summary>
        public db_paramset GetParamSetOverId(int id)
        {
            db_paramset db_Paramset;
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_Paramset = context.db_paramset.Single(r => (r.id == id));
                return db_Paramset;
            }
        }

        /// <summary>
        /// Holt die Liste der Parameter aus der Datenbank
        /// </summary>
        public List<db_parameter> GetParamListOverId(int id) // MB 6.8.2020: wenn ein neuer ParamList geladen wird, sollte es gleich geprüft werden, ob die Daten
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_paramset db_Paramset = context.db_paramset.Single(r => (r.id == id));
                List<db_parameter> List = new List<db_parameter>();
                if (db_Paramset != null)
                    List = db_Paramset.db_parameter.ToList();

                return List;
            }
        }

        public bool ParamsetIsInOrderQueue(db_paramset paramsetToCheck)
        {
            bool result = false;

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                //List<db_paramset> listParamset;
                //listParamset = context.db_paramset.ToList();

                List<db_orderqueue> listOrder = context.db_orderqueue.ToList();

                for(int i=0; i< listOrder.Count; i++)
                {
                    if (listOrder[i].iParamSetId == paramsetToCheck.id)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public bool ParamsetInPLC(db_paramset paramSetToCheck)
        {
            bool bResult = false;

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {

            }



            return bResult;
        }

    }
}
