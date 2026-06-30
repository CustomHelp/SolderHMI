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


namespace HOPA_HMI.PLC_Com_Classes
{

    /*public class SettingToHMIclass
    {
        public IStPLCSettingWithDB SettingStruct { get; set; }
        public string RegistrationName { get; set; }

        public SettingToHMIclass(IStPLCSettingWithDB st_setting, string name)
        {
            SettingStruct = new StPLCSetting<IStPLCSettingWithDB>();
            SettingStruct = st_setting;

            RegistrationName = name;

        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_DINT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int diMax;
        [MarshalAs(UnmanagedType.I4)]
        public int diMin;
        [MarshalAs(UnmanagedType.I4)]
        public int val;

        [MarshalAs(UnmanagedType.I1)]
        public bool Allowed;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string sSettingName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sUnit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_REAL
    {
        [MarshalAs(UnmanagedType.R4)]
        public float rMax;
        [MarshalAs(UnmanagedType.R4)]
        public float rMin;
        [MarshalAs(UnmanagedType.R4)]
        public float val;

        [MarshalAs(UnmanagedType.I1)]
        public bool Allowed;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string sSettingName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sUnit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_LREAL
    {
        [MarshalAs(UnmanagedType.R8)]
        public double rlMax;
        [MarshalAs(UnmanagedType.R8)]
        public double rlMin;
        [MarshalAs(UnmanagedType.R8)]
        public double val;

        [MarshalAs(UnmanagedType.I1)]
        public bool Allowed;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string sSettingName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sUnit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Setting_BOOL
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool val;

        [MarshalAs(UnmanagedType.I1)]
        public bool Allowed;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string sSettingName;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        // public string sUnit;
    }
    */







    public partial class StPLCSetting<T> : INotifyPropertyChanged, IStPLCSettingWithDB
    {
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

        // ****************** Balog 6.2.2020
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


        ////////////////////////////////////
        ///////////// Commands /////////////
        ////////////////////////////////////


        // WriteVal wird dem generischen Typ T angepasst:
        public void WriteVal(T new_Val)
        {
            // Wenn ParamSetId < 0 ist, dann ist kein Parameterset geladen => es kann/braucht nichts geschrieben zu werden
            // Log schreiben
            if (ParamSetId >= 0)
            {
                DBLog.Handler.Parameter(ParamSetId, sSettingName, ValDB.ToString(), new_Val.ToString(), "store to DB");
                // in DB speichern
                DBParam.Handler.SaveParameterInDB(ParamSetId, ADSName, new_Val.ToString());
            }
            // in die PLC schreiben - hier nicht
            if (Item.iHandle > 0)
            {
                // in die PLC schreiben
                VarCon.WriteItem(ADSName + ".val", new_Val);
                DBLog.Handler.Parameter(ParamSetId, sSettingName, ValDB.ToString(), new_Val.ToString(), "write to PLC");
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

        public void Register(string sName, ParamSetTypes paramSets)
        {
            RegisterPLC(sName);
        }

        public void RegisterPLC(string sName)
        {
            ADSName = sName;
            Item.sName = sName;
            ParamSetId = -1;

            if (typeof(T) == typeof(bool))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(ST_Setting_BOOL));
                Item.Type = typeof(ST_Setting_BOOL);
            }

            else if (typeof(T) == typeof(Int32))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(ST_Setting_DINT));
                Item.Type = typeof(ST_Setting_DINT);
            }

            else if (typeof(T) == typeof(double))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(ST_Setting_LREAL));
                Item.Type = typeof(ST_Setting_LREAL);
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }

        public void RegisterDB(int _ParamSetId, db_parameter _Parameter)
        { }

        bool bValueChanged = true;
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
            bValueChanged = true;
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
                            //ValPLC = (T)Convert.ChangeType(tmp1.val, typeof(T));

                            //bAllowed = tmp1.Allowed;

                            sSettingName = tmp1.strName;
                            sUnit = "";

                            bValueChanged = false;
                        }
                        catch
                        { }
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
                            //Min = (T)Convert.ChangeType(tmp2.val, typeof(T));
                            //ValPLC = (T)Convert.ChangeType(tmp2.val, typeof(T));

                            //bAllowed = tmp2.Allowed;

                            sSettingName = tmp2.strName;
                            sUnit = tmp2.strUnit;

                            bValueChanged = false;
                        }
                        catch
                        { }
                    }
                }
              
                else if (typeof(T) == typeof(double))
                {
                    ST_Setting_LREAL tmp4 = (ST_Setting_LREAL)Item.Value;
                    if (tmp4 != null)
                    {
                        try
                        {
                            Max = (T)Convert.ChangeType(tmp4.maxVal, typeof(T));
                            Min = (T)Convert.ChangeType(tmp4.minVal, typeof(T));
                            //ValPLC = (T)Convert.ChangeType(tmp4.val, typeof(T));

                            

                            sSettingName = tmp4.strName;
                            sUnit = tmp4.strUnit;

                            bValueChanged = false;
                        }
                        catch
                        { }
                    }
                }
            }

        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);
        }

        public string GetPLCValue()
        {
            return ValPLC.ToString();
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
