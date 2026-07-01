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
    public class ST_PF_RFID_IND_PLCToHMI
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
        public bool bWriteBusy;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte01;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte02;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte03;



        [MarshalAs(UnmanagedType.I1)]
        public bool bWriteAllowed;
        [MarshalAs(UnmanagedType.I1)] // VORSICHT: diese "Filler-Bytes" sind wichtig wegen Memory-Alignment https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_plc_intro/3539428491.html&id=
        public bool bFillerByte10;
        [MarshalAs(UnmanagedType.I1)]
        public bool bFillerByte11;
        [MarshalAs(UnmanagedType.I1)]
        public bool bFillerByte12;


        //ST_PF_RFID_IND_PLCToHMI.ST_Induktor_Data
        [MarshalAs(UnmanagedType.I1)]
        public bool bRealised;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDataVersion;
        [MarshalAs(UnmanagedType.I4)] // I4: 4-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: DINT
        public Int32 diInduktorNummber;
    }
    #endregion

    public class StRFID_IND : INotifyPropertyChanged
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

        private bool _bWriteBusy;
        public bool bWriteBusy
        {
            get
            {
                return _bWriteBusy;
            }

            set
            {
                _bWriteBusy = value;
                OnPropertyChanged();
            }
        }

        private bool _bWriteAllowed;
        public bool bWriteAllowed
        {
            get
            {
                return _bWriteAllowed;
            }

            set
            {
                _bWriteAllowed = value;
                OnPropertyChanged();
            }
        }

        private StInduktor_Data _stInduktor_Data;
        public StInduktor_Data stInduktor_Data
        {
            get
            {
                return _stInduktor_Data;
            }

            set
            {
                _stInduktor_Data = value;
                OnPropertyChanged();
            }
        }
        #endregion


        ////////////////////////////////////
        ///////////// Konstruktor //////////
        ////////////////////////////////////
        #region
        public StRFID_IND()
        {
            stInduktor_Data = new StInduktor_Data();
        }
        #endregion

        //////////////////////////////////////////
        // Subkklassen (Sub-Strukturen TwinCAT) // 
        //////////////////////////////////////////
        #region
        public class StInduktor_Data : INotifyPropertyChanged
        {
            private bool _bRealised;
            public bool bRealised
            {
                get
                {
                    return _bRealised;
                }

                set
                {
                    _bRealised = value;
                    OnPropertyChanged();
                }
            }



            private byte _byDataVersion;
            public byte byDataVersion
            {
                get
                {
                    return _byDataVersion;
                }

                set
                {
                    _byDataVersion = value;
                    OnPropertyChanged();
                }
            }

            private int _iInduktorNummber;
            public int iInduktorNumber
            {
                get
                {
                    return _iInduktorNummber;
                }

                set
                {
                    _iInduktorNummber = value;
                    OnPropertyChanged();
                }
            }


            // Create the OnPropertyChanged method to raise the event
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion

        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region

        #endregion

        ////////////////////////////////////
        ///////////// ADS - TX /////////////
        ////////////////////////////////////
        #region 
        public void WriteDataToPLC()
        {
            if (ADSName != null)
            {


                string strPath = ".HMI_TO_PLC.stData_Write.";

                VarCon.WriteItem(ADSName + strPath + "diInduktorNummber", stInduktor_Data.iInduktorNumber);

                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bWriteData", true);

            }

        }
        #endregion


        ////////////////////////////////////
        /////////// ADS - RX ///////////////
        ////////////////////////////////////
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = ADSName + ".PLC_TO_HMI";
            Item.Type = typeof(ST_PF_RFID_IND_PLCToHMI);
            Item = VarCon.AddItem(Item.sName, typeof(ST_PF_RFID_IND_PLCToHMI));


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


            ST_PF_RFID_IND_PLCToHMI tmp = (ST_PF_RFID_IND_PLCToHMI)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iModul = tmp.ushModul;
                    iStation = tmp.ushModul;

                    sBMK = tmp.strBMK;
                    sName = tmp.strName;
                    sDescription = tmp.strDescription;

                    bWriteBusy = tmp.bWriteBusy;
                    bWriteAllowed = tmp.bWriteAllowed;

                    stInduktor_Data.bRealised = tmp.bRealised;
                    stInduktor_Data.byDataVersion = tmp.byDataVersion;
                    stInduktor_Data.iInduktorNumber = tmp.diInduktorNummber;
                    
                }
                catch (Exception ex)
                {
                    AppLogger.Log("StRFID_IND.ItemChanged", ex);
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
