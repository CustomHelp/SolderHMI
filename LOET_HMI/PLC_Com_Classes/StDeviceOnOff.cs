using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace LOET_HMI
{
    //////////////////////////////////////////
    //// MarshalAs (Strukturen in der SPS) ///
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_DeviceOnOffPLCToHMI
    {
        // MBA 24.8.2020
        [MarshalAs(UnmanagedType.I2)] //Beckhoff UINT -> 16 Bit
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.I2)] //Beckhoff UINT -> 16 Bit
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string strBMK;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string strDescription;
        //******************************

        [MarshalAs(UnmanagedType.U1)]
        public byte byDeviceType; // Beckhoff USINT -> 8Bit


        [MarshalAs(UnmanagedType.I1)]
        public bool bManOn;
        [MarshalAs(UnmanagedType.I1)]
        public bool bManualAllowed;


        [MarshalAs(UnmanagedType.I1)]
        public bool bOut;

        [MarshalAs(UnmanagedType.U1)]
        public byte byState;

    }
    #endregion

    public partial class StDeviceOnOff : INotifyPropertyChanged
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

        ///////////////////////////////////////////////////

        private bool _bManOn; // Manuelle Ansteuerung (wird von HMI geschrieben)
        public bool bManOn
        {
            get
            {
                return _bManOn;
            }

            set
            {
                _bManOn = value;

                if (_bManOn)
                {
                    sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode - Select.png";
                }
                else
                {
                    sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode.png";
                }
                if(eType == eDeviceOnOffType.DT_20_PushButton && !bManOn)
                {
                    DBLog.Handler.Manual("Device " + sName, "Switch Manual", "off"); // KLAUS, MBA 26.8.2020: es muss geprüft werden, ob es so passt... evtl. wird es schon beim Registrieren aufgerufen
                }

                OnPropertyChanged();
            }
        }

        private bool _bManualAllowed;
        public bool bManualAllowed
        {
            get
            {
                return _bManualAllowed;
            }

            set
            {
                _bManualAllowed = value;
                if (_bManualAllowed)
                {
                    if(eType == eDeviceOnOffType.DT_10_ToggleSwitch)
                    {
                        VisManualAllowed_ToggleSwitch = Visibility.Visible;
                        VisManualAllowed_PushButton   = Visibility.Collapsed;
                    }
                    else if(eType == eDeviceOnOffType.DT_20_PushButton)
                    {
                        VisManualAllowed_ToggleSwitch = Visibility.Collapsed;
                        VisManualAllowed_PushButton   = Visibility.Visible;
                    }
                    
                }                    
                else
                {
                    VisManualAllowed_ToggleSwitch = Visibility.Collapsed;
                    VisManualAllowed_PushButton   = Visibility.Collapsed;
                }
                OnPropertyChanged(); // dieser Befehl wurde nachträglich hinzugefügt (Balog 4.7.2020)
            }
        }
        ///////////////////////////////////////////////////
        /*
        private Int16 _iType;
        public Int16 iType
        {
            get
            {
                return _iType;
            }

            set
            {
                _iType = value;
                OnPropertyChanged();
            }
        }
        */

        private eDeviceOnOffType _eType;
        public eDeviceOnOffType eType
        {
            get
            {
                return _eType;
            }

            set
            {
                _eType = value;

                if (_eType == eDeviceOnOffType.DT_10_ToggleSwitch)
                {
                    textLogikTyp = Properties.Resources.UcDevOnOff_ToggleSwitch;
                    
                }
                else if (_eType == eDeviceOnOffType.DT_20_PushButton)
                {
                    textLogikTyp = Properties.Resources.UcDevOnOff_PushButton;
                }
                    

                OnPropertyChanged();
            }
        }



        ///////////////////////////////////////////////////
        private bool _bOut;
        public bool bOut
        {
            get
            {
                return _bOut;
            }

            set
            {
                _bOut = value;
                OnPropertyChanged();
            }
        }

        ///////////////////////////////////////////////////
        private eComponentState _eState;
        public eComponentState eState
        {
            get
            {
                return _eState;
            }

            set
            {
                _eState = value;

                textState = GlobalFunc.GetComponentStateTxt(_eState);
                brushState = GlobalFunc.GetComponentStateColor(_eState);
                OnPropertyChanged();
            }
        }
        #endregion

        ////////////////////////////////////////
        ///////////// HMI To PLC  //////////////
        ////////////////////////////////////////
        #region 
        public ICommand cmdManual
        {
            get
            {
                return new RelayCommand(() => { WriteManualOn(); });
            }
        }

        public void WriteManualOn() // übernommen vom Horsch 5.12.2019
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Maintenance.iLevel)
            {
                if (ADSName != null)
                {
                    VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManOn", true);

                    if (eType == eDeviceOnOffType.DT_10_ToggleSwitch)
                    {
                        if (bManOn)
                            DBLog.Handler.Manual("Device " + sName, " Switch Manual ", "off");
                        else
                            DBLog.Handler.Manual("Device " + sName, " Switch Manual ", "on");
                    }
                    else if (eType == eDeviceOnOffType.DT_20_PushButton)
                    {
                        if (!bManOn)
                            DBLog.Handler.Manual("Device " + sName, "Switch Manual", "on");

                    }
                }
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }
        #endregion

        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region
        private Visibility _visManualAllowed_ToggleSwitch; // Visibility-Property für das Button für Handfunktion. Es wird durch die Variable "bManualAllowedHMI" angesteuert
        public Visibility VisManualAllowed_ToggleSwitch
        {
            get
            {
                return _visManualAllowed_ToggleSwitch;
            }

            set
            {
                _visManualAllowed_ToggleSwitch = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visManualAllowed_PushButton; // Visibility-Property für das Button für Handfunktion. Es wird durch die Variable "bManualAllowedHMI" angesteuert
        public Visibility VisManualAllowed_PushButton
        {
            get
            {
                return _visManualAllowed_PushButton;
            }

            set
            {
                _visManualAllowed_PushButton = value;
                OnPropertyChanged();
            }
        }


        private string _sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode.png";
        public string sManBtnPath
        {
            get
            {
                return _sManBtnPath;
            }

            set
            {
                _sManBtnPath = value;
                OnPropertyChanged();
            }
        }

        private System.Windows.Media.Brush _brushState;
        public System.Windows.Media.Brush brushState
        {
            get
            {
                return _brushState;
            }

            set
            {
                _brushState = value;
                OnPropertyChanged();
            }
        }

        private string _textState;
        public string textState
        {
            get
            {
                return _textState;
            }

            set
            {
                _textState = value;
                OnPropertyChanged();
            }
        }

        private string _textLogikTyp;
        public string textLogikTyp
        {
            get
            {
                return _textLogikTyp;
            }

            set
            {
                _textLogikTyp = value;
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
        /////////// ADS - RX ///////////////
        ////////////////////////////////////
        #region
        public void Register(string sName)
        {
            bManualAllowed = false;

            ADSName = sName;
            Item.sName = ADSName + ".PLC_TO_HMI";
            Item.Type = typeof(ST_DeviceOnOffPLCToHMI);

            Item = VarCon.AddItem(ADSName + ".PLC_TO_HMI", typeof(ST_DeviceOnOffPLCToHMI));

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

            ST_DeviceOnOffPLCToHMI tmp = (ST_DeviceOnOffPLCToHMI)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iModul = tmp.uiModul;
                    iStation = tmp.uiStation;

                    sBMK = tmp.strBMK;
                    sName = tmp.strName;
                    sDescription = tmp.strDescription;

                    bManOn                  = tmp.bManOn;
                    bManualAllowed          = tmp.bManualAllowed;

                    eType = (eDeviceOnOffType)tmp.byDeviceType;
                    bOut                    = tmp.bOut;                  
                    
                    eState                  = (eComponentState)tmp.byState;
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
    }
}

