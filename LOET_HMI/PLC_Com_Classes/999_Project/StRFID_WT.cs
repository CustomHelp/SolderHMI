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
    public class ST_PF_RFID_WT_PLCToHMI
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

        [MarshalAs(UnmanagedType.I1)]
        public bool bWriteAllowed;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_WT_Data
    {
        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data
        [MarshalAs(UnmanagedType.I1)]
        public bool bRealised;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDataVersion;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byWTNummber;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Variant
    {
        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data.ST_Variant
        [MarshalAs(UnmanagedType.I1)]
        public bool bEnabled;

        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data.ST_Variant.ST_KSIdent
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string ident;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben1; // MBA 11.05.2021: es muss aktuell nicht von der SPS ausgelesen werden
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben2;// MBA 11.05.2021: es muss aktuell nicht von der SPS ausgelesen werden

        

        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fTempMelt;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fTempSolder;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fTempTune;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fTempRelease;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fTempOffset;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fPowerPreCool;
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fPowerCool;

        [MarshalAs(UnmanagedType.U2)] // U4: Unsigned integer        In TwinCAT: UINT
        public ushort uiCoolingType;

        [MarshalAs(UnmanagedType.I1)]
        public bool bDVS1_Enabled;
        [MarshalAs(UnmanagedType.I1)]
        public bool bDVS2_Enabled;
        [MarshalAs(UnmanagedType.I1)]
        public bool bDVS3_Enabled;

        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte01;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte02;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte03;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte04;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte05;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte06;

        [MarshalAs(UnmanagedType.I1)]
        public bool bClamp1_Enabled;
        [MarshalAs(UnmanagedType.I1)]
        public bool bClamp2_Enabled;

        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte07;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte08;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte09;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte10;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte11;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte12;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bFillerByte13;

        [MarshalAs(UnmanagedType.I1)]
        public bool bWithMeasurment;
        [MarshalAs(UnmanagedType.U2)] // U4: Unsigned integer        In TwinCAT: UINT
        public ushort ui_Reheat;

        [MarshalAs(UnmanagedType.I1)]
        public bool bPreCoolON;
        [MarshalAs(UnmanagedType.I1)]
        public bool bCoolOn;

        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byOverride;
        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data.ST_Variant.ST_CHP_DVS
        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fDVS1FeedLength;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS1FeedSpeed;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS1DrawbackLength;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte14;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte15;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte16;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte17;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte18;

        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fDVS2FeedLength;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS2FeedSpeed;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS2DrawbackLength;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte19;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte20;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte21;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte22;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte23;

        [MarshalAs(UnmanagedType.R4)]// R4: 4-Byte-Gleitkommazahl                   In TwinCAT: REAL
        public float fDVS3FeedLength;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS3FeedSpeed;
        [MarshalAs(UnmanagedType.U1)] // U1: 1-Byte-Ganzzahl ohne Vorzeichen        In TwinCAT: BYTE
        public byte byDVS3DrawbackLength;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte24;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte25;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte26;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte27;
        //[MarshalAs(UnmanagedType.U1)]
        //public byte byFillerByte28;

        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data.ST_Variant
        //[MarshalAs(UnmanagedType.I4)] // I4: 4-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: DINT
        //public Int32 diLegalInduktor;

        // ST_PF_RFID_WT_PLCToHMI.ST_WT_Data.ST_Variant

    }
    #endregion





    public partial class StRFID_WT : INotifyPropertyChanged
    {
        public const int iNrOfVariants = 5;

        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item1_RFID_WT = new ADSItem();
        private ADSItem Item2_WT_Data = new ADSItem();
        //private ADSItem Item31_Variant= new ADSItem();
        private ADSItem[] aItem3_Variant = new ADSItem[iNrOfVariants];

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

        ///////////////////////////////////////////////////
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

        private StWT_DATA _stWT_Data;
        public StWT_DATA stWT_Data
        {
            get
            {
                return _stWT_Data;
            }

            set
            {
                _stWT_Data = value;
                OnPropertyChanged();
            }
        }
        #endregion



        //////////////////////////////////////////
        // Subkklassen (Sub-Strukturen TwinCAT) // 
        //////////////////////////////////////////
        #region
        public class StWT_DATA : INotifyPropertyChanged
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

            private byte _byWTNummber;
            public byte byWTNummber
            {
                get
                {
                    return _byWTNummber;
                }

                set
                {
                    _byWTNummber = value;
                    OnPropertyChanged();
                }
            }

            private StVariant[] _stVariant;
            public StVariant[] stVariant
            {
                get
                {
                    return _stVariant;
                }

                set
                {
                    _stVariant = value;
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
        public class StVariant : INotifyPropertyChanged
        {

            public StVariant(int _iVariantID)
            {
                iVariantID_HMI = _iVariantID;
            }


            private int _iVariantID_HMI;
            public int iVariantID_HMI
            {
                get
                {
                    return _iVariantID_HMI;
                }

                set
                {
                    _iVariantID_HMI = value;
                    OnPropertyChanged();
                }
            }




            private bool _bEnabled;
            public bool bEnabled
            {
                get
                {
                    return _bEnabled;
                }

                set
                {
                    _bEnabled = value;
                    OnPropertyChanged();
                }
            }

            private string _strIdent;
            public string strIdent
            {
                get
                {
                    return _strIdent;
                }

                set
                {
                    _strIdent = value;
                    OnPropertyChanged();
                }
            }

            private string _strEXTENSION;
            public string strEXTENSION
            {
                get
                {
                    return _strEXTENSION;
                }

                set
                {
                    _strEXTENSION = value;
                    OnPropertyChanged();
                }
            }

            private int _iLegalInduktor;
            public int iLegalInduktor
            {
                get
                {
                    return _iLegalInduktor;
                }

                set
                {
                    _iLegalInduktor = value;
                    OnPropertyChanged();
                }
            }

            private float _fTempMelt;
            public float fTempMelt
            {
                get
                {
                    return _fTempMelt;
                }

                set
                {
                    _fTempMelt = value;
                    OnPropertyChanged();
                }
            }

            private float _fTempSolder;
            public float fTempSolder
            {
                get
                {
                    return _fTempSolder;
                }

                set
                {
                    _fTempSolder = value;
                    OnPropertyChanged();
                }
            }

            private float _fTempTune;
            public float fTempTune
            {
                get
                {
                    return _fTempTune;
                }

                set
                {
                    _fTempTune = value;
                    OnPropertyChanged();
                }
            }

            private float _fTempRelease;
            public float fTempRelease
            {
                get
                {
                    return _fTempRelease;
                }

                set
                {
                    _fTempRelease = value;
                    OnPropertyChanged();
                }
            }

            private float _fTempOffset;
            public float fTempOffset
            {
                get
                {
                    return _fTempOffset;
                }

                set
                {
                    _fTempOffset = value;
                    OnPropertyChanged();
                }
            }

            private float _fPowerPreCool;
            public float fPowerPreCool
            {
                get
                {
                    return _fPowerPreCool;
                }

                set
                {
                    _fPowerPreCool = value;
                    OnPropertyChanged();
                }
            }

            private float _fPowerCool;
            public float fPowerCool
            {
                get
                {
                    return _fPowerCool;
                }

                set
                {
                    _fPowerCool = value;
                    OnPropertyChanged();
                }
            }

            private uint _uiCoolingType;
            public uint uiCoolingType
            {
                get
                {
                    return _uiCoolingType;
                }

                set
                {
                    _uiCoolingType  = value;
                    OnPropertyChanged();
                }
            }


            private uint _uiReheatTime;
            public uint uiReheatTime
            {
                get
                {
                    return _uiReheatTime;
                }

                set
                {
                    _uiReheatTime = value;
                    OnPropertyChanged();
                }
            }



            private bool _bCopperMeasurement;
            public bool bCopperMeasurement
            {
                get
                {
                    return _bCopperMeasurement;
                }

                set
                {
                    _bCopperMeasurement = value;
                    OnPropertyChanged();
                }
            }


            private bool _bDVS1Enabled;
            public bool bDVS1Enabled
            {
                get
                {
                    return _bDVS1Enabled;
                }

                set
                {
                    _bDVS1Enabled = value;
                    OnPropertyChanged();
                }
            }


            private bool _bDVS2Enabled;
            public bool bDVS2Enabled
            {
                get
                {
                    return _bDVS2Enabled;
                }

                set
                {
                    _bDVS2Enabled = value;
                    OnPropertyChanged();
                }
            }

            private bool _bDVS3Enabled;
            public bool bDVS3Enabled
            {
                get
                {
                    return _bDVS3Enabled;
                }

                set
                {
                    _bDVS3Enabled = value;
                    OnPropertyChanged();
                }
            }

            private bool _bClamp1Enabled;
            public bool bClamp1Enabled
            {
                get
                {
                    return _bClamp1Enabled;
                }

                set
                {
                    _bClamp1Enabled = value;
                    OnPropertyChanged();
                }
            }

            private bool _bClamp2Enabled;
            public bool bClamp2Enabled
            {
                get
                {
                    return _bClamp2Enabled;
                }

                set
                {
                    _bClamp2Enabled = value;
                    OnPropertyChanged();
                }
            }

            private bool _bCoolOn;
            public bool bCoolOn
            {
                get
                {
                    return _bCoolOn;
                }

                set
                {
                    _bCoolOn = value;
                    OnPropertyChanged();
                }
            }

            private bool _bPreCoolOn;
            public bool bPreCoolOn
            {
                get
                {
                    return _bPreCoolOn;
                }

                set
                {
                    _bPreCoolOn = value;
                    OnPropertyChanged();
                }
            }

            private float _fDVS1FeedLength;
            public float fDVS1FeedLength
            {
                get
                {
                    return _fDVS1FeedLength;
                }

                set
                {
                    _fDVS1FeedLength = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS1FeedSpeed;
            public byte byDVS1FeedSpeed
            {
                get
                {
                    return _byDVS1FeedSpeed;
                }

                set
                {
                    _byDVS1FeedSpeed = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS1DrawbackLength;
            public byte byDVS1DrawbackLength
            {
                get
                {
                    return _byDVS1DrawbackLength;
                }

                set
                {
                    _byDVS1DrawbackLength = value;
                    OnPropertyChanged();
                }
            }


            private float _fDVS2FeedLength;
            public float fDVS2FeedLength
            {
                get
                {
                    return _fDVS2FeedLength;
                }

                set
                {
                    _fDVS2FeedLength = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS2FeedSpeed;
            public byte byDVS2FeedSpeed
            {
                get
                {
                    return _byDVS2FeedSpeed;
                }

                set
                {
                    _byDVS2FeedSpeed = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS2DrawbackLength;
            public byte byDVS2DrawbackLength
            {
                get
                {
                    return _byDVS2DrawbackLength;
                }

                set
                {
                    _byDVS2DrawbackLength = value;
                    OnPropertyChanged();
                }
            }


            private float _fDVS3FeedLength;
            public float fDVS3FeedLength
            {
                get
                {
                    return _fDVS3FeedLength;
                }

                set
                {
                    _fDVS3FeedLength = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS3FeedSpeed;
            public byte byDVS3FeedSpeed
            {
                get
                {
                    return _byDVS3FeedSpeed;
                }

                set
                {
                    _byDVS3FeedSpeed = value;
                    OnPropertyChanged();
                }
            }

            private byte _byDVS3DrawbackLength;
            public byte byDVS3DrawbackLength
            {
                get
                {
                    return _byDVS3DrawbackLength;
                }

                set
                {
                    _byDVS3DrawbackLength = value;
                    OnPropertyChanged();
                }
            }

            private byte _byOverride;
            public byte byOverride
            {
                get
                {
                    return _byOverride;
                }

                set
                {
                    _byOverride = value;
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

        ////////////////////////////////////
        ///////////// Konstruktor //////////
        ////////////////////////////////////
        #region
        public StRFID_WT()
        {
            stWT_Data = new StWT_DATA();
            stWT_Data.stVariant = new StVariant[iNrOfVariants];

            for (int i = 0; i < iNrOfVariants; i++)
            {
                stWT_Data.stVariant[i] = new StVariant(i + 1);
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


                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.stData_Write.byWTNummber", stWT_Data.byWTNummber);

                
                for (int i = 0; i < iNrOfVariants; i++)
                {
                    string strPathVariant = ".HMI_TO_PLC.stData_Write.aVariant[" + (i + 1).ToString() + "].";
                    // Strings:
                    if(stWT_Data.stVariant[i].strIdent != ""      &&  stWT_Data.stVariant[i].strIdent != null)
                        VarCon.WriteItem(ADSName + strPathVariant + "stKSIDENT.ident", stWT_Data.stVariant[i].strIdent);
                    else
                        VarCon.WriteItem(ADSName + strPathVariant + "stKSIDENT.ident", "");



                    // Gleitkommazahlen:
                    VarCon.WriteItem(ADSName + strPathVariant + "rlTempMelt",       stWT_Data.stVariant[i].fTempMelt           );
                    VarCon.WriteItem(ADSName + strPathVariant + "rlTempSolder",     stWT_Data.stVariant[i].fTempSolder         );
                    VarCon.WriteItem(ADSName + strPathVariant + "rlTempTune",       stWT_Data.stVariant[i].fTempTune           );
                    VarCon.WriteItem(ADSName + strPathVariant + "rlTempRelease",    stWT_Data.stVariant[i].fTempRelease        );
                    VarCon.WriteItem(ADSName + strPathVariant + "rlTempOffset",     stWT_Data.stVariant[i].fTempOffset         );
                    //VarCon.WriteItem(ADSName + strPathVariant + "diLegalInduktor",  stWT_Data.stVariant[i].iLegalInduktor     );
                    VarCon.WriteItem(ADSName + strPathVariant + "ui_Reheat", stWT_Data.stVariant[i].uiReheatTime);
                    VarCon.WriteItem(ADSName + strPathVariant + "bWithMeasurment", stWT_Data.stVariant[i].bCopperMeasurement);


                    VarCon.WriteItem(ADSName + strPathVariant + "rlPowerPreCool",   stWT_Data.stVariant[i].fPowerPreCool       );
                    VarCon.WriteItem(ADSName + strPathVariant + "rlPowerCool",      stWT_Data.stVariant[i].fPowerCool          );

                    VarCon.WriteItem(ADSName + strPathVariant + "byOverride",       stWT_Data.stVariant[i].byOverride          );

                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[1].rlFeedLength",     stWT_Data.stVariant[i].fDVS1FeedLength    );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[1].byFeedSpeed",      stWT_Data.stVariant[i].byDVS1FeedSpeed     );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[1].byDrawbackLength", stWT_Data.stVariant[i].byDVS1DrawbackLength);

                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[2].rlFeedLength",     stWT_Data.stVariant[i].fDVS2FeedLength    );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[2].byFeedSpeed",      stWT_Data.stVariant[i].byDVS2FeedSpeed     );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[2].byDrawbackLength", stWT_Data.stVariant[i].byDVS2DrawbackLength);

                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[3].rlFeedLength",     stWT_Data.stVariant[i].fDVS3FeedLength    );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[3].byFeedSpeed",      stWT_Data.stVariant[i].byDVS3FeedSpeed     );
                    VarCon.WriteItem(ADSName + strPathVariant + "stDVS_Profil[3].byDrawbackLength", stWT_Data.stVariant[i].byDVS3DrawbackLength);

                    // Boolsche Variablen
                    VarCon.WriteItem(ADSName + strPathVariant + "bEnabled", stWT_Data.stVariant[i].bEnabled);

                    VarCon.WriteItem(ADSName + strPathVariant + "aDVS_Enabled[1]", stWT_Data.stVariant[i].bDVS1Enabled);
                    VarCon.WriteItem(ADSName + strPathVariant + "aDVS_Enabled[2]", stWT_Data.stVariant[i].bDVS2Enabled);
                    VarCon.WriteItem(ADSName + strPathVariant + "aDVS_Enabled[3]", stWT_Data.stVariant[i].bDVS3Enabled);

                    VarCon.WriteItem(ADSName + strPathVariant + "aClamp_Enabled[1]", stWT_Data.stVariant[i].bClamp1Enabled);
                    VarCon.WriteItem(ADSName + strPathVariant + "aClamp_Enabled[2]", stWT_Data.stVariant[i].bClamp2Enabled);

                    VarCon.WriteItem(ADSName + strPathVariant + "bPreCoolON", stWT_Data.stVariant[i].bPreCoolOn);
                    VarCon.WriteItem(ADSName + strPathVariant + "bCoolOn", stWT_Data.stVariant[i].bCoolOn);
                }


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
            Item1_RFID_WT.sName = ADSName + ".PLC_TO_HMI";
            Item1_RFID_WT.Type = typeof(ST_PF_RFID_WT_PLCToHMI);
            Item1_RFID_WT = VarCon.AddItem(Item1_RFID_WT.sName, typeof(ST_PF_RFID_WT_PLCToHMI));

            Item2_WT_Data.sName = Item1_RFID_WT.sName + ".stData_Read";
            Item2_WT_Data.Type = typeof(ST_WT_Data);
            Item2_WT_Data = VarCon.AddItem(Item2_WT_Data.sName, typeof(ST_WT_Data));

            for(int i=0; i< iNrOfVariants; i++)
            {
                aItem3_Variant[i] = new ADSItem();
                aItem3_Variant[i].sName = Item2_WT_Data.sName + ".aVariant[" + (i+1).ToString() + "]";
                aItem3_Variant[i].Type = typeof(ST_Variant);
                aItem3_Variant[i] = VarCon.AddItem(aItem3_Variant[i].sName, typeof(ST_Variant));
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_Item1ChangeEvent;
        }


        private void VarCon_Item1ChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (Item1_RFID_WT.iHandle == e.Item[j].iHandle)
                {
                    Item1_RFID_WT = e.Item[j];
                }
                if (Item2_WT_Data.iHandle == e.Item[j].iHandle)
                {
                    Item2_WT_Data = e.Item[j];
                }
                for (int i = 0; i < iNrOfVariants; i++)
                {
                    if(aItem3_Variant[i].iHandle == e.Item[j].iHandle)
                    {
                        aItem3_Variant[i] = e.Item[j];
                    }
                }
            }

            ST_PF_RFID_WT_PLCToHMI tmpRFID_WT   = (ST_PF_RFID_WT_PLCToHMI)Item1_RFID_WT.Value;
                ST_WT_Data tmpWT_Data               = (ST_WT_Data)Item2_WT_Data.Value;

            if (tmpRFID_WT != null)
            {
                try
                {
                    iModul = tmpRFID_WT.ushModul;
                    iStation = tmpRFID_WT.ushModul;

                    sBMK = tmpRFID_WT.strBMK;
                    sName = tmpRFID_WT.strName;
                    sDescription = tmpRFID_WT.strDescription;

                    bWriteBusy = tmpRFID_WT.bWriteBusy;
                    bWriteAllowed = tmpRFID_WT.bWriteAllowed;
                }
                catch
                {   ; }
            }

            if (tmpWT_Data != null)
            {
                try
                {
                    stWT_Data.bRealised = tmpWT_Data.bRealised;
                    stWT_Data.byDataVersion = tmpWT_Data.byDataVersion;
                    stWT_Data.byWTNummber = tmpWT_Data.byWTNummber;
                }
                catch
                {; }
            }

            for(int i = 0; i < aItem3_Variant.Length; i++)
            {
                if (aItem3_Variant[i] != null)
                {
                    ST_Variant tmpVariant = (ST_Variant)aItem3_Variant[i].Value;

                    try
                    {
                        stWT_Data.stVariant[i].bEnabled = tmpVariant.bEnabled;
                        stWT_Data.stVariant[i].strIdent = tmpVariant.ident;

                        //stWT_Data.stVariant[i].iLegalInduktor = tmpVariant.diLegalInduktor;

                        stWT_Data.stVariant[i].fTempMelt = tmpVariant.fTempMelt;
                        stWT_Data.stVariant[i].fTempSolder = tmpVariant.fTempSolder;
                        stWT_Data.stVariant[i].fTempTune = tmpVariant.fTempTune;
                        stWT_Data.stVariant[i].fTempRelease = tmpVariant.fTempRelease;
                        stWT_Data.stVariant[i].fTempOffset = tmpVariant.fTempOffset;

                        stWT_Data.stVariant[i].fPowerPreCool = tmpVariant.fPowerPreCool;
                        stWT_Data.stVariant[i].fPowerCool    = tmpVariant.fPowerCool;
                        stWT_Data.stVariant[i].uiCoolingType = tmpVariant.uiCoolingType;

                        stWT_Data.stVariant[i].bCopperMeasurement = tmpVariant.bWithMeasurment;
                        stWT_Data.stVariant[i].uiReheatTime = tmpVariant.ui_Reheat;

                        stWT_Data.stVariant[i].bDVS1Enabled = tmpVariant.bDVS1_Enabled;
                        stWT_Data.stVariant[i].bDVS2Enabled = tmpVariant.bDVS2_Enabled;
                        stWT_Data.stVariant[i].bDVS3Enabled = tmpVariant.bDVS3_Enabled;

                        stWT_Data.stVariant[i].bClamp1Enabled = tmpVariant.bClamp1_Enabled;
                        stWT_Data.stVariant[i].bClamp2Enabled = tmpVariant.bClamp2_Enabled;

                        stWT_Data.stVariant[i].bPreCoolOn = tmpVariant.bPreCoolON;
                        stWT_Data.stVariant[i].bCoolOn    = tmpVariant.bCoolOn;

                        stWT_Data.stVariant[i].fDVS1FeedLength = tmpVariant.fDVS1FeedLength;
                        stWT_Data.stVariant[i].fDVS2FeedLength = tmpVariant.fDVS2FeedLength;
                        stWT_Data.stVariant[i].fDVS3FeedLength = tmpVariant.fDVS3FeedLength;

                        stWT_Data.stVariant[i].byDVS1FeedSpeed = tmpVariant.byDVS1FeedSpeed;
                        stWT_Data.stVariant[i].byDVS2FeedSpeed = tmpVariant.byDVS2FeedSpeed;
                        stWT_Data.stVariant[i].byDVS3FeedSpeed = tmpVariant.byDVS3FeedSpeed;

                        stWT_Data.stVariant[i].byDVS1DrawbackLength = tmpVariant.byDVS1DrawbackLength;
                        stWT_Data.stVariant[i].byDVS2DrawbackLength = tmpVariant.byDVS2DrawbackLength;
                        stWT_Data.stVariant[i].byDVS3DrawbackLength = tmpVariant.byDVS3DrawbackLength;

                        stWT_Data.stVariant[i].byOverride = tmpVariant.byOverride;
                    }
                    catch {; }
                }
            }


        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_Item1ChangeEvent;
            VarCon.RemoveItem(Item1_RFID_WT);
            VarCon.RemoveItem(Item2_WT_Data);
            for(int i=0; i<aItem3_Variant.Length; i++)
            {
                VarCon.RemoveItem(aItem3_Variant[i]);
            }
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
