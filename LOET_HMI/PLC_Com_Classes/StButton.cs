using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;


namespace LOET_HMI
{
    //////////////////////////////////////////
    //// MarshalAs (Struktur in der SPS) /////
    //////////////////////////////////////////
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_ButtonPLCToHMI
    {
        [MarshalAs(UnmanagedType.I2)] //Beckhoff UINT -> 16 Bit
        public Int16 uiModul;
        [MarshalAs(UnmanagedType.I2)] //Beckhoff UINT -> 16 Bit
        public Int16 uiStation;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string strPopupHeader;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string strPopupText;


        [MarshalAs(UnmanagedType.I1)]
        public bool bAllowed;
        [MarshalAs(UnmanagedType.I1)]
        public bool bFeedback;
        [MarshalAs(UnmanagedType.I1)]
        public bool bPopup;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 iState;
    }
    #endregion


    public partial class StButton : INotifyPropertyChanged
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
            get
            {
                return _iModulHMI;
            }

            set
            {
                _iModulHMI = value;
                OnPropertyChanged();
            }
        }

        private int _iStationHMI;
        public int iStationHMI
        {
            get
            {
                return _iStationHMI;
            }

            set
            {
                _iStationHMI = value;
                OnPropertyChanged();
            }
        }

        ///////////////////////////////////////////////////
        private string _strNameHMI;
        public string strNameHMI
        {
            get
            {
                return _strNameHMI;
            }

            set
            {
                _strNameHMI = value;
                OnPropertyChanged();
            }
        }
        
        private string _strPopupHeaderHMI;
        public string strPopupHeaderHMI
        {
            get
            {
                return _strPopupHeaderHMI;
            }

            set
            {
                _strPopupHeaderHMI = value;
                OnPropertyChanged();
            }
        }

        private string _strPopupTextHMI;
        public string strPopupTextHMI
        {
            get
            {
                return _strPopupTextHMI;
            }

            set
            {
                _strPopupTextHMI = value;
                OnPropertyChanged();
            }
        }

        ///////////////////////////////////////////////////////
        private bool _bAllowedHMI;
        public bool bAllowedHMI
        {
            get
            {
                return _bAllowedHMI;
            }

            set
            {
                _bAllowedHMI = value;
                OnPropertyChanged();
            }
        }

        private bool _bFeedbackHMI;
        public bool bFeedbackHMI
        {
            get
            {
                return _bFeedbackHMI;
            }

            set
            {
                _bFeedbackHMI = value;

                if(_bFeedbackHMI)
                    BrushBtnBackground = BrushFeedbackOn;
                else
                    BrushBtnBackground = BrushFeedbackOff;

                OnPropertyChanged();
            }
        }

        private bool _bPopupHMI;
        public bool bPopupHMI
        {
            get
            {
                return _bPopupHMI;
            }

            set
            {
                _bPopupHMI = value;
                OnPropertyChanged();
            }
        }

        ///////////////////////////////////////////////////////
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
                OnPropertyChanged();
            }
        }
        #endregion

        ////////////////////////////////////////
        ////// Hochsprachen-Eigenschaften //////
        ////////////////////////////////////////
        #region
        //System.Windows.Media.Brush BrushProcessOn = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF369956"));
        //public System.Windows.Media.Brush BrushProcessOn = System.Windows.Media.Brushes.LawnGreen;
        //System.Windows.Media.Brush BrushProcessOff = System.Windows.Media.Brushes.Gray;
        //System.Windows.Media.Brush BrushProcessOff = new Button().Background;
        //public System.Windows.Media.Brush BrushProcessOff = new Button().Background; // kommt von der UserControl, da diese Farbe beliebig gewählt werden kann


        private System.Windows.Media.Brush _BrushFeedbackOn;
        public System.Windows.Media.Brush BrushFeedbackOn
        {
            get
            {
                return _BrushFeedbackOn;
            }

            set
            {
                _BrushFeedbackOn = value;
                if (bFeedbackHMI)
                    BrushBtnBackground = BrushFeedbackOn;
                OnPropertyChanged();
            }
        }

        private System.Windows.Media.Brush _BrushFeedbackOff;
        public System.Windows.Media.Brush BrushFeedbackOff
        {
            get
            {
                return _BrushFeedbackOff;
            }

            set
            {
                _BrushFeedbackOff = value;
                if (!bFeedbackHMI)
                    BrushBtnBackground = BrushFeedbackOff;
                OnPropertyChanged();
            }
        } 


        private System.Windows.Media.Brush _BrushBtnBackground;
        public System.Windows.Media.Brush BrushBtnBackground // Background-Property wird damit verknüpft
        {
            get
            {
                return _BrushBtnBackground;
            }

            set
            {
                _BrushBtnBackground = value;
                OnPropertyChanged();
            }
        }

        #endregion

        ////////////////////////////////////
        ///////////// Commands /////////////
        ////////////////////////////////////
        #region
        public ICommand cmdActive
        {
            get
            {
                return new RelayCommand(() => { WritePressed(); });
            }
        }

        public void WritePressed()
        {
            if (ADSName != null)
            {
                VarCon.WriteItem(ADSName + ".HMI_TO_PLC.bButtonPressed", true);
                //DBLog.Handler.Manual("Button " + sCylNameHMI, "SwitchButton ", (!bManOnHMI).ToString());
            }
            else
                ;
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

        public void Register(string sName)
        {
            //bManualAllowedHMI = false;

            ADSName = sName;
            Item.sName = ADSName + ".PLC_to_HMI";
            Item.Type = typeof(ST_ButtonPLCToHMI);

            Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_ButtonPLCToHMI));

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

            ST_ButtonPLCToHMI tmp = (ST_ButtonPLCToHMI)Item.Value;
            if (tmp != null)
            {
                try
                {
                    iModulHMI          = tmp.uiModul;
                    iStationHMI        = tmp.uiStation;

                    strNameHMI         = tmp.strName;
                    strPopupHeaderHMI  = tmp.strPopupHeader;
                    strPopupTextHMI    = tmp.strPopupText;

                    bAllowedHMI        = tmp.bAllowed;
                    bFeedbackHMI       = tmp.bFeedback;
                    bPopupHMI          = tmp.bPopup;

                    eStateHMI          = (eComponentState)tmp.iState;
                }
                catch
                {
                    ;
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

