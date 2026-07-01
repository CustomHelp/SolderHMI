using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Input;

namespace LOET_HMI.PLC_Com_Classes
{
    //////////////////////////////////////////
    //// MarshalAs (Strukturen in der SPS) ///
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Counter
    {
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.U2)]
        public Int16 uiStation;


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)] //Stringlänge der SPS-Struktur anpassen. Hier x+1
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string strDescription;

        [MarshalAs(UnmanagedType.U1)] // Beckhoff USINT ist 8 Bit groß
        public byte byType; 
      
        [MarshalAs(UnmanagedType.U4)] // Beckhoff UDINT ist 32 Bit groß
        public UInt32 udiTotal; 
        [MarshalAs(UnmanagedType.U4)] // Beckhoff UDINT ist 32 Bit groß
        public UInt32 udiIO;
        [MarshalAs(UnmanagedType.U4)] // Beckhoff UDINT ist 32 Bit groß
        public UInt32 udiNIO; 
        [MarshalAs(UnmanagedType.U2)] // Beckhoff: UINT ist 16 Bit groß
        public Int16 uiQuote;


        [MarshalAs(UnmanagedType.U4)] // Beckhoff UDINT ist 32 Bit groß
        public UInt32 udiAct;
        [MarshalAs(UnmanagedType.U4)] // Beckhoff UDINT ist 32 Bit groß
        public UInt32 udiTarget;

        [MarshalAs(UnmanagedType.I1)]
        public bool bEnableReset;


    }
    #endregion

    public partial class StCounter : INotifyPropertyChanged
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
        /////////////////////////////////////////////////////
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
        /////////////////////////////////////////////////////
        private eCounterType _eType;
        public eCounterType eType
        {
            get
            {
                return _eType;
            }

            set
            {
                _eType = value;
                OnPropertyChanged();
            }
        }


        private uint _uiTotal;
        public  uint uiTotal
        {
            get
            {
                return _uiTotal;
            }

            set
            {
                _uiTotal = value;
                OnPropertyChanged();
            }
        }

        private uint _uiIO;
        public uint uiIO
        {
            get
            {
                return _uiIO;
            }

            set
            {
                _uiIO = value;
                OnPropertyChanged();
            }
        }

        private uint _uiNIO;
        public uint uiNIO
        {
            get
            {
                return _uiNIO;
            }

            set
            {
                _uiNIO = value;
                OnPropertyChanged();
            }
        }


        private float _flQuote;
        public float flQuote
        {
            get
            {
                return _flQuote;
            }

            set
            {
                _flQuote = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////////////////
        private bool _bEnableReset;
        public bool bEnableReset
        {
            get
            {
                return _bEnableReset;
            }

            set
            {
                _bEnableReset = value;
                OnPropertyChanged();
            }
        }

        private uint _uiAct;
        public uint uiAct
        {
            get
            {
                return _uiAct;
            }

            set
            {
                _uiAct = value;
                OnPropertyChanged();
            }
        }

     
        private uint _uiTarget;
        public uint uiTarget
        {
            get
            {
                return _uiTarget;
            }

            set
            {
                _uiTarget = value;
                OnPropertyChanged();
            }
        }
        #endregion



        ////////////////////////////////////
        ///////////// ADS - TX /////////////
        ////////////////////////////////////
        #region
        public ICommand cmdReset
        {
            get
            {
                return new RelayCommand(() => { WriteReset(); });
            }
        }

        public void WriteReset() // übernommen vom Horsch 5.12.2019
        {
            if (ADSName != null)
            {
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bReset", true);
            }
        }

        public void WriteTargetValue(uint uiTarget)
        {
            if (ADSName != null)
            {
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.udiTarget", uiTarget);
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
        /////////// ADS - RX ///////////////
        ////////////////////////////////////
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = ADSName + ".PLC_TO_HMI";
            Item.Type = typeof(ST_Counter);

            Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_Counter));

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

            ST_Counter tmp = (ST_Counter)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iModul = tmp.uiModul;
                    iStation = tmp.uiStation;

                    sName = tmp.strName;
                    sDescription = tmp.strDescription;

                    eType = (eCounterType)tmp.byType;


                    uiTotal         = tmp.udiTotal;
                    uiIO            = tmp.udiIO;
                    uiNIO           = tmp.udiNIO;
                    flQuote     = (float)tmp.uiQuote / 10;

                    bEnableReset    = tmp.bEnableReset;
                    uiAct           = tmp.udiAct;
                    uiTarget        = tmp.udiTarget;
                }
                catch (Exception ex)
                {
                    AppLogger.Log("StCounter.ItemChanged", ex);
                }
            }
        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);
        }

        #endregion
    }
}
