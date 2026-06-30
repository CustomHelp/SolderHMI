using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Drawing;
using LOET_HMI;
using TwinCAT.Ads;
using System.IO;

namespace LOET_HMI
{
    public partial class StGrCylinderHMI : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }


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

                /*
                if (_bManOnHMI)
                    sManBtnPath = "Resources/OP3Manual.png";
                else
                    sManBtnPath = "Resources/Manual_MainButton.png";
                 */
                //OnPropertyChanged();
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

                //OnPropertyChanged();
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
                //OnPropertyChanged();
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
                //OnPropertyChanged();
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
                //OnPropertyChanged();
            }
        }


        //////////////////////////////////////////////////
        ///////////// Zylinder Eigenschaften /////////////
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
                //OnPropertyChanged();
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
                //OnPropertyChanged();
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

        ////////////////////////////////////////
        ///////////// Visibilities /////////////
        void ManualVisibilityDecide()
        {
            /*
            if (!bManualAllowedHMI && bIsSlideCylHMI)
                VisManualOn = Visibility.Hidden;
            else
                VisManualOn = Visibility.Visible;
            */
            if (bManualAllowedHMI && !bIsSlideCylHMI)
                VisManualAllowed = Visibility.Visible;
            else
                VisManualAllowed = Visibility.Hidden;


           // OnPropertyChanged();
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
        System.Windows.Media.Brush BrushValveON = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFA7E2F0"));

        System.Windows.Media.Brush BrushValveOFF =  System.Windows.Media.Brushes.Transparent;
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




        string sGreenLEDOn  = "Resources/LEDs/led-green-th.png";
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


        private string _sManBtnPath = "Resources/Manual_MainButton.png";
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




        ////////////////////////////////////
        ///////////// Commands /////////////
        public ICommand cmdManual
        {
            get
            {
                return new RelayCommand(() => { WriteManualOn(); });
            }
        }

        public void WriteManualOn()
        {
            if (ADSName != null)
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bManOn", !bManOnHMI);
            else
                ;
        }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public void Register(string sName)
        {
            bManualAllowedHMI = false;

            ADSName = sName;
            Item.sName = ADSName + ".PLC_to_HMI";
            Item.Type = typeof(ST_CylinderPLCToHMI);

            //Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_CylinderPLCToHMI), true);
            Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_CylinderPLCToHMI));
            /*
            Item.ItemList.Add(AddItem(typeof(Int32)));
            Item.ItemList.Add(AddItem(typeof(Int32)));
            Item.ItemList.Add(AddItem(typeof(Int16)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(typeof(bool)));
            Item.ItemList.Add(AddItem(ADSName + ".PLC_to_HMI.sCylName",         typeof(string)));
            Item.ItemList.Add(AddItem(ADSName + ".PLC_to_HMI.sSensorNameA",     typeof(string)));
            Item.ItemList.Add(AddItem(ADSName + ".PLC_to_HMI.sSensorNameB",     typeof(string)));
            Item.ItemList.Add(AddItem(ADSName + ".PLC_to_HMI.sSideCylComment",  typeof(string)));
            Item.ItemList.Add(AddItem(ADSName + ".PLC_to_HMI.sMainCylName",     typeof(string)));
            */

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
                    //iState = tmp.iState;

                    bManOnHMI = tmp.bManOn;
                    bRestPosHMI = tmp.bRestPos;
                    bForerunPosHMI = tmp.bForerunPos;

                    bValveRestHMI = tmp.bValveRest;
                    bValveForerunHMI = tmp.bValveForerun;

                    bHasRestPosHMI = tmp.bHasRestPos;
                    bHasForerunPosHMI = tmp.bHasForerunPos;

                    bManualAllowedHMI = tmp.bManualAllowed;
                    bIsSlideCylHMI = tmp.bIsSlideCyl;

                    //sCylNameHMI = tmp.sCylName;
                    ////sSensor_BMK_A = tmp.sSensorNameA;
                    ////sSensor_BMK_B = tmp.sSensorNameB;
                    //sSideCylCommentHMI = tmp.sSideCylComment;
                    //sMainCylNameHMI = tmp.sMainCylName;
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
    }
}

