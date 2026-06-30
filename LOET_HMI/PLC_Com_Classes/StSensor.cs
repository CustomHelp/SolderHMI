using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;


namespace LOET_HMI
{
    //////////////////////////////////////////
    //// MarshalAs (Strukturen in der SPS) ///
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Sensor_LREAL_PLCToHMI
    {
        [MarshalAs(UnmanagedType.R8)]
        public double val;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sUnit;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 iState;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string sName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sElectrDescr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sRIDesignation;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string sFunction;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Sensor_BOOL_PLCToHMI
    {
        [MarshalAs(UnmanagedType.U2)] // Beckhoff UINT: 16 Bit;     C# ushort: Unsignet 16-Bit integer  -> diese verwenden???
        public ushort ushModul;
        [MarshalAs(UnmanagedType.U2)]
        public ushort ushStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string strBMK;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string strDescription;

        // ENUM_Colour_Sensor vom Typ DWORD -> Beckhoff DWORD: 32 Bit (Wertbereich: 0...4294967295)   -> UInt32
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 uiLampColour;

        [MarshalAs(UnmanagedType.I1)]
        public bool val;
    }
    #endregion



    ////////////////////////////////////////
    ////////////// Interface  //////////////
    ////////////////////////////////////////
    public interface IStSensor
    {
        void Register(string sName);
        void Deregister();
    }


    public partial class StSensor<T> : INotifyPropertyChanged, IStSensor
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }

        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region
        private int _iModulHMI;
        public int iModulHMI
        {
            get { return _iModulHMI; }
            set
            {
                _iModulHMI = value;
                OnPropertyChanged();
            }
        }

        private int _iStationHMI;
        public int iStationHMI
        {
            get { return _iStationHMI; }
            set
            {
                _iStationHMI = value;
                OnPropertyChanged();
            }
        }
        ///////////////////////////////////// 
        ///
        private string _strBMK_HMI;
        public string strBMK_HMI
        {
            get { return _strBMK_HMI; }
            set
            {
                _strBMK_HMI = value;
                OnPropertyChanged();
            }
        }

        private string _strNameHMI;
        public string strNameHMI
        {
            get { return _strNameHMI; }
            set
            {
                _strNameHMI = value;
                OnPropertyChanged();
            }
        }

        private string _strDescriptionHMI;
        public string strDescriptionHMI
        {
            get { return _strDescriptionHMI; }
            set
            {
                _strDescriptionHMI = value;
                OnPropertyChanged();
            }
        }
        /////////////////////////////////////


        private T _ValHMI;
        public T ValHMI
        {
            get
            {
                return _ValHMI;
            }

            set
            {
                _ValHMI = value;
                OnPropertyChanged();
            }
        }

        private UInt32 _uiLampColourHMI;
        public UInt32 uiLampColourHMI
        {
            get { return _uiLampColourHMI; }
            set
            {
                _uiLampColourHMI = value;
                OnPropertyChanged();
            }
        }




        /// /////////////////////////////////////////////////////
        private string _sUnitHMI;
        public string sUnitHMI
        {
            get
            {
                return _sUnitHMI;
            }

            set
            {
                _sUnitHMI = value;
                OnPropertyChanged();
            }
        }


        #endregion




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
            ADSName = sName;
            Item.sName = sName;

            if (typeof(T) == typeof(bool))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(ST_Sensor_BOOL_PLCToHMI));
                Item.Type = typeof(ST_Sensor_BOOL_PLCToHMI);
            }


            else if (typeof(T) == typeof(double))
            {
                Item = VarCon.AddItem(ADSName + "", typeof(ST_Sensor_LREAL_PLCToHMI));
                Item.Type = typeof(ST_Sensor_LREAL_PLCToHMI);
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }

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
                    ST_Sensor_BOOL_PLCToHMI tmp1 = (ST_Sensor_BOOL_PLCToHMI)Item.Value;
                    if (tmp1 != null)
                    {
                        try
                        {

                            ValHMI = (T)Convert.ChangeType(tmp1.val, typeof(T));
                            

                            iModulHMI           = tmp1.ushModul;
                            iStationHMI         = tmp1.ushStation;
                            strBMK_HMI          = tmp1.strBMK;
                            strNameHMI          = tmp1.strName;
                            strDescriptionHMI   = tmp1.strDescription;
                            uiLampColourHMI      = tmp1.uiLampColour;

                            sUnitHMI = "---";
            
                            bValueChanged = false;
                        }
                        catch
                        { }
                    }

                }

                else if (typeof(T) == typeof(double))
                {
                    ST_Sensor_LREAL_PLCToHMI tmp4 = (ST_Sensor_LREAL_PLCToHMI)Item.Value;
                    if (tmp4 != null)
                    {
                        try
                        {
                            //ValHMI = (T)Convert.ChangeType(tmp4.val, typeof(T));
                            //sUnitHMI = tmp4.sUnit;


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
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
                VarCon.RemoveItem(Item);
            }));
        }
    }
}
