using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;



namespace HOPA_HMI.PLC_Com_Classes
{
    // Urspünglicher Name am 3.8.2020: IStSetting__PLC
    public interface IStPLCSettingWithDB
    {
        /// <summary>
        /// ONLY AT SETTING_HMI
        /// </summary>
        void Register(string sName);


        void RegisterPLC(string sName);

        void Register(string sName, ParamSetTypes paramSets);

        void RegisterDB(int _ParamSetId, db_parameter _Parameter);

        string GetPLCValue();
        void SetPLCValue(string value);

        void Deregister();

        // ****** Balog 6.2.2020
        string GetADSName();
        void SetDescription(string txtDescr);
        // **********

    }

    // Ursprünglicher Name noch bei HOMS und REEB: St DBSetting  
    public partial class StPLCSettingWithDB<T> : INotifyPropertyChanged, IStPLCSettingWithDB
    {
        #region Variablen
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        private string ADSName { get; set; }
        private int ParamSetId { get; set; }

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

        private T _ValPLC;
        public T ValPLC
        {
            get
            {
                return _ValPLC;
            }

            set
            {
                _ValPLC = value;
                OnPropertyChanged();
            }
        }


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
                OnPropertyChanged();
            }
        }

        //***********************************************************************



        private bool _bAllowed;
        public bool bAllowed
        {
            get
            {
                return _bAllowed;
            }

            set
            {
                _bAllowed = value;
                OnPropertyChanged();
            }
        }


        private string _sSettingName;
        public string sSettingName
        {
            get
            {
                return _sSettingName;
            }

            set
            {
                _sSettingName = value;
                OnPropertyChanged();
            }
        }

        private string _sUnit;
        public string sUnit
        {
            get
            {
                return _sUnit;
            }

            set
            {
                _sUnit = value;
                OnPropertyChanged();
            }
        }

        private int _iUserLevel;
        public int iUserLevel
        {
            get
            {
                return _iUserLevel;
            }

            set
            {
                _iUserLevel = value;
                OnPropertyChanged();
            }
        }


        // ****************** Balog 6.2.2020
        /*
        private string _sDescrEnglish;
        public string sDescrEnglish
        {
            get
            {
                return _sDescrEnglish;
            }

            set
            {
                _sDescrEnglish = value;
                OnPropertyChanged();
            }
        }

        private string _sDescrGerman;
        public string sDescrGerman
        {
            get
            {
                return _sDescrGerman;
            }

            set
            {
                _sDescrGerman = value;
                OnPropertyChanged();
            }
        }
        */
        private string _sDescription; // 1 Feld für alle Sprachen -> die Übersetzungen gehen in die Resource-Dateien
        public string sDescription
        {
            get
            {
                return _sDescription;
            }

            set
            {
                _sDescription = value;
                OnPropertyChanged();
            }
        }
        // ****************************


        #endregion


        ////////////////////////////////////
        ///////////// Commands /////////////
        ////////////////////////////////////


        // WriteVal wird dem generischen Typ T angepasst:
        public void WriteVal(T new_Val)
        {
            // Wenn ParamSetId < 0 ist, dann ist kein Parameterset geladen => es kann/braucht nichts geschrieben zu werden
            if (ParamSetId >= 0)
            {
                // ***** Log schreiben
                DBLog.Handler.Parameter(ParamSetId, sSettingName, ValDB.ToString(), new_Val.ToString(), "store to DB");


                // ***** in DB speichern
                //DBParam.Handler.SaveParameter(ParamSetId, ADSName, new_Val.ToString());

                // Balog 20.7.2020: wenn die HMI-Sprache Deutsch war und ein Gleitkommazahl-Parameter geändert wurde, wurde das Dezimalzeichen im HMI nicht mehr angezeigt. 
                // Grund: das Dezimalzeichen wird in der Datenbank in diesem Fall als Komma gespeichert. Beim Einlesen wird allerdings immer nach einem Punkt gesucht (InvariantCulture).

                string sNewVal_tmp1 = new_Val.ToString();   
                // Wenn die HMI-Sprache Deutsch ist, ist das Dezimalzeichen eine Komma (siehe CultureInfo). 
                // Dann liefert ToString() ein String, in dem das Dezimalzeichen eine Komma ist.

                string sNewVal_tmp2 = (string)Convert.ChangeType(new_Val, typeof(string), CultureInfo.InvariantCulture);
                // Wenn man diese String-Konvertierung verwendet, wird als Dezimalzeichen sprachenunabhängig ("InvariantCulture") immer ein Punkt verwendet. 
                // So wird das Dezimalzeichen nach der Änderung eines Gleitkommazahl-Parameters auch im HMI richtig angezeigt.

                DBParam.Handler.SaveParameterInDB(ParamSetId, ADSName, sNewVal_tmp2); // die 2. Variante verwenden

                // ***** in die PLC schreiben - hier nicht
                if ((Item.iHandle > 0)
                    || ((GlobalVar.ActTypeParamSet.id == ParamSetId) || (GlobalVar.ActMachineParamSet.id == ParamSetId))) // Frank 21.10.2019 .. in PLC schreiben wenn das Paramset in der PLC aktiv ist ... wenn mehr Paramset erstellt werden => erweitern
                {
                    VarCon.WriteItem(ADSName, new_Val);
                    DBLog.Handler.Parameter(ParamSetId, sSettingName, ValDB.ToString(), new_Val.ToString(), "write to PLC");
                }

            }
        }

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ////////////////////////////////////
        /////////// Connection /////////////
        ////////////////////////////////////
        public void Register(string sName)
        {

        }
        public void RegisterPLC(string sName) /// Nur PLC
        {
            ADSName = sName;
            Item.sName = sName;

            if (typeof(T) == typeof(bool))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(bool));
                Item.Type = typeof(bool);
            }
            else if (typeof(T) == typeof(Int16))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(Int16));
                Item.Type = typeof(Int16);
            }
            else if ((typeof(T) == typeof(Int32)) || (typeof(T) == typeof(int)))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(Int32));
                Item.Type = typeof(Int32);
            }
            else if (typeof(T) == typeof(float))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(float));
                Item.Type = typeof(float);
            }
            else if (typeof(T) == typeof(double))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(double));
                Item.Type = typeof(double);
            }
            else if (typeof(T) == typeof(string))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(string));
                Item.Type = typeof(string);
            }
            else
            {
                MessageBox.Show("Typ nicht verfügbar: ", ADSName);
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }

        // Für Anzeige in Stationsseiten (immer Act Param Set) ... normal nicht für RENA
        public void Register(string sName, ParamSetTypes paramType)
        {
            ADSName = sName;
            Item.sName = sName;
            if (paramType == ParamSetTypes.Type)
                ParamSetId = GlobalVar.ActTypeParamSet.id;

            if (paramType == ParamSetTypes.Machine)
                ParamSetId = GlobalVar.ActMachineParamSet.id;

            if (typeof(T) == typeof(bool))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(bool));
                Item.Type = typeof(bool);
            }
            else if (typeof(T) == typeof(Int16))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(Int16));
                Item.Type = typeof(Int16);
            }
            else if ((typeof(T) == typeof(Int32)) || (typeof(T) == typeof(int)))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(Int32));
                Item.Type = typeof(Int32);
            }
            else if (typeof(T) == typeof(float))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(float));
                Item.Type = typeof(float);
            }
            else if (typeof(T) == typeof(double))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(double));
                Item.Type = typeof(double);
            }
            else if (typeof(T) == typeof(string))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(string));
                Item.Type = typeof(string);
            }
            else
            {
                MessageBox.Show("Typ nicht verfügbar: ", ADSName);
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;

            // Database
            if (ParamSetTypes.Type == paramType)
            {
                try
                {
                    db_parameter Param = GlobalVar.ActTypeParamList.Single(p => p.sADSName == sName);
                    // MB 3.8.2020: auskommentiert wegen DB-Modellaktualisierung
                    //Min = (T)Convert.ChangeType(Param.sMin, typeof(T));
                    //Max = (T)Convert.ChangeType(Param.sMax, typeof(T));
                    //sSettingName = Param.sHMIName;
                    //sUnit = Param.sUnit;
                    //iUserLevel = Param.iUserLevel;
                }
                catch
                {
                    MessageBox.Show("Parameter nicht in der Typparameter-DB enthalten.");
                }
            }
            else if (ParamSetTypes.Machine == paramType)
                try
                {
                    db_parameter Param = GlobalVar.ActMachineParamList.Single(p => p.sADSName == sName); //Enumerable.Single Method: Returns a single, specific element of a sequence. (e.g.:select the only element of an array that satisfies a condition)
                    // MB 3.8.2020: auskommentiert wegen DB-Modellaktualisierung
                    //Min = (T)Convert.ChangeType(Param.sMin, typeof(T));
                    //Max = (T)Convert.ChangeType(Param.sMax, typeof(T));
                    //sSettingName = Param.sHMIName;
                    //sUnit = Param.sUnit;
                    //iUserLevel = Param.iUserLevel;
                }
                catch
                {
                    MessageBox.Show("Parameter nicht in der Maschinenparameter-DB enthalten.");
                }
        }


        // Für Listenanzeige /// Nur Datenbank
        public void RegisterDB(int _ParamSetId, db_parameter _Parameter)
        {
            ADSName = _Parameter.sADSName;
            ParamSetId = _ParamSetId;

            try
            {
                //Min = (T)Convert.ChangeType(_Parameter.sMin, typeof(T));  // auskommentiert durch Balog 18.6.2020
                //Max = (T)Convert.ChangeType(_Parameter.sMax, typeof(T));  // auskommentiert durch Balog 18.6.2020
                //sSettingName = _Parameter.sHMIName;                       // auskommentiert durch Balog 18.6.2020
                //sUnit = _Parameter.sUnit;                                 // auskommentiert durch Balog 18.6.2020
                //iUserLevel = _Parameter.iUserLevel;                       // auskommentiert durch Balog 18.6.2020

                //ValDB = (T)Convert.ChangeType(_Parameter.sValue, typeof(T));

                ValDB = (T)Convert.ChangeType(_Parameter.sValue, typeof(T), CultureInfo.InvariantCulture);
                // Balog 30.6.2020: 
                //      "CultureInfo.InvariantCulture" wurde hinugefügt, damit auch das Dezimaltrennzeichen angezeigt wird.
                //      Sonst wird das Dezimaltrennzeichen weggelassen 
            }
            catch
            {

            }

            // *************************************************
            // ********************** Neu **********************
            // Balog 18.6.2020:
            /*
            ParamStaticFields paramStaticFields;
            try
            {
                //paramStaticFields = ((MainWindow)Application.Current.MainWindow).dataHOPA.listParamStaticFields.Single(r => r.sADSName == ADSName);
                //paramStaticFields = GlobalVar.dataHopa.listParamStaticFields.Single(r => r.sADSName == ADSName);

                //sSettingName    = paramStaticFields.sHMIName;
               // sDescription    = paramStaticFields.sHMIDescr;
                Min             = (T)Convert.ChangeType(paramStaticFields.sMin, typeof(T));
                Max             = (T)Convert.ChangeType(paramStaticFields.sMax, typeof(T));
                sUnit           = paramStaticFields.sUnit;
                iUserLevel      = paramStaticFields.iUserLevel;
            }
            catch
            {
                MessageBox.Show(
                    "Der folgende Parameter besitzt keine statische Parameterfelder (Name, Beschreibung, Min, Max, Unit, User level): " + "\n\n" + ADSName,
                    "Statische Parameterfelder",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);                  
            }
            */
            // *************************************************
            // *************************************************

            try
            {
                if (GlobalVar.ActUser.iUserLevel >= iUserLevel)
                    bAllowed = true;
                else
                    bAllowed = false;
            }
            catch
            { }



        }

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

            if (Item.Value != null)
            {
                if (typeof(T) == typeof(bool))
                    ValPLC = (T)Convert.ChangeType((bool)Item.Value, typeof(T));
                else if (typeof(T) == typeof(Int16))
                    ValPLC = (T)Convert.ChangeType((Int16)Item.Value, typeof(T));
                else if ((typeof(T) == typeof(Int32)) || (typeof(T) == typeof(int)))
                    ValPLC = (T)Convert.ChangeType((Int32)Item.Value, typeof(T));
                else if (typeof(T) == typeof(float))
                    ValPLC = (T)Convert.ChangeType((float)Item.Value, typeof(T));
                else if (typeof(T) == typeof(double))
                    ValPLC = (T)Convert.ChangeType((double)Item.Value, typeof(T));
                else if (typeof(T) == typeof(string))
                    ValPLC = (T)Convert.ChangeType((string)Item.Value, typeof(T));
            }

            try // Balog 19.6.2020
            { 
                if (GlobalVar.ActUser.iUserLevel >= iUserLevel)
                    bAllowed = true;
                else
                    bAllowed = false;
            }
            catch
            {
            }

        }



        public void Deregister()
        {
            if (Item.iHandle > 0)
            {
                VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
                VarCon.RemoveItem(Item);
            }
        }

        public string GetPLCValue()
        {
            if (ValPLC != null)
            {
                return ValPLC.ToString();
            }
            else
                return "Error";
        }


        public void SetPLCValue(string value)
        {
            ValPLC = (T)Convert.ChangeType(value, typeof(T));
        }

        public string GetADSName()
        {
            return ADSName;
        }

        public void SetDescription(string txtDescr)
        {
            //sDescrGerman = txtGerman;   //ggf. löschen
            //sDescrEnglish = txtEnglish; //ggf. löschen
            sDescription = txtDescr;
        }





    }
}
