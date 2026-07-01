using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace LOET_HMI.PLC_Com_Classes
{
    // ************************************************************************************
    // *************************** MarshalAs (Strukturen in der SPS)  *********************
    // ************************************************************************************
    #region
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_Order
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bReadyForNewOrder;		
        [MarshalAs(UnmanagedType.I1)]
        public bool bOrderStarted;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 udiOrderCountAct;			// Beckhoff UDINT -> 32 Bit	
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 udiOrderCountCartAct;
        [MarshalAs(UnmanagedType.I1)]
        public bool bOrderDone;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string strOrderName;			
    }
    #endregion


    public partial class StOrder : INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();
        public string ADSName { get; set; }

        // ************************************************************************************
        // ****************************** Properties - PLC_TO_HMI *****************************
        // ************************************************************************************
        #region

        private bool _bReadyForNewOrder; // Bereit für neuen Auftrag
        public bool bReadyForNewOrder
        {
            get { return _bReadyForNewOrder; }
            set
            {
                _bReadyForNewOrder = value;
                OnPropertyChanged();
            }
        }
        
        private bool _bOrderStarted; // Auftrag Gestartet
        public bool bOrderStarted
        {
            get { return _bOrderStarted; }
            set
            {
                _bOrderStarted = value;
                OnPropertyChanged();
            }
        }
        
        private int _iOrderCountAct; // Anzahl der bereits gefertigtetn Artikel
        public int iOrderCountAct
        {
            get { return _iOrderCountAct; }
            set
            {
                _iOrderCountAct = value;
                OnPropertyChanged();
            }
        }

        private int _iOrderCountCartAct; // Anzahl der bereits gefertigten Kartons 
        public int iOrderCountCartAct
        {
            get { return _iOrderCountCartAct; }
            set
            {
                _iOrderCountCartAct = value;
                OnPropertyChanged();
            }
        }

        

        private bool _bOrderDone; // Auftrag Fertig
        public bool bOrderDone
        {
            get { return _bOrderDone; }
            set
            {
                _bOrderDone = value;
                OnPropertyChanged();
            }
        }

        private string _strOrderName; // Eindeutiger Auftragsname
        public string strOrderName
        {
            get { return _strOrderName; }
            set
            {
                _strOrderName = value;
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

        // ***************************************************************************************
        // ********************************* ADS - RX ********************************************
        // ***************************************************************************************
        #region
        public void Register(string sName)
        {
            ADSName = sName;
            Item.sName = ADSName + ".PLC_to_HMI";
            Item.Type = typeof(ST_Order);

            Item = VarCon.AddItem(ADSName + ".PLC_to_HMI", typeof(ST_Order));

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

            ST_Order tmp = (ST_Order)Item.Value;
            if (tmp != null)
            {
                try
                {
                    bReadyForNewOrder   = tmp.bReadyForNewOrder;
                    bOrderStarted       = tmp.bOrderStarted;
                    iOrderCountCartAct  = (int)tmp.udiOrderCountCartAct;
                    iOrderCountAct      = (int)tmp.udiOrderCountAct;
                    bOrderDone          = tmp.bOrderDone;
                    strOrderName        = tmp.strOrderName;
                }
                catch (Exception ex)
                {
                    AppLogger.Log("StOrder.ItemChanged", ex);
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
