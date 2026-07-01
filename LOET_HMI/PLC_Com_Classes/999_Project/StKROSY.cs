using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace LOET_HMI.PLC_Com_Classes
{
    //////////////////////////////////////////
    //// MarshalAs (Struktur in der SPS) /////
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_KrosyGlobal_PLCToHMI
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bPLC_Heartbeat;
        [MarshalAs(UnmanagedType.I1)]
        public bool bKROSY_Heartbeat;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string strPLC_Hostname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string strPLC_IP;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string strPLC_Version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string strPLC_Language;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string strKROSY_Device;

        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: INT
        public Int16 iOrderMax;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: INT
        public Int16 iObjectMax;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_KrosyStation_PLCToHMI
    {
        //???? variablen einzeln auslesen

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strscanCode;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string ident;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben2;

        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: INT
        public Int16 iamount;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen        In TwinCAT: INT
        public Int16 iamountOK;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_KrosyStationObject_PLCToHMI
    {
        // Substruktur KROSYstate
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 KROSYstate_objecttype;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 KROSYstate_objectstate;

        // Substruktur PLCstate
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 PLCstate_objecttype;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 PLCstate_objectstate;

        // Substruktur ksidnet
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string ident;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string ben2;


        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 i_WTNr;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 i_INDNr;
        [MarshalAs(UnmanagedType.I2)] // I2: 2-Byte-Ganzzahl mit Vorzeichen  
        public Int16 iOverride;

        [MarshalAs(UnmanagedType.I1)]
        public bool b_DVS1_ON;
        [MarshalAs(UnmanagedType.I1)]
        public bool b_DVS2_ON;
        [MarshalAs(UnmanagedType.I1)]
        public bool b_DVS3_ON;
        [MarshalAs(UnmanagedType.I1)]
        public bool bIO;
        [MarshalAs(UnmanagedType.I1)]
        public bool bNIO;
    }


    #endregion






    public class StKROSY : INotifyPropertyChanged
    {
        public const int ciNrOfObjects = 1;

        // ADS Verbindung
        IADSConnection VarCon = new ADSService();

        private ADSItem ItemGlobal = new ADSItem();  
        //private ADSItem ItemStation1 = new ADSItem();
        //private ADSItem ItemStation2 = new ADSItem();
        private ADSItem[] arrItemStation = new ADSItem[2];
        private ADSItem[] arrItemStation1Objects = new ADSItem[ciNrOfObjects];
        private ADSItem[] arrItemStation2Objects = new ADSItem[ciNrOfObjects];

            

        public string ADSName { get; set; }


        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region
        private StGlobal _stGlobal;
        public StGlobal stGlobal
        {
            get
            {
                return _stGlobal;
            }

            set
            {
                _stGlobal = value;
                OnPropertyChanged();
            }
        }

        private StStation[] _arrStation;
        public StStation[] arrStation
        {
            get
            {
                return _arrStation;
            }

            set
            {
                _arrStation = value;
                OnPropertyChanged();
            }
        }


        #endregion

        ////////////////////////////////////
        ///////////// Konstruktor //////////
        ////////////////////////////////////
        #region
        public StKROSY()
        {
            stGlobal    = new StGlobal();
            arrStation   = new StStation[2];

            for(int iStat = 1; iStat<=2; iStat++)
            {
                arrStation[iStat-1] = new StStation(iStat);

                arrStation[iStat-1].arrStationObjects = new StStationObject[ciNrOfObjects];

                for(int iObj = 1; iObj <= ciNrOfObjects; iObj++)
                {
                    arrStation[iStat-1].arrStationObjects[iObj-1] = new StStationObject(iObj);
                }
            }
        }
        #endregion


        //////////////////////////////////////////
        // Subkklassen (Sub-Strukturen TwinCAT) // 
        //////////////////////////////////////////
        #region
        public class StGlobal : INotifyPropertyChanged
        {
            private bool _bPLC_Heartbeat;
            public bool bPLC_Heartbeat
            {
                get
                {
                    return _bPLC_Heartbeat;
                }

                set
                {
                    _bPLC_Heartbeat = value;
                    OnPropertyChanged();
                }
            }


            private bool _bKROSY_Heartbeat;
            public bool bKROSY_Heartbeat
            {
                get
                {
                    return _bKROSY_Heartbeat;
                }

                set
                {
                    _bKROSY_Heartbeat = value;
                    OnPropertyChanged();
                }
            }

            private string _strPLC_Hostname;
            public string strPLC_Hostname
            {
                get
                {
                    return _strPLC_Hostname;
                }

                set
                {
                    _strPLC_Hostname = value;
                    OnPropertyChanged();
                }
            }

            private string _strPLC_IP;
            public string strPLC_IP
            {
                get
                {
                    return _strPLC_IP;
                }

                set
                {
                    _strPLC_IP = value;
                    OnPropertyChanged();
                }
            }


            private string _strPLC_Version;
            public string strPLC_Version
            {
                get
                {
                    return _strPLC_Version;
                }

                set
                {
                    _strPLC_Version = value;
                    OnPropertyChanged();
                }
            }

            private string _strPLC_Language;
            public string strPLC_Language
            {
                get
                {
                    return _strPLC_Language;
                }

                set
                {
                    _strPLC_Language = value;
                    OnPropertyChanged();
                }
            }


            private string _strKROSY_Device;
            public string strKROSY_Device
            {
                get
                {
                    return _strKROSY_Device;
                }

                set
                {
                    _strKROSY_Device = value;
                    OnPropertyChanged();
                }
            }

            private int _iOrderMax;
            public int iOrderMax
            {
                get
                {
                    return _iOrderMax;
                }

                set
                {
                    _iOrderMax = value;
                    OnPropertyChanged();
                }
            }

            private int _iObjectMax;
            public int iObjectMax
            {
                get
                {
                    return _iObjectMax;
                }

                set
                {
                    _iObjectMax = value;
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

        public class StStation : INotifyPropertyChanged
        {
            private int _iStationNr;
            public int iStationNr
            {
                get
                {
                    return _iStationNr;
                }

                set
                {
                    _iStationNr = value;
                    OnPropertyChanged();
                }
            }

            //********************************

            private string _strscanCode;
            public string strscanCode
            {
                get
                {
                    return _strscanCode;
                }

                set
                {
                    _strscanCode = value;
                    OnPropertyChanged();
                }
            }

            private string _strKSIdent_ident;
            public string strKSIdent_ident
            {
                get
                {
                    return _strKSIdent_ident;
                }

                set
                {
                    _strKSIdent_ident = value;
                    OnPropertyChanged();
                }
            }

            private int _iAmount;
            public int iAmount
            {
                get
                {
                    return _iAmount;
                }

                set
                {
                    _iAmount = value;
                    OnPropertyChanged();
                }
            }

            private int _iAmountOK;
            public int iAmountOK
            {
                get
                {
                    return _iAmountOK;
                }

                set
                {
                    _iAmountOK = value;
                    OnPropertyChanged();
                }
            }

            private StStationObject[] _arrStationObjects;
            public StStationObject[] arrStationObjects
            {
                get
                {
                    return _arrStationObjects;
                }

                set
                {
                    _arrStationObjects = value;
                    OnPropertyChanged();
                }
            }

            public StStation(int _iStationNr)
            {
                iStationNr = _iStationNr;
            }


            // Create the OnPropertyChanged method to raise the event
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class StStationObject : INotifyPropertyChanged
        {
            private int _iObjectNr;
            public int iObjectNr
            {
                get
                {
                    return _iObjectNr;
                }

                set
                {
                    _iObjectNr = value;
                    OnPropertyChanged();
                }
            }

            //*********************************************


            private int _iKROSYstate_objecttype;
            public int iKROSYstate_objecttype
            {
                get
                {
                    return _iKROSYstate_objecttype;
                }

                set
                {
                    _iKROSYstate_objecttype = value;
                    OnPropertyChanged();
                }
            }

            private int _iKROSYstate_objectstate;
            public int iKROSYstate_objectstate
            {
                get
                {
                    return _iKROSYstate_objectstate;
                }

                set
                {
                    _iKROSYstate_objectstate = value;
                    OnPropertyChanged();
                }
            }

            private int _iPLCstate_objecttype;
            public int iPLCstate_objecttype
            {
                get
                {
                    return _iPLCstate_objecttype;
                }

                set
                {
                    _iPLCstate_objecttype = value;
                    OnPropertyChanged();
                }
            }

            private int _iPLCstate_objectstate;
            public int iPLCstate_objectstate
            {
                get
                {
                    return _iPLCstate_objectstate;
                }

                set
                {
                    _iPLCstate_objectstate = value;
                    OnPropertyChanged();
                }
            }


            private string _strKSIdent_ident;
            public string strKSIdent_ident
            {
                get
                {
                    return _strKSIdent_ident;
                }

                set
                {
                    _strKSIdent_ident = value;
                    OnPropertyChanged();
                }
            }

            private int _i_WTNr;
            public int i_WTNr
            {
                get
                {
                    return _i_WTNr;
                }

                set
                {
                    _i_WTNr = value;
                    OnPropertyChanged();
                }
            }

            private int _i_INDNr;
            public int i_INDNr
            {
                get
                {
                    return _i_INDNr;
                }

                set
                {
                    _i_INDNr = value;
                    OnPropertyChanged();
                }
            }

            private int _iOverride;
            public int iOverride
            {
                get
                {
                    return _iOverride;
                }

                set
                {
                    _iOverride = value;
                    OnPropertyChanged();
                }
            }

            private bool _b_DVS1_ON;
            public bool b_DVS1_ON
            {
                get
                {
                    return _b_DVS1_ON;
                }

                set
                {
                    _b_DVS1_ON = value;
                    OnPropertyChanged();
                }
            }

            private bool _b_DVS2_ON;
            public bool b_DVS2_ON
            {
                get
                {
                    return _b_DVS2_ON;
                }

                set
                {
                    _b_DVS2_ON = value;
                    OnPropertyChanged();
                }
            }

            private bool _b_DVS3_ON;
            public bool b_DVS3_ON
            {
                get
                {
                    return _b_DVS3_ON;
                }

                set
                {
                    _b_DVS3_ON = value;
                    OnPropertyChanged();
                }
            }

            private bool _bIO;
            public bool bIO
            {
                get
                {
                    return _bIO;
                }

                set
                {
                    _bIO = value;
                    OnPropertyChanged();
                }
            }

            private bool _bNIO;
            public bool bNIO
            {
                get
                {
                    return _bNIO;
                }

                set
                {
                    _bNIO = value;
                    OnPropertyChanged();
                }
            }

            public StStationObject(int _iObjectNr)
            {
                iObjectNr = _iObjectNr;
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
        ///////////// ADS - TX /////////////
        ////////////////////////////////////
        #region
        public void ResetStation(int _iStationNr)
        {
            VarCon.WriteItem(ADSName + ".g_arrKrosyStationHMI[" + _iStationNr.ToString() + "].HMI_TO_PLC.bReset", true);
        }
        #endregion

        ////////////////////////////////////
        /////////// ADS - RX ///////////////
        ////////////////////////////////////
        #region
        public void Register()
        {
            ADSName = "GVL_KROSY";

            ItemGlobal.sName = ADSName + ".g_arrKrosyGlobalHMI[1].PLC_TO_HMI";
            ItemGlobal.Type = typeof(ST_KrosyGlobal_PLCToHMI);
            ItemGlobal = VarCon.AddItem(ItemGlobal.sName, ItemGlobal.Type);

            string ADSNameStation = ADSName + ".g_arrKrosyStationHMI";
            
            for(int iIndStat=0; iIndStat<2; iIndStat++)
            {
                arrItemStation[iIndStat] = new ADSItem();

                arrItemStation[iIndStat].sName = ADSNameStation + "[" + (iIndStat+1).ToString() + "].PLC_TO_HMI";
                arrItemStation[iIndStat].Type = typeof(ST_KrosyStation_PLCToHMI);
                arrItemStation[iIndStat] = VarCon.AddItem(arrItemStation[iIndStat].sName, arrItemStation[iIndStat].Type);


                for (int iIndObj = 0; iIndObj < ciNrOfObjects; iIndObj++)
                {
                    //string ADSNameStatObject = arrItemStation[iStat].sName + ".arrOrderInfo";
                    string ADSNameStatObject = arrItemStation[iIndStat].sName + ".arrOrderInfo[" + (iIndObj+1).ToString() + "]";

                    if ((iIndStat+1) == 1) // Objekte von Station 1
                    {
                        arrItemStation1Objects[iIndObj] = new ADSItem();

                        arrItemStation1Objects[iIndObj].sName = ADSNameStatObject;
                        arrItemStation1Objects[iIndObj].Type = typeof(ST_KrosyStationObject_PLCToHMI);
                        arrItemStation1Objects[iIndObj] = VarCon.AddItem(arrItemStation1Objects[iIndObj].sName, arrItemStation1Objects[iIndObj].Type);
                    }
                    else if ((iIndStat+1) == 2)// Objekte von Station 2
                    {
                        arrItemStation2Objects[iIndObj] = new ADSItem();

                        arrItemStation2Objects[iIndObj].sName = ADSNameStatObject;
                        arrItemStation2Objects[iIndObj].Type = typeof(ST_KrosyStationObject_PLCToHMI);
                        arrItemStation2Objects[iIndObj] = VarCon.AddItem(arrItemStation2Objects[iIndObj].sName, arrItemStation2Objects[iIndObj].Type);
                    }
                }
            }

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_Item1ChangeEvent;
        }

        private void VarCon_Item1ChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (ItemGlobal.iHandle == e.Item[j].iHandle)
                {
                    ItemGlobal = e.Item[j];
                    UpdateProperties_StGlobal((ST_KrosyGlobal_PLCToHMI)ItemGlobal.Value);
                }

                for (int iIndStat = 0; iIndStat < 2; iIndStat++)
                {
                    if (arrItemStation[iIndStat].iHandle == e.Item[j].iHandle)
                    {
                        arrItemStation[iIndStat] = e.Item[j];
                        UpdateProperties_StStation((ST_KrosyStation_PLCToHMI)arrItemStation[iIndStat].Value,   _iStatIndex: iIndStat);
                    }
                    for (int iIndObj = 0; iIndObj < ciNrOfObjects; iIndObj++)
                    {
                        if ((iIndStat+1) == 1) // Objekte von Station 1
                        {
                            if(arrItemStation1Objects[iIndObj].iHandle == e.Item[j].iHandle)
                            {
                                arrItemStation1Objects[iIndObj] = e.Item[j];
                                UpdateProperties_StStationObject((ST_KrosyStationObject_PLCToHMI)arrItemStation1Objects[iIndObj].Value,   _iStatIndex: iIndStat,   _iObjIndex: iIndObj);
                            }
                            
                        }
                        else if ((iIndStat+1) == 2) // Objekte von Station 2
                        {
                            if (arrItemStation2Objects[iIndObj].iHandle == e.Item[j].iHandle)
                            {
                                arrItemStation2Objects[iIndObj] = e.Item[j];
                                UpdateProperties_StStationObject((ST_KrosyStationObject_PLCToHMI)arrItemStation2Objects[iIndObj].Value, _iStatIndex: iIndStat, _iObjIndex: iIndObj);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateProperties_StGlobal(ST_KrosyGlobal_PLCToHMI _tmpStGlobal)
        {
            try
            {
                stGlobal.bPLC_Heartbeat = _tmpStGlobal.bPLC_Heartbeat;
                stGlobal.bKROSY_Heartbeat = _tmpStGlobal.bKROSY_Heartbeat;

                stGlobal.strPLC_Hostname = _tmpStGlobal.strPLC_Hostname;
                stGlobal.strPLC_IP = _tmpStGlobal.strPLC_IP;
                stGlobal.strPLC_Version = _tmpStGlobal.strPLC_Version;
                stGlobal.strPLC_Language = _tmpStGlobal.strPLC_Language;

                stGlobal.strKROSY_Device = _tmpStGlobal.strKROSY_Device;

                stGlobal.iOrderMax = _tmpStGlobal.iOrderMax;
                stGlobal.iObjectMax = _tmpStGlobal.iObjectMax;
            }
            catch (Exception ex)
            {
                AppLogger.Log("StKROSY.UpdateProperties_StGlobal", ex);
            }
        }

        private void UpdateProperties_StStation(ST_KrosyStation_PLCToHMI _tmpStStation, int _iStatIndex)
        {
            try
            {
                arrStation[_iStatIndex].strscanCode = _tmpStStation.strscanCode;
                arrStation[_iStatIndex].strKSIdent_ident = _tmpStStation.ident;
                arrStation[_iStatIndex].iAmount = _tmpStStation.iamount;
                arrStation[_iStatIndex].iAmountOK = _tmpStStation.iamountOK;
            }
            catch (Exception ex)
            {
                AppLogger.Log("StKROSY.UpdateProperties_StStation", ex);
            }
            
        }

        private void UpdateProperties_StStationObject(ST_KrosyStationObject_PLCToHMI _tmpStStatObj, int _iStatIndex, int _iObjIndex)
        {
            try
            {
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].iKROSYstate_objecttype = _tmpStStatObj.KROSYstate_objecttype;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].iKROSYstate_objectstate = _tmpStStatObj.KROSYstate_objectstate;

                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].iPLCstate_objecttype = _tmpStStatObj.PLCstate_objecttype;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].iPLCstate_objectstate = _tmpStStatObj.PLCstate_objectstate;

                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].strKSIdent_ident = _tmpStStatObj.ident;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].i_WTNr = _tmpStStatObj.i_WTNr;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].i_INDNr = _tmpStStatObj.i_INDNr;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].iOverride = _tmpStStatObj.iOverride;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].b_DVS1_ON = _tmpStStatObj.b_DVS1_ON;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].b_DVS2_ON = _tmpStStatObj.b_DVS2_ON;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].b_DVS3_ON = _tmpStStatObj.b_DVS3_ON;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].bIO = _tmpStStatObj.bIO;
                arrStation[_iStatIndex].arrStationObjects[_iObjIndex].bNIO = _tmpStStatObj.bNIO;
            }
            catch (Exception ex)
            {
                AppLogger.Log("StKROSY.UpdateProperties_StStationObject", ex);
            }
        }


        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_Item1ChangeEvent;
            VarCon.RemoveItem(ItemGlobal);
            for (int i = 0; i < arrItemStation.Length; i++)
            {
                VarCon.RemoveItem(arrItemStation[i]);
            }
            for (int i = 0; i < arrItemStation1Objects.Length; i++)
            {
                VarCon.RemoveItem(arrItemStation1Objects[i]);
            }
            for (int i = 0; i < arrItemStation2Objects.Length; i++)
            {
                VarCon.RemoveItem(arrItemStation2Objects[i]);
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
