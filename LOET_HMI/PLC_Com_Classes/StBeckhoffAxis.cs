using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using LOET_HMI.PLC_Com_Classes;



namespace LOET_HMI
{
    //////////////////////////////////////////
    //// MarshalAs (Strukturen in der SPS) ///
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_AxisPLCToHMI
    {
        // MBA 21.8.2020
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


        [MarshalAs(UnmanagedType.I1)]
        public bool bManOn;
        [MarshalAs(UnmanagedType.I1)]
        public bool bManualAllowed;

        [MarshalAs(UnmanagedType.I1)]
        public bool bLimit_N;
        [MarshalAs(UnmanagedType.I1)]
        public bool bLimit_P;
        [MarshalAs(UnmanagedType.I1)]
        public bool bReady;
        [MarshalAs(UnmanagedType.I1)]
        public bool bController;

        [MarshalAs(UnmanagedType.I1)]
        public bool bHomed;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHasJob;
        [MarshalAs(UnmanagedType.I1)]
        public bool bNegativeDirection;
        [MarshalAs(UnmanagedType.I1)]
        public bool bPositiveDirection;
        [MarshalAs(UnmanagedType.I1)]
        public bool bNotMoving;
        [MarshalAs(UnmanagedType.I1)]
        public bool bCoupled;
        [MarshalAs(UnmanagedType.I1)]
        public bool bInTargetPosition;
        [MarshalAs(UnmanagedType.I1)]
        public bool bInPositionArea;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHasRef;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 iState;

        [MarshalAs(UnmanagedType.R8)] // Beckhoff LREAL -> 64 Bit;  C# double -> 64 Bit
        public double lrPositionLag;
        [MarshalAs(UnmanagedType.R8)]
        public double lrActOverride;
        [MarshalAs(UnmanagedType.R8)]
        public double lrSpeedAct;
        //[MarshalAs(UnmanagedType.R8)] // MBA 21.8.2020:
        //public double lrSpeedSetPoint; // MBA 21.8.2020:
        //[MarshalAs(UnmanagedType.R8)] // MBA 21.8.2020:
        //public double lrPosTarget; // MBA 21.8.2020:
        [MarshalAs(UnmanagedType.R8)]
        public double lrPosAct;

        // MBA 21.8.2020:
        [MarshalAs(UnmanagedType.R8)]
        public double lrSpeedSetpoint;
        [MarshalAs(UnmanagedType.R8)]
        public double lrSpeedSetPointMin;
        [MarshalAs(UnmanagedType.R8)]
        public double lrSpeedSetPointMax;

        [MarshalAs(UnmanagedType.R8)]
        public double lrPosTarget;
        [MarshalAs(UnmanagedType.R8)]
        public double lrPosTargetMin;
        [MarshalAs(UnmanagedType.R8)]
        public double lrPosTargetMax;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string strUnit;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string strUnitSpeed;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strSoEMessage;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string strError;
        //***********************************


        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sName;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        //public string sUnit;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        //public string sUnitSpeed;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        //public string sSoEMessage;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        //public string sError;

        /////////////// Rena:
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bShutter1IsUp;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bShutter2IsUp;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bLift2Down;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bPress2;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bPress3;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bCutting;      //RENA
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bResWeld;      //RENA
        //
        ////////////////////////

        //public ST_Setting_LREAL stTargetPos   = new ST_Setting_LREAL();
        //public ST_Setting_LREAL stTargetSpeed = new ST_Setting_LREAL();
    }
    #endregion

    public partial class StBeckhoffAxis : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }


        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region PLC To HMI
        private int _iModul;
        public int iModul // HINZUFÜGEN
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
        public int iStation // HINZUFÜGEN
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
        public string sBMK // HINZUFÜGEN
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
        public string sDescription // HINZUFÜGEN
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
                    VisManualOn = Visibility.Visible;
                    if (bHasRef)
                        VisRefAllowed = Visibility.Visible;
                }
                else
                {
                    sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode.png";
                    VisManualOn = Visibility.Hidden;
                    VisRefAllowed = Visibility.Hidden;
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
                    VisManualAllowed = Visibility.Visible;
                else
                    VisManualAllowed = Visibility.Hidden;
                OnPropertyChanged(); // dieser Befehl wurde nachträglich hinzugefügt (Balog 4.7.2020)
            }
        }
        ///////////////////////////////////////////////////

        private bool _bLimit_N;
        public bool bLimit_N
        {
            get
            {
                return _bLimit_N;
            }

            set
            {
                _bLimit_N = value;
                OnPropertyChanged();
            }
        }

        private bool _bLimit_P;
        public bool bLimit_P
        {
            get
            {
                return _bLimit_P;
            }

            set
            {
                _bLimit_P = value;
                OnPropertyChanged();
            }
        }

        private bool _bReady;
        public bool bReady
        {
            get
            {
                return _bReady;
            }

            set
            {
                _bReady = value;
                OnPropertyChanged();
            }
        }

        private bool _bController;
        public bool bController
        {
            get
            {
                return _bController;
            }

            set
            {
                _bController = value;
                OnPropertyChanged();
            }
        }

        ///////////////////////////////////////////////////

        private bool _bHomed;
        public bool bHomed
        {
            get
            {
                return _bHomed;
            }

            set
            {
                _bHomed = value;
                OnPropertyChanged();
            }
        }

        private bool _bHasJob;
        public bool bHasJob
        {
            get
            {
                return _bHasJob;
            }

            set
            {
                _bHasJob = value;
                OnPropertyChanged();
            }
        }

        private bool _bNegativeDirection;
        public bool bNegativeDirection
        {
            get
            {
                return _bNegativeDirection;
            }

            set
            {
                _bNegativeDirection = value;
                OnPropertyChanged();
            }
        }

        private bool _bPositiveDirection;
        public bool bPositiveDirection
        {
            get
            {
                return _bPositiveDirection;
            }

            set
            {
                _bPositiveDirection = value;
                OnPropertyChanged();
            }
        }


        private bool _bNotMoving;
        public bool bNotMoving
        {
            get
            {
                return _bNotMoving;
            }

            set
            {
                _bNotMoving = value;
                OnPropertyChanged();
            }
        }

        private bool _bCoupled;
        public bool bCoupled
        {
            get
            {
                return _bCoupled;
            }

            set
            {
                _bCoupled = value;
                OnPropertyChanged();
            }
        }

        private bool _bInTargetPosition;
        public bool bInTargetPosition
        {
            get
            {
                return _bInTargetPosition;
            }

            set
            {
                _bInTargetPosition = value;
                OnPropertyChanged();
            }
        }

        private bool _bInPositionArea;
        public bool bInPositionArea
        {
            get
            {
                return _bInPositionArea;
            }

            set
            {
                _bInPositionArea = value;
                OnPropertyChanged();
            }
        }

        private bool _bHasRef;
        public bool bHasRef 
        {
            get
            {
                return _bHasRef;
            }

            set
            {
                _bHasRef = value;

                if (_bHasRef && bManOn)
                    VisRefAllowed = Visibility.Visible;
                else
                    VisRefAllowed = Visibility.Collapsed;
                OnPropertyChanged();
            }
        }
        /////////////////////////////////////////////////


        private int _iState;
        public int iState // HINZUFÜGEN
        {
            get
            {
                return _iState;
            }

            set
            {
                _iState = value;
                OnPropertyChanged();
            }
        }

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



        ///////////////////////////////////////////////

        private double _lrPositionLag;
        public double lrPositionLag
        {
            get
            {
                return _lrPositionLag;
            }

            set
            {
                _lrPositionLag = value;
                OnPropertyChanged();
            }
        }

        private double _lrActOverride;
        public double lrActOverride
        {
            get
            {
                return _lrActOverride;
            }

            set
            {
                _lrActOverride = value;
                OnPropertyChanged();
            }
        }

        private double _lrSpeedAct;
        public double lrSpeedAct
        {
            get
            {
                return _lrSpeedAct;
            }

            set
            {
                _lrSpeedAct = value;
                OnPropertyChanged();
            }
        }

        private double _lrPosAct;
        public double lrPosAct
        {
            get
            {
                return _lrPosAct;
            }

            set
            {
                _lrPosAct = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////

        private double _lrSpeedSetPoint;
        public double lrSpeedSetPoint
        {
            get
            {
                return _lrSpeedSetPoint;
            }

            set
            {
                _lrSpeedSetPoint = value;
                OnPropertyChanged();
            }
        }

        private double _lrSpeedSetPointMin;
        public double lrSpeedSetPointMin
        {
            get
            {
                return _lrSpeedSetPointMin;
            }

            set
            {
                _lrSpeedSetPointMin = value;
                OnPropertyChanged();
            }
        }

        private double _lrSpeedSetPointMax;
        public double lrSpeedSetPointMax
        {
            get
            {
                return _lrSpeedSetPointMax;
            }

            set
            {
                _lrSpeedSetPointMax = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////
        private double _lrPosTarget;
        public double lrPosTarget
        {
            get
            {
                return _lrPosTarget;
            }

            set
            {
                _lrPosTarget = value;
                OnPropertyChanged();
            }
        }

        private double _lrPosTargetMin;
        public double lrPosTargetMin
        {
            get
            {
                return _lrPosTargetMin;
            }

            set
            {
                _lrPosTargetMin = value;
                OnPropertyChanged();
            }
        }

        private double _lrPosTargetMax;
        public double lrPosTargetMax
        {
            get
            {
                return _lrPosTargetMax;
            }

            set
            {
                _lrPosTargetMax = value;
                OnPropertyChanged();
            }
        }
        ///////////////////////////////////////////////////////////
        private string _sUnit;
        public string sUnit
        {
            get
            {
                return _sUnit;
            }

            set
            {
                _sUnit = "[" + value + "]";
                //sUnit1 = "[" + value + "]";
                OnPropertyChanged();
            }
        }


        private string _sUnitSpeed;
        public string sUnitSpeed
        {
            get
            {
                return _sUnitSpeed;
            }

            set
            {
                _sUnitSpeed = "[" + value + "]";
                OnPropertyChanged();
            }
        }


        ///////////////////////////////////////////////////////////

        private string _sSoEMessage;
        public string sSoEMessage
        {
            get
            {
                return _sSoEMessage;
            }

            set
            {
                _sSoEMessage = value;
                OnPropertyChanged();
            }
        }


        private string _sError;
        public string sError
        {
            get
            {
                return _sError;
            }

            set
            {
                _sError =  value;
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
        public void WriteManualOn()
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Maintenance.iLevel)
            {
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManOn", true);

                if (_bManOn)
                    DBLog.Handler.Manual("Axis " + sName, "Switch Manual", "off");
                else
                    DBLog.Handler.Manual("Axis " + sName, "Switch Manual", "on");
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

        public ICommand cmdStart
        {
            get
            {
                return new RelayCommand(() => { WriteStart(); });
            }
        }

        public ICommand cmdStop
        {
            get
            {
                return new RelayCommand(() => { WriteStop(); });
            }
        }



        public ICommand cmdRef
        {
            get
            {
                return new RelayCommand(() => { WriteRef(); });
            }
        }

        public ICommand cmdQuit
        {
            get
            {
                return new RelayCommand(() => { WriteQuit(); });
            }
        }

        ///////////////////////////////////////////////////////
        public void SetPlus(bool Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenPos_Low", Value);
        }
        public void SetPlusPlus(bool Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenPos_High", Value);
        }
        public void SetMinus(bool Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenNeg_Low", Value);
        }
        public void SetMinusMinus(bool Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenNeg_High", Value);
        }
        ///////////////////////////////////////////////////////
        public void WriteStart()
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bStartFahrt", true);
        }
        public void WriteStop()
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bStop", true);
        }
        public void WriteQuit()
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bReset", true);
        }
        public void WriteRef()
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bStartReferenz", true);
        }
        ///////////////////////////////////////////////////////
        public void WriteTargetPos(double Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.lrTargetPos", Value);
        }
        public void WriteTargetSpeed(double Value)
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.lrTargetSpeed", Value);
        }
        ///////////////////////////////////////////////////////

        public void SetTrigReleaseContr(bool Value) // KLAUS: was macht es? MBA 21.8.2020
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bReleaseController", Value);
        }

        #endregion

        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region 

        private string _sManBtnPath = "LOET_HMI/Resources/Manual Mode.png";
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

        /*
        private string _sUnit1;
        public string sUnit1
        {
            get
            {
                return _sUnit1;
            }

            set
            {
                _sUnit1 = value;
                OnPropertyChanged();
            }
        }
        */
        private Visibility _visManualOn; 
        public Visibility VisManualOn 
        {
            get
            {
                return _visManualOn;
            }

            set
            {
                _visManualOn = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visManualAllowed;
        public Visibility VisManualAllowed
        {
            get
            {
                return _visManualAllowed;
            }

            set
            {
                _visManualAllowed = value;

                if (_visManualAllowed == Visibility.Hidden) // Rena: wegen den Schotts muss diese Bedingung so angepasst werden (Balog 17.4.2020) 
                    VisManualOn = Visibility.Hidden;

                OnPropertyChanged();
            }

        }
        private Visibility _visRefAllowed;
        public Visibility VisRefAllowed
        {
            get
            {
                return _visRefAllowed;
            }

            set
            {
                _visRefAllowed = value;
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
            Item.sName = ADSName + ".PLC_to_HMI";
            Item.Type = typeof(ST_AxisPLCToHMI);

            Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_AxisPLCToHMI));

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

            ST_AxisPLCToHMI tmp = (ST_AxisPLCToHMI)Item.Value;
            ;
            if (tmp != null)
            {
                try
                {
                    iModul              = tmp.uiModul;
                    iStation            = tmp.uiStation;
                                        
                    sBMK                = tmp.strBMK;
                    sName               = tmp.strName;
                    sDescription        = tmp.strDescription;

                    bManOn              = tmp.bManOn;
                    bManualAllowed      = tmp.bManualAllowed;

                    bLimit_N            = tmp.bLimit_N;
                    bLimit_P            = tmp.bLimit_P;
                    bReady              = tmp.bReady;
                    bController         = tmp.bController;

                    bHomed              = tmp.bHomed;
                    bHasJob             = tmp.bHasJob;
                    bNegativeDirection  = tmp.bNegativeDirection;
                    bPositiveDirection  = tmp.bPositiveDirection;
                    bNotMoving          = tmp.bNotMoving;
                    bCoupled            = tmp.bCoupled;
                    bInTargetPosition   = tmp.bInTargetPosition;
                    bInPositionArea     = tmp.bInPositionArea;
                    bHasRef             = tmp.bHasRef;

                    eState              = (eComponentState)tmp.iState;

                    lrPositionLag       = Math.Round(tmp.lrPositionLag,3);
                    lrActOverride       = tmp.lrActOverride;
                    lrSpeedAct          = Math.Round(tmp.lrSpeedAct,3);
                    lrPosAct            = Math.Round(tmp.lrPosAct, 3);

                    lrSpeedSetPoint     = Math.Round(tmp.lrSpeedSetpoint, 3);
                    lrSpeedSetPointMin  = Math.Round(tmp.lrSpeedSetPointMin, 3);
                    lrSpeedSetPointMax  = Math.Round(tmp.lrSpeedSetPointMax, 3);

                    lrPosTarget         = Math.Round(tmp.lrPosTarget,3);
                    lrPosTargetMin      = Math.Round(tmp.lrPosTargetMin, 3);
                    lrPosTargetMax      = Math.Round(tmp.lrPosTargetMax, 3);

                    sUnit               = tmp.strUnit;
                    sUnitSpeed          = tmp.strUnitSpeed;

                    sSoEMessage         = tmp.strSoEMessage;
                    sError              = tmp.strError;
                }
                catch
                {

                }           
            }
        } 

        public void Deregister()
        {
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenPos_High", false);
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenPos_Low", false);
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenNeg_High", false);
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bTippenNeg_Low", false);

            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);
        }
        #endregion
    }
}

