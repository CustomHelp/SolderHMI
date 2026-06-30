using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LOET_HMI.PLC_Com_Classes
{
    //////////////////////////////////////////
    //// MarshalAs (Struktur in der SPS) /////
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_HU5000_PLCToHMI
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

        [MarshalAs(UnmanagedType.I1)]
        public bool bManOn;


        // Ansteuerung
        [MarshalAs(UnmanagedType.I1)]
        public bool bHF1_Ein_Control;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHF2_Ein_Control;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHF_AUS_Control;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen      In TwinCAT: INT
        public Int16 iActPower_Control;


        // Rückmeldungen
        [MarshalAs(UnmanagedType.I1)]
        public bool bHF1_Ein;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHF2_Ein;
        [MarshalAs(UnmanagedType.I1)]
        public bool bBereit;
        [MarshalAs(UnmanagedType.I1)]
        public bool bFehler;
        [MarshalAs(UnmanagedType.I1)]
        public bool bTelegrammOK;

        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen      In TwinCAT: INT
        public Int16 iActPower;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen      In TwinCAT: INT
        public Int16 iActCurrent;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen      In TwinCAT: INT
        public Int16 iActFrequence;

        // Fehlerbits
        [MarshalAs(UnmanagedType.I1)]
        public bool bErrorCooler;
        [MarshalAs(UnmanagedType.I1)]
        public bool bMinFreuqency;
        [MarshalAs(UnmanagedType.I1)]
        public bool bLimitFreqency;
        [MarshalAs(UnmanagedType.I1)]
        public bool bLimitCurrent;
        [MarshalAs(UnmanagedType.I1)]
        public bool bOverCurrent;
        [MarshalAs(UnmanagedType.I1)]
        public bool bCableCurrent;
        [MarshalAs(UnmanagedType.I1)]
        public bool bCableCurrent_1;
        [MarshalAs(UnmanagedType.I1)]
        public bool bUnderVoltage;
        [MarshalAs(UnmanagedType.I1)]
        public bool bPowerSupply;
        [MarshalAs(UnmanagedType.I1)]
        public bool bTemperature;
    }
    #endregion


    public class StHU5000 : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }

        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region
        private int _iModul;
        public int iModul
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

        private int _iStation;
        public int iStation
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

        private string _sBMK;
        public string sBMK
        {
            get
            {
                return _sBMK;
            }

            set
            {
                _sBMK = value;
                OnPropertyChanged();
            }
        }

        private string _sName;
        public string sName
        {
            get
            {
                return _sName;
            }

            set
            {
                _sName = value;
                OnPropertyChanged();
            }
        }

        private string _sDescription;
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

        private bool _bManOn;
        public bool bManOn
        {
            get
            {
                return _bManOn;
            }

            set
            {
                _bManOn = value;
                OnPropertyChanged();
            }
        }

        // **************************************
        // ************ Ansteuerung *************
        // **************************************
        private bool _bHF1_Ein_Control;
        public bool bHF1_Ein_Control
        {
            get
            {
                return _bHF1_Ein_Control;
            }

            set
            {
                _bHF1_Ein_Control = value;
                OnPropertyChanged();
            }
        }

        private bool _bHF2_Ein_Control;
        public bool bHF2_Ein_Control
        {
            get
            {
                return _bHF2_Ein_Control;
            }

            set
            {
                _bHF2_Ein_Control = value;
                OnPropertyChanged();
            }
        }

        private bool _bHF_AUS_Control;
        public bool bHF_AUS_Control
        {
            get
            {
                return _bHF_AUS_Control;
            }

            set
            {
                _bHF_AUS_Control = value;
                OnPropertyChanged();
            }
        }

        private int _iActPower_Control;
        public int iActPower_Control
        {
            get
            {
                return _iActPower_Control;
            }

            set
            {
                _iActPower_Control = value;
                OnPropertyChanged();
            }
        }


        // **************************************
        // ************ Rückmeldungen ***********
        // **************************************
        private bool _bHF1_Ein;
        public bool bHF1_Ein
        {
            get
            {
                return _bHF1_Ein;
            }

            set
            {
                _bHF1_Ein = value;
                OnPropertyChanged();
            }
        }

        private bool _bHF2_Ein;
        public bool bHF2_Ein
        {
            get
            {
                return _bHF2_Ein;
            }

            set
            {
                _bHF2_Ein = value;
                OnPropertyChanged();
            }
        }

        private bool _bBereit;
        public bool bBereit
        {
            get
            {
                return _bBereit;
            }

            set
            {
                _bBereit = value;
                OnPropertyChanged();
            }
        }

        private bool _bFehler;
        public bool bFehler
        {
            get
            {
                return _bFehler;
            }

            set
            {
                _bFehler = value;
                OnPropertyChanged();
            }
        }

        private bool _bTelegrammOK;
        public bool bTelegrammOK
        {
            get
            {
                return _bTelegrammOK;
            }

            set
            {
                _bTelegrammOK = value;
                OnPropertyChanged();
            }
        }

        private int _iActPower;
        public int iActPower
        {
            get
            {
                return _iActPower;
            }

            set
            {
                _iActPower = value;
                OnPropertyChanged();
            }
        }

        private int _iActCurrent;
        public int iActCurrent
        {
            get
            {
                return _iActCurrent;
            }

            set
            {
                _iActCurrent = value;
                OnPropertyChanged();
            }
        }

        private int _iActFrequence;
        public int iActFrequence
        {
            get
            {
                return _iActFrequence;
            }

            set
            {
                _iActFrequence = value;
                OnPropertyChanged();
            }
        }


        // **************************************
        // ************ Fehlerbits **************
        // **************************************
        private bool _bErrorCooler;
        public bool bErrorCooler
        {
            get
            {
                return _bErrorCooler;
            }

            set
            {
                _bErrorCooler = value;
                OnPropertyChanged();
            }
        }

        private bool _bMinFreuqency;
        public bool bMinFreuqency
        {
            get
            {
                return _bMinFreuqency;
            }

            set
            {
                _bMinFreuqency = value;
                OnPropertyChanged();
            }
        }

        private bool _bLimitFreqency;
        public bool bLimitFreqency
        {
            get
            {
                return _bLimitFreqency;
            }

            set
            {
                _bLimitFreqency = value;
                OnPropertyChanged();
            }
        }


        private bool _bLimitCurrent;
        public bool bLimitCurrent
        {
            get
            {
                return _bLimitCurrent;
            }

            set
            {
                _bLimitCurrent = value;
                OnPropertyChanged();
            }
        }


        private bool _bOverCurrent;
        public bool bOverCurrent
        {
            get
            {
                return _bOverCurrent;
            }

            set
            {
                _bOverCurrent = value;
                OnPropertyChanged();
            }
        }

        private bool _bCableCurrent;
        public bool bCableCurrent
        {
            get
            {
                return _bCableCurrent;
            }

            set
            {
                _bCableCurrent = value;
                OnPropertyChanged();
            }
        }

        private bool _bCableCurrent_1;
        public bool bCableCurrent_1
        {
            get
            {
                return _bCableCurrent_1;
            }

            set
            {
                _bCableCurrent_1 = value;
                OnPropertyChanged();
            }
        }

        private bool _bUnderVoltage;
        public bool bUnderVoltage
        {
            get
            {
                return _bUnderVoltage;
            }

            set
            {
                _bUnderVoltage = value;
                OnPropertyChanged();
            }
        }

        private bool _bPowerSupply;
        public bool bPowerSupply
        {
            get
            {
                return _bPowerSupply;
            }

            set
            {
                _bPowerSupply = value;
                OnPropertyChanged();
            }
        }

        private bool _bTemperature;
        public bool bTemperature
        {
            get
            {
                return _bTemperature;
            }

            set
            {
                _bTemperature = value;
                OnPropertyChanged();
            }
        }
        #endregion

        ////////////////////////////////////
        ///////////// Konstruktor //////////
        ////////////////////////////////////
        #region

        #endregion

        ////////////////////////////////////
        ///////////// ADS - TX /////////////
        ////////////////////////////////////
        #region

        #endregion


        ////////////////////////////////////
        /////////// ADS - RX ///////////////
        ////////////////////////////////////
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = ADSName + ".PLC_TO_HMI";
            Item.Type = typeof(ST_HU5000_PLCToHMI);
            Item = VarCon.AddItem(Item.sName, typeof(ST_HU5000_PLCToHMI));


            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }

        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (Item.iHandle == e.Item[j].iHandle)
                {
                    Item = e.Item[j];
                }
            }


            ST_HU5000_PLCToHMI tmp = (ST_HU5000_PLCToHMI)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iModul = tmp.ushModul;
                    iStation = tmp.ushModul;

                    sBMK = tmp.strBMK;
                    sName = tmp.strName;
                    sDescription = tmp.strDescription;

                    bManOn = tmp.bManOn;

                    bHF1_Ein_Control	= tmp.bHF1_Ein_Control    ;
                    bHF2_Ein_Control	= tmp.bHF2_Ein_Control    ;
                    bHF_AUS_Control		= tmp.bHF_AUS_Control	    ;
                    iActPower_Control   = tmp.iActPower_Control    ;
                                                               
                    bHF1_Ein		    = tmp.bHF1_Ein		     ;
                    bHF2_Ein		    = tmp.bHF2_Ein		     ;
                    bBereit			    = tmp.bBereit			     ;
                    bFehler			    = tmp.bFehler			     ;
                    bTelegrammOK        = tmp.bTelegrammOK         ;
                                                               
                    iActPower           = tmp.iActPower            ;
                    iActCurrent         = tmp.iActCurrent          ;
                    iActFrequence       = tmp.iActFrequence        ;
                                                               
                    bErrorCooler        = tmp.bErrorCooler         ;
                    bMinFreuqency       = tmp.bMinFreuqency        ;
                    bLimitFreqency      = tmp.bLimitFreqency       ;
                    bLimitCurrent       = tmp.bLimitCurrent        ;
                    bOverCurrent        = tmp.bOverCurrent         ;
                    bCableCurrent       = tmp.bCableCurrent        ;
                    bCableCurrent_1     = tmp.bCableCurrent_1      ;
                    bUnderVoltage       = tmp.bUnderVoltage        ;
                    bPowerSupply        = tmp.bPowerSupply         ;
                    bTemperature        = tmp.bTemperature;

                }
                catch
                {

                }
            }


        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);
        }
        #endregion


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
