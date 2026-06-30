using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace LOET_HMI
{
    //////////////////////////////////////////
    //// MarshalAs (Struktur in der SPS) /////
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_CylinderPLCToHMI
    {
        [MarshalAs(UnmanagedType.U2)] // Beckhoff UINT: 16 Bit;     C# ushort: Unsignet 16-Bit integer  -> diese verwenden???
        public ushort ushModul;
        [MarshalAs(UnmanagedType.U2)]
        public ushort ushStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string strBMK;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string strDescription;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string strSensor_BMK_A;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string strSensor_BMK_B;


        [MarshalAs(UnmanagedType.I1)]
        public bool bManOn;
        [MarshalAs(UnmanagedType.I1)]
        public bool bRestPos;
        [MarshalAs(UnmanagedType.I1)]
        public bool bForerunPos;

        [MarshalAs(UnmanagedType.I1)]
        public bool bValveRest;
        [MarshalAs(UnmanagedType.I1)]
        public bool bValveForerun;

        [MarshalAs(UnmanagedType.I1)]
        public bool bHasRestPos;
        [MarshalAs(UnmanagedType.I1)]
        public bool bHasForerunPos;

        [MarshalAs(UnmanagedType.I1)]
        public bool bManualAllowed;

        [MarshalAs(UnmanagedType.I1)]
        public bool bIsSlideCyl;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string strSideCylComment;

        [MarshalAs(UnmanagedType.I4)]
        public int diTimeRestPos;
        [MarshalAs(UnmanagedType.I4)]
        public int diTimeForerunPos;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 iState;


        /////////////////////////////////
        // Nur beim Hydraulik-Zylinder
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bIsHydraulicCyl;
        //[MarshalAs(UnmanagedType.R8)]
        //public double lrAnSensVal;
        //[MarshalAs(UnmanagedType.R8)]
        //public double lrAnSensSetpointHigh;
        //[MarshalAs(UnmanagedType.R8)]
        //public double lrAnSensSetpointLow;
        //[MarshalAs(UnmanagedType.R8)]
        //public double lrAnSensTol;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        //public string sAnSensUnit;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sAnSensDescription;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sValveRestDescr;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sValveForerunDescr;
        /////////////////////////////////

        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sCylName;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        //public string sSensorNameA;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        //public string sSensorNameB;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sSideCylComment;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string sMainCylName;



        /////////////////////////////////
        //////////////// Neu ////////////////////
        ////////////////////////////////////
        //[MarshalAs(UnmanagedType.U2)] // Beckhoff UINT: 16 Bit;     C# ushort: Unsignet 16-Bit integer  -> diese verwenden???
        //public ushort usModul;
        //[MarshalAs(UnmanagedType.U2)] 
        //public ushort usStation;
        //
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        //public string strBMK;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        //public string strName;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        //public string strDescription;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        //public string strSensor_BMK_A;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        //public string strSensor_BMK_B;
        //
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bManOn;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bRestPos;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bForerunPos;
        //
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bValveRest;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bValveForerun;
        //
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bHasRestPos;
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bHasForerunPos;
        //
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bManualAllowed;
        //
        //[MarshalAs(UnmanagedType.I1)]
        //public bool bIsSlideCyl;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        //public string strSideCylComment;
        //
        //[MarshalAs(UnmanagedType.I4)]
        //public int diTimeRestPos;
        //[MarshalAs(UnmanagedType.I4)]
        //public int diTimeForerunPos;
        //[MarshalAs(UnmanagedType.I2)]
        //public Int16 iState;
    }
    #endregion

    public partial class StCylinder : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }

        public bool bFirstADSUpdateDone {get; set;} // TRUE, wenn die Variablen von der SPS zum 1. Mal bereits abgeholt wurden.

        ICHPTranslate Translater = new CHPTransService();

        ////////////////////////////////////////
        ///////////// PLC To HMI  //////////////
        ////////////////////////////////////////
        #region
        private int _iModulHMI;
        public int    iModulHMI
        {
            get { return _iModulHMI; }
            set
            {
                _iModulHMI = value;
                OnPropertyChanged();
            }
        }

        private int _iStationHMI;
        public int    iStationHMI
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

        private string _strSensor_BMK_A_HMI;
        public string strSensor_BMK_A_HMI
        {
            get { return _strSensor_BMK_A_HMI; }
            set
            {
                _strSensor_BMK_A_HMI = value;
                OnPropertyChanged();
            }
        }

        private string _strSensor_BMK_B_HMI;
        public string strSensor_BMK_B_HMI 
        {
            get { return _strSensor_BMK_B_HMI; }
            set
            {
                _strSensor_BMK_B_HMI = value;
                OnPropertyChanged();
            }
        }


        ////////////////////////////////////////
        ///////////// Ansteuerung  /////////////
        private bool _bManOnHMI; // Manuelle Ansteuerung (wird von HMI geschrieben)
        public bool bManOnHMI
        {
            get
            {
                return _bManOnHMI;
            }

            set
            {
                _bManOnHMI = value;

                if (_bManOnHMI)
                    sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode - Select.png";
                else
                    sManBtnPath = "/LOET_HMI;component/Resources/Manual Mode.png";

                OnPropertyChanged();
            }
        }

        /////////////////////////////////////
        ///////////// Sensoren  /////////////
        private bool _bRestPosHMI; // Sensor Ruhelage
        public bool bRestPosHMI
        {
            get
            {
                return _bRestPosHMI;
            }

            set
            {
                _bRestPosHMI = value;
                if (_bRestPosHMI)
                    sLEDPath_RestPos = sGreenLEDOn;
                else
                    sLEDPath_RestPos = sGreenLEDOff;
                OnPropertyChanged();

            }
        }

        private bool _bForerunPosHMI; // Sensor Vorlauf
        public bool bForerunPosHMI
        {
            get
            {
                return _bForerunPosHMI;
            }

            set
            {
                _bForerunPosHMI = value;

                if (_bForerunPosHMI)
                {

                    sLEDPath_ForerunPos = sGreenLEDOn;
                }

                else
                {
                    sLEDPath_ForerunPos = sGreenLEDOff;
                }
                OnPropertyChanged();

            }
        }

        ////////////////////////////////////
        ///////////// Ventile  /////////////
        private bool _bValveRestHMI; // Ventil Ruhelage
        public bool bValveRestHMI
        {
            get
            {
                return _bValveRestHMI;
            }

            set
            {
                _bValveRestHMI = value;

                if (_bValveRestHMI)
                    BrushValveRest = BrushValveON;
                else
                    BrushValveRest = BrushValveOFF;

                OnPropertyChanged();
            }
        }

        private bool _bValveForerunHMI; // Ventil Vorlauf
        public bool bValveForerunHMI
        {
            get
            {
                return _bValveForerunHMI;
            }

            set
            {
                _bValveForerunHMI = value;

                if (_bValveForerunHMI)
                    BrushValveForerun = BrushValveON;
                else
                    BrushValveForerun = BrushValveOFF;

                OnPropertyChanged();
            }
        }

        ////////////////////////////////////
        ///////////// Zeiten  /////////////
        private Int32 _iTimeRestPosHMI; // Zeit zwischen Ventil Ansteuerung und Sensorsignal (Ruhelage)
        public Int32 iTimeRestPosHMI
        {
            get
            {
                return _iTimeRestPosHMI;
            }

            set
            {
                _iTimeRestPosHMI = value;

                sTextRestPosTime = _iTimeRestPosHMI.ToString() + " ms";
                OnPropertyChanged();
            }
        }


        private Int32 _iTimeForerunPosHMI; // Zeit zwischen Ventil Ansteuerung und Sensorsignal (Vorlauf)
        public Int32 iTimeForerunPosHMI
        {
            get
            {
                return _iTimeForerunPosHMI;
            }

            set
            {
                _iTimeForerunPosHMI = value;

                sTextForerunTime = _iTimeForerunPosHMI.ToString() + " ms";
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////////////
        ///////////// Nur beim Hydraulik-Zyl. ////////////
        private bool _bIsHydraulicCylHMI;
        public bool bIsHydraulicCylHMI
        {
            get { return _bIsHydraulicCylHMI; }
            set
            {
                _bIsHydraulicCylHMI = value;
                OnPropertyChanged();
            }
        }


        private bool _bManAOnHMI;
        public bool bManAOnHMI
        {
            get { return _bManAOnHMI; }
            set
            {
                _bManAOnHMI = value;
                OnPropertyChanged();
            }
        }

        private bool _bManBOnHMI;
        public bool bManBOnHMI
        {
            get { return _bManBOnHMI; }
            set
            {
                _bManBOnHMI = value;
                OnPropertyChanged();
            }
        }



        private double _lrAnSensValHMI;
        public double lrAnSensValHMI
        {
            get { return _lrAnSensValHMI; }
            set
            {
                _lrAnSensValHMI = value;
                OnPropertyChanged();
            }
        }

        private double _lrAnSensSetpointHighHMI;
        public double lrAnSensSetpointHighHMI
        {
            get { return _lrAnSensSetpointHighHMI; }
            set
            {
                _lrAnSensSetpointHighHMI = value;
                OnPropertyChanged();
            }
        }

        private double _lrAnSensSetpointLowHMI;
        public double lrAnSensSetpointLowHMI
        {
            get { return _lrAnSensSetpointLowHMI; }
            set
            {
                _lrAnSensSetpointLowHMI = value;
                OnPropertyChanged();
            }
        }

        private double _lrAnSensTolHMI;
        public double lrAnSensTolHMI
        {
            get { return _lrAnSensTolHMI; }
            set
            {
                _lrAnSensTolHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sAnSensUnitHMI;
        public string sAnSensUnitHMI
        {
            get { return _sAnSensUnitHMI; }
            set
            {
                _sAnSensUnitHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sAnSensDescriptionHMI;
        public string sAnSensDescriptionHMI
        {
            get { return _sAnSensDescriptionHMI; }
            set
            {
                _sAnSensDescriptionHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sValveRestDescrHMI;
        public string sValveRestDescrHMI
        {
            get { return _sValveRestDescrHMI; }
            set
            {
                _sValveRestDescrHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sValveForerunDescrHMI;
        public string sValveForerunDescrHMI
        {
            get { return _sValveForerunDescrHMI; }
            set
            {
                _sValveForerunDescrHMI = value;
                OnPropertyChanged();
            }
        }
        //////////////////////////////////////////////////
        ///////////// Zylinder Eigenschaften /////////////
        private eComponentState _eStateHMI;
        public eComponentState eStateHMI
        {
            get
            {
                return _eStateHMI;
            }

            set
            {
                _eStateHMI = value;
                /*
                switch(_eStateHMI)
                {
                    case eComponentState.CS_00_Normal:
                        brushState = brush_00_Normal;
                        textInfo = "OK";
                        break;
                    case eComponentState.CS_10_Fault:
                        brushState = brush_10_Fault;
                        textInfo = "Fehler";
                        break;
                    case eComponentState.CS_20_Manual:
                        brushState = brush_20_Manual;
                        textInfo = "Handbetrieb";
                        break;
                    case eComponentState.CS_30_Wait:
                        brushState = brush_30_Wait;
                        textInfo = "Warten";
                        break;
                    case eComponentState.CS_40_Warn:
                        brushState = brush_40_Warn;
                        textInfo = "Warnung";
                        break;
                    case eComponentState.CS_50_Mess:
                        brushState = brush_50_Mess;
                        textInfo = "Meldung";
                        break;
                }
                */
                textState = GlobalFunc.GetComponentStateTxt(_eStateHMI);
                brushState = GlobalFunc.GetComponentStateColor(_eStateHMI);

                OnPropertyChanged();
            }
        }

        private string _sCylNameHMI;
        public string sCylNameHMI
        {
            get
            {
                return _sCylNameHMI;
            }

            set
            {
                _sCylNameHMI = value;
                OnPropertyChanged();
            }
        }

        private string _sSensor_BMK_A;
        public string sSensor_BMK_A
        {
            get
            {
                return _sSensor_BMK_A;
            }

            set
            {
                _sSensor_BMK_A = value;
                OnPropertyChanged();
            }
        }

        private string _sSensor_BMK_B;
        public string sSensor_BMK_B
        {
            get
            {
                return _sSensor_BMK_B;
            }

            set
            {
                _sSensor_BMK_B = value;
                OnPropertyChanged();
            }
        }

        private string _sSideCylCommentHMI;
        public string sSideCylCommentHMI
        {
            get
            {
                return _sSideCylCommentHMI;
            }

            set
            {
                _sSideCylCommentHMI = value;
                OnPropertyChanged();
            }
        }


        private string _sMainCylNameHMI; // (nur wenn SlaveZylinder) NAme des Master-Zylinders
        public string sMainCylNameHMI
        {
            get
            {
                return _sMainCylNameHMI;
            }

            set
            {
                _sMainCylNameHMI = value;
                OnPropertyChanged();
            }
        }

        private bool _bHasRestPosHMI; // Ist der Sensor Ruhelage vorhanden?
        public bool bHasRestPosHMI
        {
            get
            {
                return _bHasRestPosHMI;
            }

            set
            {
                _bHasRestPosHMI = value;

                if (_bHasRestPosHMI)
                    VisHasRestPos = Visibility.Visible;
                else
                    VisHasRestPos = Visibility.Hidden;
                OnPropertyChanged();
            }
        }

        private bool _bHasForerunPosHMI; // Ist der Sensor Vorlauf vorhanden?
        public bool bHasForerunPosHMI
        {
            get
            {
                return _bHasForerunPosHMI;
            }

            set
            {
                _bHasForerunPosHMI = value;

                if (_bHasForerunPosHMI)
                    VisHasForerunPos = Visibility.Visible;
                else
                    VisHasForerunPos = Visibility.Hidden;
                OnPropertyChanged();
            }
        }

        private bool _bManualAllowedHMI; // Erlaubnis für manuelle Ansteuerung (bManOn)
        public bool bManualAllowedHMI
        {
            get
            {
                return _bManualAllowedHMI;
            }

            set
            {
                _bManualAllowedHMI = value;
                ManualVisibilityDecide();
                OnPropertyChanged();
            }
        }

        private bool _bIsSlideCylHMI; // Slave-Zylinder
        public bool bIsSlideCylHMI
        {
            get
            {
                return _bIsSlideCylHMI;
            }

            set
            {
                _bIsSlideCylHMI = value;
                ManualVisibilityDecide(); // die Methode entscheidet, ob das Manual-Button sichtbar sein soll

                if (_bIsSlideCylHMI) // die Bedingung entscheidet, ob das TextBlock für sSideCylComment sichtbar sein soll
                    VisIsSlide = Visibility.Visible;
                else
                    VisIsSlide = Visibility.Hidden;
                OnPropertyChanged();
            }
        }

        private string _strSideCylCommentHMI;
        public string strSideCylCommentHMI
        {
            get
            {
                return _strSideCylCommentHMI;
            }

            set
            {
                _strSideCylCommentHMI = value;
                OnPropertyChanged();
            }
        }
        #endregion

        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region
        void ManualVisibilityDecide()
        {
            if (bManualAllowedHMI && !bIsSlideCylHMI && !bIsHydraulicCylHMI)
                VisManualAllowed = Visibility.Visible;
            else
                VisManualAllowed = Visibility.Hidden;
        }

        private Visibility _visManualAllowed; // Visibility-Property für das Button für Handfunktion. Es wird durch die Variable "bManualAllowedHMI" angesteuert
        public Visibility VisManualAllowed
        {
            get
            {
                return _visManualAllowed;
            }

            set
            {
                _visManualAllowed = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visHasRestPos;  // Angesteuert durch die Variable "bHasRestPosHMI"
        public Visibility VisHasRestPos     // Verantwortlich für die Sichtbarkeit:  
        {                                   //          - des LEDs für "Zyl. in Ruhelage"
            get                             //          - der Zeit zwischen Ventil-Ansteuerung und Sensorsignal (Ruhelage)
            {
                return _visHasRestPos;
            }

            set
            {
                _visHasRestPos = value;
                OnPropertyChanged();
            }
        }


        private Visibility _visHasForerunPos; // Angesteuert durch die Variable "bHasForerunPosHMI"
        public Visibility VisHasForerunPos    // Verantwortlich für die Sichtbarkeit:  
        {                                     //          - des LEDs für "Zyl. in Vorlauf"
            get                               //          - der Zeit zwischen Ventil-Ansteuerung und Sensorsignal (Vorlauf)
            {
                return _visHasForerunPos;
            }

            set
            {
                _visHasForerunPos = value;
                OnPropertyChanged();
            }
        }

        private Visibility _visIsSlide; // Angesteuert durch die Variable "bIsSlideCylHMI" 
        public Visibility VisIsSlide
        {
            get
            {
                return _visIsSlide;
            }

            set
            {
                _visIsSlide = value;
                OnPropertyChanged();
            }
        }



        //System.Windows.Media.Brush BrushValveON = System.Windows.Media.Brushes.Blue;
        System.Windows.Media.Brush BrushValveON = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF369956"));

        System.Windows.Media.Brush BrushValveOFF = System.Windows.Media.Brushes.Transparent;
        //System.Windows.Media.Brush BrushValveOFF = System.Windows.Media.Brushes.Red;


        private System.Windows.Media.Brush _BrushValveRest;
        public System.Windows.Media.Brush BrushValveRest
        {
            get
            {
                return _BrushValveRest;
            }

            set
            {
                _BrushValveRest = value;
                OnPropertyChanged();
            }
        }


        private System.Windows.Media.Brush _BrushValveForerun;
        public System.Windows.Media.Brush BrushValveForerun
        {
            get
            {
                return _BrushValveForerun;
            }

            set
            {
                _BrushValveForerun = value;
                OnPropertyChanged();
            }
        }




        string sGreenLEDOn = "Resources/LEDs/led-green-th.png";
        string sGreenLEDOff = "Resources/LEDs/green-led-off-th.png";


        private string _sLEDPath_ForerunPos; // 
        public string sLEDPath_ForerunPos
        {
            get
            {
                return _sLEDPath_ForerunPos;
            }

            set
            {
                _sLEDPath_ForerunPos = value;
                OnPropertyChanged();
            }
        }

        private string _sLEDPath_RestPos; // 
        public string sLEDPath_RestPos
        {
            get
            {
                return _sLEDPath_RestPos;
            }

            set
            {
                _sLEDPath_RestPos = value;
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




        private string _sTextForerunTime; // (nur wenn SlaveZylinder) NAme des Master-Zylinders
        public string sTextForerunTime
        {
            get
            {
                return _sTextForerunTime;
            }

            set
            {
                _sTextForerunTime = value;
                OnPropertyChanged();
            }
        }

        private string _sTextRestPosTime;
        public string sTextRestPosTime
        {
            get
            {
                return _sTextRestPosTime;
            }

            set
            {
                _sTextRestPosTime = value;
                OnPropertyChanged();
            }
        }

        // ****************************************
        //Brush chp_brush = (Brush)Application.Current.FindResource("CHP_ColorBrush");

        //System.Windows.Media.Brush brush_00_Normal  = (Brush)Application.Current.FindResource("CHP_ColorBrush");
        //System.Windows.Media.Brush brush_10_Fault   = System.Windows.Media.Brushes.Red;
        //System.Windows.Media.Brush brush_20_Manual = System.Windows.Media.Brushes.Blue;
        //System.Windows.Media.Brush brush_30_Wait   = System.Windows.Media.Brushes.Yellow;
        //System.Windows.Media.Brush brush_40_Warn   = System.Windows.Media.Brushes.Yellow;
        //System.Windows.Media.Brush brush_50_Mess = System.Windows.Media.Brushes.Gray;

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


        ////////////////////////////////////
        ///////////// Commands /////////////
        ////////////////////////////////////
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
                if (ADSName != null)
                {
                    VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManOn", true);
                    if (bManOnHMI)
                        DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch Manual", "off");
                    else
                        DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch Manual", "on");
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

        public void WriteManualToggleOn()
        {
            if (ADSName != null)
            {
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManToggleOn", true);
                //DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch bManToggleOn", "on"); KLAUS
            }
        }
        #endregion









        #region Hydr.Zyl
        //***************************************
        //******* Hydraulik-Zyl. Anfang ********
        public bool ValveA { get; set; }
        public bool ValveB { get; set; }

        public void ValveAPressed()
        {
            if (ADSName != null)
            {
                if (!bManAOnHMI)
                    DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch ManualA ", true.ToString());
                else
                    DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch ManualA ", false.ToString());

                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManAOn", true); // Immer true senden -> kippt bManualA und löscht bManualB im SPS
            }
            else
                ;
        }

        public void ValveBPressed()
        {
            if (ADSName != null)
            {
                if (!bManBOnHMI)
                    DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch ManualB ", true.ToString());
                else
                    DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch ManualB ", false.ToString());

                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManBOn", true); // Immer true senden -> kippt bManualB und löscht bManualA im SPS
            }
            else
                ;
        }

        public void ValvesOff()
        {
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManAOn", false);
            VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManBOn", false);
            DBLog.Handler.Manual("Cylinder " + sCylNameHMI, "Switch ManualA and ManualB ", false.ToString());
        }
        //******* Hydraulik-Zyl. Ende ********
        //***************************************
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
            bManualAllowedHMI = false;
            bFirstADSUpdateDone = false;

            ADSName = sName;
            Item.sName = ADSName + ".PLC_to_HMI";
            Item.Type = typeof(ST_CylinderPLCToHMI);

            Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_CylinderPLCToHMI));

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

            ST_CylinderPLCToHMI tmp = (ST_CylinderPLCToHMI)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iTimeRestPosHMI = tmp.diTimeRestPos;
                    iTimeForerunPosHMI = tmp.diTimeForerunPos;
                    eStateHMI = (eComponentState)tmp.iState;

                    bManOnHMI = tmp.bManOn;
                    bRestPosHMI = tmp.bRestPos;
                    bForerunPosHMI = tmp.bForerunPos;

                    bValveRestHMI = tmp.bValveRest;
                    bValveForerunHMI = tmp.bValveForerun;

                    bHasRestPosHMI = tmp.bHasRestPos;
                    bHasForerunPosHMI = tmp.bHasForerunPos;

                    bManualAllowedHMI = tmp.bManualAllowed;
                    bIsSlideCylHMI = tmp.bIsSlideCyl;


                    iModulHMI = tmp.ushModul;
                    iStationHMI = tmp.ushStation;
                    strBMK_HMI = tmp.strBMK;

                    if(!bFirstADSUpdateDone)
                    {
                        //strNameHMI          = Translater.TransCylTxt(tmp.strName,        GlobalVar.Language);
                        //strDescriptionHMI   = Translater.TransCylTxt(tmp.strDescription, GlobalVar.Language);

                        strNameHMI          = Translater.TransTxt(tmp.strName,          eFBType.fb_Cyl);
                        strDescriptionHMI   = Translater.TransTxt(tmp.strDescription,   eFBType.fb_Cyl);

                        bFirstADSUpdateDone = true;
                    }

                    strSensor_BMK_A_HMI = tmp.strSensor_BMK_A;
                    strSensor_BMK_B_HMI = tmp.strSensor_BMK_B;

                    strSideCylCommentHMI = tmp.strSideCylComment;




                    //sCylNameHMI = tmp.sCylName;
                    //sSensor_BMK_A = tmp.sSensorNameA;
                    //sSensor_BMK_B = tmp.sSensorNameB;
                    //sSideCylCommentHMI = tmp.sSideCylComment;
                    //sMainCylNameHMI = tmp.sMainCylName;
                    //
                    //// Nur beim Hydraulik-Zyl:
                    //bIsHydraulicCylHMI      = tmp.bIsHydraulicCyl;
                    ////bManAOnHMI              = tmp.bManAOn;
                    ////bManBOnHMI              = tmp.bManBOn;
                    //lrAnSensValHMI          = tmp.lrAnSensVal;
                    //lrAnSensSetpointHighHMI = tmp.lrAnSensSetpointHigh;
                    //lrAnSensSetpointLowHMI  = tmp.lrAnSensSetpointLow;
                    //lrAnSensTolHMI          = tmp.lrAnSensTol;
                    //sAnSensUnitHMI          = tmp.sAnSensUnit;
                    //sAnSensDescriptionHMI   = tmp.sAnSensDescription;
                    //sValveRestDescrHMI      = tmp.sValveRestDescr;
                    //sValveForerunDescrHMI   = tmp.sValveForerunDescr;

                }
                catch(Exception ex)
                {
                    Debug.WriteLine("Error StCylinder: " + ex.Message);
                }
            }
        }

        public void Deregister()
        {
            VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
            VarCon.RemoveItem(Item);

            bFirstADSUpdateDone = false;
        }
        #endregion
    }


}

