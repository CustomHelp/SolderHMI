using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;

namespace LOET_HMI.PLC_Com_Classes
{

    public class SettingCluster
    {
        public IStParamPLCDB SettingStruct { get; set; }
        public string RegistrationName { get; set; }

        public SettingCluster(IStParamPLCDB st_setting, string name)
        {
            SettingStruct = new StParamPLCDB<IStParamPLCDB>();
            SettingStruct = st_setting;
            RegistrationName = name;
        }
    }

    public interface IStParamPLCDB 
    {
        // ***** Interface - Zugriff auf die Sturkturelemente (des Setzen der neue Werte in SPS/DB erfolgt über WriteVal())
        string GetPLCValueProperty();
        void   SetPLCValueProperty(string value);
        void   SetDBProperties(string value, int paramSetID); // Den von der DB ausgelesenen Wert in der Klasse StParamPLCDB ablegen. (VORSICHT: der Wert in der Datenbank wird hier nicht geschrieben!)

        void   SetADSNameProperty(string adsName);
        void   SetNameProperty(string  sName); // MBA 31.05.2021
        void   SetDescrProperty(string sDescription); // MBA 31.05.2021

        string GetADSNameProperty();
        string GetNameProperty();
        string GetDescrProperty(); // MBA 31.05.2021

        int    GetModulIDProperty();
        int    GetStationIDProperty();

        // ***** Interface ADS
        void Register(string sName);
        void Register();
        void Deregister();
        bool GetItemFirstChanged();


    }

    // ************************************************************************************
    // *************************** MarshalAs (Strukturen in der SPS)  *********************
    // ************************************************************************************
    #region MarshalAs (Strukturen in der SPS) 
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_BOOL
    {
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string strUnit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDescription;

        
        [MarshalAs(UnmanagedType.I1)]
        public bool val;
        /*
        //[MarshalAs(UnmanagedType.I1)]
        //public bool minVal;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool maxVal;

        [MarshalAs(UnmanagedType.U2)] // Beckhoff: UINT ist 16 Bit groß
        public Int16 uiMinLevel;
        */
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_DINT
    {
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string strUnit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDescription;

        [MarshalAs(UnmanagedType.I4)]
        public Int32 minVal;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 maxVal;
       
        [MarshalAs(UnmanagedType.I4)] // Beckhoff: DINT ist 32 Bit groß
        public Int32 val;
        /*
        [MarshalAs(UnmanagedType.U2)] // Beckhoff: UINT ist 16 Bit groß
        public Int16 uiMinLevel;
        */

        // Für die Dropdown-Variante:
        [MarshalAs(UnmanagedType.I1)]
        public bool bIsDropdown;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiCountDropItems;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem01;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem02;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem03;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem04;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem05;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem06;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem07;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem08;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem09;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strDropDownItem10;        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_LREAL
    {
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string strUnit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDescription;

        [MarshalAs(UnmanagedType.R8)]
        public double minVal;
        [MarshalAs(UnmanagedType.R8)]
        public double maxVal;

        
        [MarshalAs(UnmanagedType.R8)] // Beckhoff: LREAL ist 64 Bit groß
        public double val;
        /*
        [MarshalAs(UnmanagedType.U2)] // Beckhoff: UINT ist 16 Bit groß
        public Int16 uiMinLevel;
        */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_STRING
    {
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string strUnit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDescription;


        //[MarshalAs(UnmanagedType.R8)]
        //public double minVal;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string maxVal;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 101)]
        public string val;



        //[MarshalAs(UnmanagedType.U2)] // Beckhoff: UINT ist 16 Bit groß
        //public Int16 uiMinLevel;

    }
    #endregion


    public partial class StParamPLCDB<T> : INotifyPropertyChanged, IStParamPLCDB
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();


        ICHPTranslate Tanslater = new CHPTransService();


        // ************************************************************************************
        // ****************************** Properties - ADS ************************************
        // ************************************************************************************
        #region
        public string ADSName { get; set; }

        //public bool bFirstADSItemChanged { get; set; }

        private bool _bFirstADSItemChanged;
        public bool bFirstADSItemChanged
        {
            get
            {
                return _bFirstADSItemChanged;
            }

            set
            {
                _bFirstADSItemChanged = value;               
                OnPropertyChanged();
                DBParam.Handler.iCountFirstADSItemChanged += 1;
            }
        }




        #endregion

        // ************************************************************************************
        // ****************************** Properties - DB *************************************
        // ************************************************************************************
        #region
        private T _ValDB;
        public T ValDB
        {
            get
            {
                return _ValDB;
            }

            set
            {
                _ValDB = value;
                ValDB_ForDataGrid = value.ToString(); // Damit"OnPropertyChanged()" auch beim DataGrid-Property aufgerufen wird
                OnPropertyChanged();
            }
        }


        public string ValDB_ForDataGrid // Es ist praktisch eine Schnittstelle für die DINT-Parameter, damit auch die DropDownMenüs benutzerfreundlich verwaltet werden.
        {
            get
            {
                string text = "";
                if (bIsDropdown)
                {
                    if (typeof(T) == typeof(Int32)) // bei Dropdown-Menü-Parameter ist in "ValDB" der DropDownItemIndex (1...10) gespeichert. Im DataGrid sollte statt dem Index der DropDownItemValue (z.B. 100, 150, usw) angezeigt werden.
                    {
                        T iValueDropDownItemDB = (T)Convert.ChangeType(-1, typeof(T), CultureInfo.InvariantCulture); // Init.Wert -1 in den generischen Typ "T" casten. (return Type muss T bleiben, bei Int32 Fehlermeldung...)
                        int iTmpValDB = (int)Convert.ChangeType(_ValDB, typeof(int));

                        for (int i = 1; i <= 10; i++) // Aufgrund dem Index den Wert finden
                        {
                            if (iTmpValDB == i)
                            {
                                text = arr_sDropDownItems[i - 1];
                            }
                        }

                        return text;
                    }
                    else
                    {
                        return text;
                    }
                }
                else // wenn es kein Dropdown-Menü-Parameter ist, direkt "ValDB" zurückgeben
                {
                    return text;
                }
            }
            set // Hier wird nur OnPropertyChanged() aufgerufen, damit der get-Zweig für das DataGrid aufgerufen wird. (Der Wert wird in einer private-Variable nicht abgelegt)  
            {
                OnPropertyChanged();
            }
        }




        private int _iParamSetID;
        public int iParamSetID
        {
            get
            {
                return _iParamSetID;
            }

            set
            {
                _iParamSetID = value;
                OnPropertyChanged();
            }
        }


        #endregion


        // ************************************************************************************
        // ****************************** Properties - PLC_TO_HMI *****************************
        // ************************************************************************************
        #region 
        private T _Val;
        public T Val
        {
            get
            {
                return _Val;
            }

            set
            {
                _Val = value;
                Val_ForDataGrid = value.ToString(); // Damit"OnPropertyChanged()" auch beim DataGrid-Property aufgerufen wird
                OnPropertyChanged();
            }
        }

        private T _Min;
        public T Min
        {
            get
            {
                return _Min;
            }

            set
            {
                _Min = value;
                OnPropertyChanged();
            }
        }

        private T _Max;
        public T Max
        {
            get
            {
                return _Max;
            }

            set
            {
                _Max = value;
                OnPropertyChanged();
            }
        }

        public string Val_ForDataGrid // Es ist praktisch eine Schnittstelle für die DINT-Parameter, damit auch die DropDownMenüs benutzerfreundlich verwaltet werden.
        {
            get
            {
                string text = "";
                if (typeof(T) == typeof(Int32) && bIsDropdown) // bei Dropdown-Menü-Parameter ist in "Val" der DropDownItemIndex (1...10) gespeichert. Im DataGrid sollte statt dem Index der DropDownItemValue angezeigt werden.
                {
                    T iValueDropDownItemDB = (T)Convert.ChangeType(-1, typeof(T), CultureInfo.InvariantCulture); // Init.Wert -1 in den generischen Typ "T" casten. (return Type muss T bleiben, bei Int32 Fehlermeldung...)
                    int iTmpVal = (int)Convert.ChangeType(_ValDB, typeof(int));

                    for (int i = 1; i <= 10; i++) // Aufgrund dem Index den Wert finden
                    {
                        if (iTmpVal == i)
                        {
                            text = arr_sDropDownItems[i - 1];
                            return text;
                        }
                    }
                    return text;
                }
                else // wenn es kein Dropdown-Menü-Parameter ist, direkt "Val" zurückgeben
                {
                    return _Val.ToString();
                }
            }

            set // Hier wird nur OnPropertyChanged() aufgerufen, damit der get-Zweig für das DataGrid aufgerufen wird. (Der Wert wird in einer private-Variable nicht abgelegt)  
            {
                OnPropertyChanged();
                ;
            }

        }


        //**********************************************************************
        private string _strName;
        public string strName
        {
            get
            {
                return _strName;
            }

            set
            {
                //_strName = value;
                _strName = Tanslater.TransTxt(value, eFBType.fc_Param); 
                OnPropertyChanged();
            }
        }
        private string _strUnit;
        public string strUnit
        {
            get
            {
                return _strUnit;
            }

            set
            {
                _strUnit = value;
                OnPropertyChanged();
            }
        }

        private string _strDescription;
        public string strDescription
        {
            get
            {
                return _strDescription;
            }

            set
            {
                //_strDescription = value;
                _strDescription = Tanslater.TransTxt(value, eFBType.fc_Param);
                OnPropertyChanged();
            }
        }

        //***********************************************************************
        private Int16 _iMinLevel;
        public Int16 iMinLevel
        {
            get
            {
                return _iMinLevel;
            }

            set
            {
                _iMinLevel = value;
                OnPropertyChanged();
            }
        }
        
        private Int16 _iModul;
        public Int16 iModul
        {
            get
            {
                return _iModul;
            }

            set
            {
                _iModul = value;
                OnPropertyChanged();
            }
        }
        
        private Int16 _iStation;
        public Int16 iStation
        {
            get
            {
                return _iStation;
            }

            set
            {
                _iStation = value;
                OnPropertyChanged();
            }
        }

        //*************************************************************************
        //******************* Ergänzungen NUR für Dropdown Menü *******************
        private bool _bIsDropdown;
        public bool bIsDropdown
        {
            get
            {
                return _bIsDropdown;
            }

            set
            {
                _bIsDropdown = value;
                OnPropertyChanged();
            }
        }

        private int _iCountDropItems;
        public int iCountDropItems
        {
            get
            {
                return _iCountDropItems;
            }

            set
            {
                _iCountDropItems = value;
                OnPropertyChanged();
            }
        }

        private string[] _arr_sDropDownItems;
        public string[] arr_sDropDownItems // Die Einträge im Dropdown Menü
        {
            get
            {
                return _arr_sDropDownItems;
            }

            set
            {
                _arr_sDropDownItems = value;
                OnPropertyChanged();
            }
        }



        private int _iIndexSelectedDropDownItem;
        public int iIndexSelectedDropDownItem  // Bei Dropdown-Menü-Parameter wird nicht der Eintrag selbst, sondern der Index in der DB gespeichert und an die SPS gesendet.
        {
            get
            {
                return _iIndexSelectedDropDownItem;
            }

            set
            {
                _iIndexSelectedDropDownItem = value;
                OnPropertyChanged();
            }
        }
        //*************** Ergänzungen NUR für Dropdown Menü (Ende) ****************
        //*************************************************************************

        #endregion


        // ************************************************************************************
        // ****************************** Neuen Wert speichern ********************************
        // ************************************************************************************

        public void WriteValToPLCAndDB(T new_Val)
        {

            // ***********************************************************************
            // *********************** Value in SPS schreiben / ADS - TX *************
            // ***********************************************************************
            if(GlobalVar.ActTypeParamSet == null || GlobalVar.ActMachineParamSet == null) // n.i.O.: meistens bei den ersten Aufrufen...
            {
                string sTmp = "";

                if (GlobalVar.ActMachineParamSet == null)
                {
                    sTmp = "Maschinenparameter";
                }
                if (GlobalVar.ActTypeParamSet == null)
                {
                    if (sTmp == "")
                        sTmp = "Typparameter";
                    else
                        sTmp = sTmp + " und Typparameter";
                }

                MessageBox.Show("Zunächst muss ein Parametersatz von folgenden Parametertypen in die SPS geladen werden:\n"
                                + sTmp
                                + "\n\n"
                                + "Hierfür beim jeweiligen Parametertyp auf 'In SPS laden' klicken.", // text
                                "", // caption 
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            else // i.O.
            {
                if (iParamSetID == GlobalVar.ActMachineParamSet.id || iParamSetID == GlobalVar.ActTypeParamSet.id)
                // Nur dann in die SPS schreiben, wenn der vorliegende Parameter zu dem Parametersatz gehört, der gerade aktiv ist (in die SPS geladen ist)
                // Sonst die Wertänderung nur in der DB abspeichern (siehe unten)
                {
                    VarCon.WriteItem(ADSName + ".HMI_TO_PLC.val", new_Val);
                }
            }



            // ************************************************************************
            // ************************ Value in DB speichern *************************
            // ************************************************************************
            // URSPRÜNGLICHER BEFEHL:
            // DBParam.Handler.SaveParameter(ParamSetId, ADSName, new_Val.ToString()); 
            //      Balog 20.7.2020: wenn die HMI-Sprache Deutsch war und ein Gleitkommazahl-Parameter geändert wurde, wurde das Dezimalzeichen im HMI nicht mehr angezeigt. 
            //      Grund: das Dezimalzeichen wird in der Datenbank in diesem Fall als Komma gespeichert. Beim Einlesen wird allerdings immer nach einem Punkt gesucht (InvariantCulture).

            // KONVERTIERUNGSTEST 1:
            string sNewVal_tmp1 = new_Val.ToString();
            //      Wenn die HMI-Sprache Deutsch ist, ist das Dezimalzeichen eine Komma (siehe CultureInfo). 
            //      Dann liefert ToString() ein String, in dem das Dezimalzeichen eine Komma ist.

            // KONVERTIERUNGSTEST 2:
            string sNewVal_tmp2 = (string)Convert.ChangeType(new_Val, typeof(string), CultureInfo.InvariantCulture);
            //      Wenn man diese String-Konvertierung verwendet, wird als Dezimalzeichen sprachenunabhängig ("InvariantCulture") immer ein Punkt verwendet. 
            //      So wird das Dezimalzeichen nach der Änderung eines Gleitkommazahl-Parameters auch im HMI richtig angezeigt.

            DBParam.Handler.SaveParameterInDB(iParamSetID, ADSName, sNewVal_tmp2); // Param. mit der 2. Konvertierungsoption speichern





        }



        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        // ***************************************************************************************
        // ********************* Implementierung Interface - Zugriff auf Properties **************
        // ***************************************************************************************
        #region 
        // **** PLC-Value
        public string GetPLCValueProperty()
        {
            return Val.ToString();
        }

        public void SetPLCValueProperty(string value)
        {
            Val = (T)Convert.ChangeType(value, typeof(T));
        }

        // **** DB-Value
        //public void SetDBValue(string value)
        public void SetDBProperties(string value, int paramSetID) // MB 4.8.2020
        {
            ValDB = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            // Wenn man diese String-Konvertierung verwendet, wird als Dezimalzeichen sprachenunabhängig ("InvariantCulture") immer ein Punkt verwendet. 
            // So wird das Dezimalzeichen nach der Änderung eines Gleitkommazahl-Parameters auch im HMI richtig angezeigt.


            // MB: 
            iParamSetID = paramSetID;
        }

        public string GetDBValue()
        {
            return (string)Convert.ChangeType(ValDB, typeof(string), CultureInfo.InvariantCulture);
            // Wenn man diese String-Konvertierung verwendet, wird als Dezimalzeichen sprachenunabhängig ("InvariantCulture") immer ein Punkt verwendet. 
            // So wird das Dezimalzeichen nach der Änderung eines Gleitkommazahl-Parameters auch im HMI richtig angezeigt.
        }

        // ***** Name
        public string GetNameProperty()
        {
            return strName;
        }

        public void SetNameProperty(string sName)
        {
            strName = sName;
        }


        // ***** Description
        public string GetDescrProperty()
        {
            return strDescription;
        }

        public void SetDescrProperty(string sDescription)
        {
            strDescription = sDescription;
        }


        // **** ADS-Name
        public void SetADSNameProperty(string adsName)
        {
            ADSName = adsName;
        }

        public string GetADSNameProperty()
        {
            return ADSName;
        }


        // **** Modul/Statin ID
        public int GetModulIDProperty()
        {
            return iModul;
        }

        public int GetStationIDProperty()
        {
            return iStation;
        }

        #endregion

        // ***************************************************************************************
        // ********************** Implementierung Interface - ADS ********************************
        // ***************************************************************************************
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = sName;
            DoRegister();
        }

        public void Register()
        {
            Item.sName = ADSName;
            DoRegister();
        }

        public void Deregister()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
                VarCon.RemoveItem(Item);
            }));
        }

        public bool GetItemFirstChanged()
        {
            return bFirstADSItemChanged;
        }
        #endregion


        // ***************************************************************************************
        // ********************************* ADS - RX ********************************************
        // ***************************************************************************************
        #region ADS-Connection
        public void DoRegister()
        {
            if (typeof(T) == typeof(bool))
            {
                Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_Setting_BOOL));
                Item.Type = typeof(ST_Setting_BOOL);
            }
            else if (typeof(T) == typeof(Int32))
            {
                Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_Setting_DINT));
                Item.Type = typeof(ST_Setting_DINT);
            }
            else if (typeof(T) == typeof(double))
            {
                Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_Setting_LREAL));
                Item.Type = typeof(ST_Setting_LREAL);
            }
            else if (typeof(T) == typeof(string))
            {
                Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_Setting_STRING));
                Item.Type = typeof(ST_Setting_STRING);
            }
            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }

        bool bValueChanged = true; //IBN: evt. als Property definieren
        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (Item.iHandle == e.Item[j].iHandle)
                {
                    if ((Item != e.Item[j]) || (Item.Value == null))
                    {
                        Item = e.Item[j];

                    }
                }
            }

            if (!bFirstADSItemChanged) // MB 5.8.2020: für die Filterung nach Station/Modul verwendet
                bFirstADSItemChanged = true; 

            bValueChanged = true; //MB 5.8.2020: von Frank 
            if (bValueChanged)
            {
                if (typeof(T) == typeof(bool))
                {
                    ST_Setting_BOOL tmp1 = (ST_Setting_BOOL)Item.Value;
                    if (tmp1 != null)
                    {
                        try
                        {
                            Max = (T)Convert.ChangeType(true, typeof(T));
                            Min = (T)Convert.ChangeType(false, typeof(T));
                            Val = (T)Convert.ChangeType(tmp1.val, typeof(T));

                            strName = tmp1.strName;
                            strDescription = tmp1.strDescription;
                            strUnit = "---";

                            //iMinLevel = tmp1.uiMinLevel;
                            iModul = tmp1.uiModul;
                            iStation = tmp1.uiStation;

                            bValueChanged = false;
                        }
                        catch (Exception ex)
                        { AppLogger.Log("StParamPLCDB.ItemChanged", ex); }
                    }

                }
                else if (typeof(T) == typeof(Int32))
                {
                    ST_Setting_DINT tmp2 = (ST_Setting_DINT)Item.Value;
                    if (tmp2 != null)
                    {
                        try
                        {
                            Max = (T)Convert.ChangeType(tmp2.maxVal, typeof(T));
                            Min = (T)Convert.ChangeType(tmp2.minVal, typeof(T));
                            Val = (T)Convert.ChangeType(tmp2.val, typeof(T));

                            strName = tmp2.strName;
                            strDescription = tmp2.strDescription;
                            strUnit = tmp2.strUnit;

                            //iMinLevel = tmp2.uiMinLevel;
                            iModul = tmp2.uiModul;
                            iStation = tmp2.uiStation;

                            bIsDropdown = tmp2.bIsDropdown;
                            iCountDropItems = tmp2.uiCountDropItems;

                            if (iCountDropItems > 0)
                            {
                                arr_sDropDownItems = new string[10];

                                for (int i = 0; i < iCountDropItems; i++)
                                {
                                    if (i == 0) arr_sDropDownItems[i] = tmp2.strDropDownItem01;
                                    if (i == 1) arr_sDropDownItems[i] = tmp2.strDropDownItem02;
                                    if (i == 2) arr_sDropDownItems[i] = tmp2.strDropDownItem03;
                                    if (i == 3) arr_sDropDownItems[i] = tmp2.strDropDownItem04;
                                    if (i == 4) arr_sDropDownItems[i] = tmp2.strDropDownItem05;
                                    if (i == 5) arr_sDropDownItems[i] = tmp2.strDropDownItem06;
                                    if (i == 6) arr_sDropDownItems[i] = tmp2.strDropDownItem07;
                                    if (i == 7) arr_sDropDownItems[i] = tmp2.strDropDownItem08;
                                    if (i == 8) arr_sDropDownItems[i] = tmp2.strDropDownItem09;
                                    if (i == 9) arr_sDropDownItems[i] = tmp2.strDropDownItem10;
                                }
                            }
                            
                            bValueChanged = false;
                        }
                        catch (Exception ex)
                        { AppLogger.Log("StParamPLCDB.ItemChanged", ex); }
                    }
                }
                else if (typeof(T) == typeof(double))
                {
                    ST_Setting_LREAL tmp3 = (ST_Setting_LREAL)Item.Value;
                    if (tmp3 != null)
                    {
                        try
                        {
                            Max = (T)Convert.ChangeType(tmp3.maxVal, typeof(T));
                            Min = (T)Convert.ChangeType(tmp3.minVal, typeof(T));
                            Val = (T)Convert.ChangeType(tmp3.val, typeof(T));

                            strName = tmp3.strName;
                            strDescription = tmp3.strDescription;
                            strUnit = tmp3.strUnit;

                            //iMinLevel = tmp3.uiMinLevel;
                            iModul = tmp3.uiModul;
                            iStation = tmp3.uiStation;

                            bValueChanged = false;
                        }
                        catch (Exception ex)
                        { AppLogger.Log("StParamPLCDB.ItemChanged", ex); }
                    }
                }
                else if (typeof(T) == typeof(string))
                {
                    ST_Setting_STRING tmp4 = (ST_Setting_STRING)Item.Value;
                    if (tmp4 != null)
                    {
                        try
                        {
                            Max = (T)Convert.ChangeType(tmp4.maxVal, typeof(T));
                            //Min = (T)Convert.ChangeType(tmp4.minVal, typeof(T));
                            Val = (T)Convert.ChangeType(tmp4.val, typeof(T));

                            strName = tmp4.strName;
                            strDescription = tmp4.strDescription;
                            strUnit = tmp4.strUnit;

                            //iMinLevel = tmp4.uiMinLevel;
                            iModul = tmp4.uiModul;
                            iStation = tmp4.uiStation;

                            bValueChanged = false;
                        }
                        catch (Exception ex)
                        { AppLogger.Log("StParamPLCDB.ItemChanged", ex); }
                    }
                }

            }

        }
        #endregion



    }
}
