using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgOrder.xaml
    /// </summary>
    /// 
    public class OrderHMI : db_orderqueue
    {
        //public int iCartonQuantProPalett { get; set; }
        public string sParamSetname { get; set; }
        public DateTime dtSentToPLC { get; set; }
        //public eOrderStartMode eStartModeNextOrder {get; set;}
    }


    public partial class PgOrder : Page, INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private ADSItem Item = new ADSItem();

        //public CollectionViewSource itemCollectionViewSourceOrder = new CollectionViewSource();
        //public List<OrderProduction> listOrder = new List<OrderProduction>();
        public List<OrderHMI> listOrderWithNames = new List<OrderHMI>();

        public bool bWaitingForFirstPLCtoHMIData { get; set; } // Nachdem die ADS-Kommunikation aktiv ist, ca. 1-2 Sekunden warten, damit alle PLCtoHMI-Bits von der SPS ausgelesen werden. Erst dann darf die restliche Logik beginnen
        public bool bPgOrderInitialized { get; set; }

        public bool bLoadingNewOrderSuccesful { get; set; }
        public int iOrderManagementStep { get; set; }
        public int iCountAttemptSendingOrder { get; set; }

        public bool bShowStartNextOrderWindow { get; set; }
        public int iTmpCountOrderList { get; set; }

        public bool bADSRegistered { get; set; }

        private System.Windows.Threading.DispatcherTimer timerOrderData = new System.Windows.Threading.DispatcherTimer();


        // ***********************************************************
        // ***************** Properties für Anzeige ******************
        // ***********************************************************
        #region
        private bool _bRX_ReadyForNewOrderProperty;
        public bool bRX_ReadyForNewOrderProperty
        {
            get { return _bRX_ReadyForNewOrderProperty; }
            set
            {
                _bRX_ReadyForNewOrderProperty = value;
                OnPropertyChanged();
            }
        }

        private bool _bRX_OrderStartedProperty;
        public bool bRX_OrderStartedProperty
        {
            get { return _bRX_OrderStartedProperty; }
            set
            {
                _bRX_OrderStartedProperty = value;
                OnPropertyChanged();
            }
        }

        private int _iRX_OrderCountActProperty;
        public int iRX_OrderCountActProperty
        {
            get { return _iRX_OrderCountActProperty; }
            set
            {
                tbQuantAct.Text = _iRX_OrderCountActProperty.ToString(); // den einen TextBlock immer aktualisieren
                if (value != _iRX_OrderCountActProperty // Wegen ADS wird der Setter hier ständig aufgerufen. Der Wert sollte allerdings nur neu geschrieben werden, wenn es sich geändert hat.
                    && bRX_OrderStartedProperty)
                {
                    _iRX_OrderCountActProperty = value;
                    DBOrder.Handler.UpdateActOrderInDB_Quant(_iRX_OrderCountActProperty);
                    RefreshDatabaseDisplays();
                    OnPropertyChanged();
                }
            }
        }

        private int _iRX_OrderCountCartActProperty;
        public int iRX_OrderCountCartActProperty
        {
            get { return _iRX_OrderCountCartActProperty; }
            set
            {
                tbQuantActCart.Text = _iRX_OrderCountCartActProperty.ToString();
                if (value != _iRX_OrderCountCartActProperty
                    && bRX_OrderStartedProperty) // Wegen ADS wird der Setter hier ständig aufgerufen. Der Wert sollte allerdings nur neu geschrieben werden, wenn es sich geändert hat.
                {
                    _iRX_OrderCountCartActProperty = value;
                    DBOrder.Handler.UpdateActOrderInDB_Carton(_iRX_OrderCountCartActProperty);
                    RefreshDatabaseDisplays();
                    OnPropertyChanged();
                }
            }
        }

        private bool _bRX_OrderDoneProperty;
        public bool bRX_OrderDoneProperty
        {
            get { return _bRX_OrderDoneProperty; }
            set
            {
                _bRX_OrderDoneProperty = value;
                OnPropertyChanged();
            }
        }

        private string _strRX_OrderNameProperty;
        public string strRX_OrderNameProperty
        {
            get { return _strRX_OrderNameProperty; }
            set
            {
                _strRX_OrderNameProperty = value;
                tbOrderName.Text = _strRX_OrderNameProperty.ToString();
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


        public PgOrder()
        {

            //bWaitingForFirstPLCtoHMIData = true;
            bPgOrderInitialized          = false;

            InitializeComponent();

            RegisterOrder();


            //timerOrderData.Interval = new TimeSpan(0, 0, 2);
            //timerOrderData.Tick += TimerOrderData_Tick; 

        }

        public void RegisterOrder()
        {
            Item = VarCon.AddItem("GVL_Order.orderHMI.PLC_TO_HMI", typeof(ST_Order)); //KLAUS 

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;

            bADSRegistered = true;
        }

        public void DeregisterOrder()
        {   
            if(bADSRegistered)
            {
                VarCon.ItemChangeEvent -= VarCon_ItemChangeEvent;
                VarCon.RemoveItem(Item);

                bADSRegistered = false;
            }
        }


        /*
        private void TimerOrderData_Tick(object sender, EventArgs e)
        {
            bWaitingForFirstPLCtoHMIData = false;
            timerOrderData.Stop();
            timerOrderData.IsEnabled = false;                           
        }
        */


        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                if (Item.iHandle == e.Item[j].iHandle)
                {
                    Item = e.Item[j];
                }
            }

            try
            {
                ST_Order PLCtoHMIrx = Item.Value as ST_Order;
                #region Variablenüberischt in der SPS
                // PLC_TO_HMI in der SPS: 		
                //      bReadyForNewOrder	 : BOOL;		  // Bereit für neuen Auftrag
                //      bOrderStarted        : BOOL;          // Auftrag Gestartet
                //      udiOrderCountAct     : UDINT;         // Anzahl der bereits gefertigtetn Artikel
                //      udiOrderCountCartAct : UDINT;         // Anzahl der bereits gefertigten Kartons 
                //      bOrderDone           : BOOL;          // Auftrag Fertig
                //      strOrderName         : STRING(30);	  // Eindeutiger Auftragsname

                // HMI_TO_PLC in der SPS:
                //      bNewOrder             : BOOL;         // Starte neuen Auftrag
                //      udiOrderCountTarget   : UDINT;        // Anzahl der zu fertigen Artikel
                //      bOrderCancel          : BOOL;         // Auftrag durch Benutzer abgebrochen
                //      bConfirmed            : BOOL;         // Bestätigunsbit
                //      strOrderName          : STRING(30);	  // Eindeutiger Auftragsname
                #endregion


                // *******************************************************************
                // *************** Übertragungsprotokoll initialisieren **************
                // *******************************************************************
                if (!bPgOrderInitialized)
                {
                    if (PLCtoHMIrx.bOrderDone)
                        iOrderManagementStep = 10;
                    else if (PLCtoHMIrx.bReadyForNewOrder)
                        iOrderManagementStep = 20;
                    else if (PLCtoHMIrx.bOrderStarted)
                        iOrderManagementStep = 40;
                    else
                        iOrderManagementStep = 10;

                    bPgOrderInitialized = true;
                }
                // *******************************************************************
                // ********************* Übertragungsprotokoll ***********************
                // *******************************************************************
                else
                {
                    // 10.: Warten auf "bOrderDone" von von der SPS
                    if (PLCtoHMIrx.bOrderDone && iOrderManagementStep == 10)
                    {
                        DBOrder.Handler.UpdateActOrderInDB_State(eOrderArchivStates.OA_30_Finished); // Die Update-Methode prüft, ob der Status schon "abgebrochen" ist. Falls ja, wird der Status nict überschrieben

                        // Für die SPS bestätigen, dass das HMI bOrderDone empfangen hat
                        VarCon.WriteItem("GVL_Order.orderHMI.HMI_TO_PLC.bConfirmed", true);
                        iOrderManagementStep = iOrderManagementStep + 10;

                        if (rbStartManually.IsChecked == true)
                            bShowStartNextOrderWindow = true;
                    }

                    // 20: Warten auf "bReadyForNewOrder" von der SPS und den nächsten Auftrag laden
                    if (iOrderManagementStep == 20)
                    {
                        if (PLCtoHMIrx.bReadyForNewOrder && !bLoadingNewOrderSuccesful)
                        {

                            // A) Nächsten Auftrag automatisch starten
                            if (rbStartAutomatic.IsChecked == true)
                            {
                                bLoadingNewOrderSuccesful = DBOrder.Handler.SendNextOrderToPLC();
                                iCountAttemptSendingOrder++;
                            }

                            // B) Nächsten Auftrag händisch starten
                            else if (rbStartManually.IsChecked == true && bShowStartNextOrderWindow)
                            {
                                ShowNextOrderWindow();
                            }


                        }
                        else if (iCountAttemptSendingOrder == 5)
                        {
                            MessageBox.Show("Die Anhahl der Versuche zum Senden des nächsten Auftrages wurde überschritten.",
                                            "Nächsten Auftrag senden",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                        else
                        {
                            if (bLoadingNewOrderSuccesful)
                            {
                                RefreshDatabaseDisplays(); // DataGrid aktualisieren, da der bisher 1. Auftrag nicht mehr in der Warteschlange ist und alle andere um 1 nach vorne verschieben                                                                    

                                iCountAttemptSendingOrder = 0; // Variable zurücksetzen

                                iOrderManagementStep = iOrderManagementStep + 10;
                            }
                        }
                    }

                    // 30: Der SPS melden, dass der neue Auftrag gesendet wurde
                    if (bLoadingNewOrderSuccesful && iOrderManagementStep == 30)
                    {
                        VarCon.WriteItem("GVL_Order.orderHMI.HMI_TO_PLC.bNewOrder", true);
                        iOrderManagementStep = iOrderManagementStep + 10;
                        bLoadingNewOrderSuccesful = false;
                    }
                    else if (!bLoadingNewOrderSuccesful && iOrderManagementStep == 30)
                    {
                        iOrderManagementStep = 20;
                    }

                    // 40: Warten auf "bOrderStarted" von der SPS
                    if (PLCtoHMIrx.bOrderStarted && iOrderManagementStep == 40)
                    {
                        DBOrder.Handler.UpdateActOrderInDB_State(eOrderArchivStates.OA_20_Started);
                        RefreshDatabaseDisplays();
                        bLoadingNewOrderSuccesful = false;
                        iOrderManagementStep = 10;

                    }
                }

                // *******************************************************************
                // ******************* Ständige aktualisierungen *********************
                // *******************************************************************
                iRX_OrderCountActProperty = (int)PLCtoHMIrx.udiOrderCountAct;
                iRX_OrderCountCartActProperty = (int)PLCtoHMIrx.udiOrderCountCartAct;
                strRX_OrderNameProperty = PLCtoHMIrx.strOrderName;
                bRX_OrderDoneProperty = PLCtoHMIrx.bOrderDone;
                bRX_OrderStartedProperty = PLCtoHMIrx.bOrderStarted;
                bRX_ReadyForNewOrderProperty = PLCtoHMIrx.bReadyForNewOrder;

                // Taster "Nächsten Auftrag laden und starten"
                if (PLCtoHMIrx.bReadyForNewOrder)
                    BtnStartNextOrder.IsEnabled = true;
                else
                    BtnStartNextOrder.IsEnabled = false;

                // Taster "Auftrag abbrechen"
                if (PLCtoHMIrx.bOrderStarted)
                    BtnCancelActOrder.IsEnabled = true;
                else
                    BtnCancelActOrder.IsEnabled = false;

            }
            catch (Exception ex)
            {
                AppLogger.Log("PgOrder.UpdateOrderButtons", ex);
            }

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            VarCon.WriteItem("GVL_Order.orderHMI.HMI_TO_PLC.bOrderCancel", true);
            DBOrder.Handler.UpdateActOrderInDB_State(eOrderArchivStates.OA_90_Cancelled);
        }

        private void DGOrder_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDatabaseDisplays();

        }

        public void RefreshDatabaseDisplays()
        {
            EnumOrderStateConverter converter = new EnumOrderStateConverter();
            // **********************************************************************
            // ******************* Auftrag aktuell in der SPS: **********************
            // **********************************************************************
            if (GlobalVar.ActOrderInPLC != null)
            {
                if (GlobalVar.ActOrderInPLC.bActiveInPLC)
                {
                    tbParamsetNameID.Text = GlobalVar.ActOrderInPLC.sParamsetName;
                    tbState.Text = (string)converter.Convert(GlobalVar.ActOrderInPLC.iState, typeof(string), null, null);

                    tbQuantTarget.Text = GlobalVar.ActOrderInPLC.iQuantityTarget.ToString();
                    tbQuantDone.Text = GlobalVar.ActOrderInPLC.iQuantityDone.ToString();
                    tbCartonsProPallet.Text = GlobalVar.ActOrderInPLC.iCartonQuantProPallet.ToString();
                    tbCartonsDone.Text = GlobalVar.ActOrderInPLC.iCartonDone.ToString();

                    tbAddedBy.Text = GlobalVar.ActOrderInPLC.sAddedByToQueue;
                    /*if(GlobalVar.ActOrderInPLC.dtAddedOnToQueue == default(DateTime))
                        tbAddedOn.Text              = "";
                    else*/
                    tbAddedOn.Text = GlobalVar.ActOrderInPLC.dtAddedOnToQueue.ToString(GlobalVar.sDateFormat);

                    /*if(GlobalVar.ActOrderInPLC.dtLoadedToPLCOn == default(DateTime))
                        tbLoadedToPLCDateTime.Text  = "";
                    else*/
                    tbLoadedToPLCDateTime.Text = GlobalVar.ActOrderInPLC.dtLoadedToPLCOn.ToString(GlobalVar.sDateFormat);

                    if (GlobalVar.ActOrderInPLC.dtStartedOn == default(DateTime))
                        tbStartedDateTime.Text = "";
                    else
                        tbStartedDateTime.Text = GlobalVar.ActOrderInPLC.dtStartedOn?.ToString(GlobalVar.sDateFormat);

                    if (GlobalVar.ActOrderInPLC.dtFinishedOn == default(DateTime))
                        tbFinishedDateTime.Text = "";
                    else
                        tbFinishedDateTime.Text = GlobalVar.ActOrderInPLC.dtFinishedOn?.ToString(GlobalVar.sDateFormat);

                    if (GlobalVar.ActOrderInPLC.dtCancelledOn == default(DateTime))
                        tbCanceledDateTime.Text = "";
                    else
                        tbCanceledDateTime.Text = GlobalVar.ActOrderInPLC.dtCancelledOn?.ToString(GlobalVar.sDateFormat);
                }
                else // aktuell gibt es keinen Auftrag in der SPS
                {
                    tbParamsetNameID.Text = "";
                    tbState.Text = "";

                    tbQuantTarget.Text = "";
                    tbQuantDone.Text = "";
                    tbCartonsProPallet.Text = "";
                    tbCartonsDone.Text = "";

                    tbAddedBy.Text = "";
                    tbAddedOn.Text = "";
                    tbLoadedToPLCDateTime.Text = "";
                    tbStartedDateTime.Text = "";
                    tbFinishedDateTime.Text = "";
                    tbCanceledDateTime.Text = "";
                }

            }
            if (GlobalVar.PrevOrderInPLC != null)
            {

                // **********************************************************************
                // ********************* Auftrag zuvor in der SPS: **********************
                // **********************************************************************
                tbParamsetNameIDPrev.Text = GlobalVar.PrevOrderInPLC.sParamsetName;
                tbStatePrev.Text = (string)converter.Convert(GlobalVar.PrevOrderInPLC.iState, typeof(string), null, null);

                tbQuantTargetPrev.Text = "-";// GlobalVar.PrevOrderInPLC.iQuantityTarget.ToString();
                tbQuantDonePrev.Text = GlobalVar.PrevOrderInPLC.iQuantityDone.ToString();
                tbCartonsProPalletPrev.Text = GlobalVar.PrevOrderInPLC.iCartonQuantProPallet.ToString();
                tbCartonsDonePrev.Text = GlobalVar.PrevOrderInPLC.iCartonDone.ToString();

                tbAddedByPrev.Text = GlobalVar.PrevOrderInPLC.sAddedByToQueue;
                tbAddedOnPrev.Text = GlobalVar.PrevOrderInPLC.dtAddedOnToQueue.ToString(GlobalVar.sDateFormat);
                tbLoadedToPLCDateTimePrev.Text = GlobalVar.PrevOrderInPLC.dtLoadedToPLCOn.ToString(GlobalVar.sDateFormat);

                if (GlobalVar.PrevOrderInPLC.dtStartedOn == default(DateTime))
                    tbStartedDateTimePrev.Text = "";
                else
                    tbStartedDateTimePrev.Text = GlobalVar.PrevOrderInPLC.dtStartedOn?.ToString(GlobalVar.sDateFormat);

                if (GlobalVar.PrevOrderInPLC.dtFinishedOn == default(DateTime))
                    tbFinishedDateTimePrev.Text = "";
                else
                    tbFinishedDateTimePrev.Text = GlobalVar.PrevOrderInPLC.dtFinishedOn?.ToString(GlobalVar.sDateFormat);

                if (GlobalVar.PrevOrderInPLC.dtCancelledOn == default(DateTime))
                    tbCanceledDateTimePrev.Text = "";
                else
                    tbCanceledDateTimePrev.Text = GlobalVar.PrevOrderInPLC.dtCancelledOn?.ToString(GlobalVar.sDateFormat);


            }

            // **********************************************************************
            // ********************* Warteschlange (DataGrid) ***********************
            // **********************************************************************
            List<db_orderqueue> listOrdersDB; // = new List<db_order>();

            listOrdersDB = DBOrder.Handler.LoadOrdersFromDB();
            listOrderWithNames = DBOrder.Handler.FindNameToOrdersInQueue(listOrdersDB);

            dGOrder.ItemsSource = listOrderWithNames;
            dGOrder.CanUserAddRows = false; // Ohne diesen Befehl erscheint eine zusätzliche leere Zeile
            
            
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.eOrderStartMode == eOrderStartMode.Automatically)
            {
                rbStartAutomatic.IsChecked = true;
                rbStartManually.IsChecked = false;
                BtnStartNextOrder.Visibility = Visibility.Hidden;
            }
            else if (GlobalVar.eOrderStartMode == eOrderStartMode.Manually)
            {
                rbStartAutomatic.IsChecked = false;
                rbStartManually.IsChecked = true;
                BtnStartNextOrder.Visibility = Visibility.Visible;
            }

        }




        //*******************************************************************
        //********************** Eintrag löschen ****************************
        //*******************************************************************

        private void DGOrder_GotFocus(object sender, RoutedEventArgs e)
        {

        }
        private void DGOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {

            if (dGOrder.SelectedItem != null)
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();

                //var row = dGOrder.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                OrderHMI selectedOrder = dGOrder.SelectedItem as OrderHMI;

                Window_DeleteOrder window_DeleteOrder = new Window_DeleteOrder(selectedOrder);
                window_DeleteOrder.ShowDialog();

                if (window_DeleteOrder.DialogResult == true)
                    RefreshDatabaseDisplays();

                GlobalFunc.PopUp_SetMainWBackgrNormal();

            }
        }



        private void DGOrder_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }



        private void DGOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void PgOrd_Unloaded(object sender, RoutedEventArgs e)
        {
            dGOrder.ItemsSource = null;
        }



        private void BtnStartNextOrder_Click(object sender, RoutedEventArgs e)
        {
            ShowNextOrderWindow();
        }

        public void ShowNextOrderWindow()
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                List<db_orderqueue> listOrders = new List<db_orderqueue>();
                try // Warteschlange der Aufträge ist nicht leer
                {
                    listOrders = context.db_orderqueue.ToList();
                    iTmpCountOrderList = listOrders.Count;
                }
                catch // Warteschlange der Aufträge ist leer
                {
                    iTmpCountOrderList = 0;
                }
            }

            if (iTmpCountOrderList > 0)
            {
                GlobalFunc.PopUp_SetMainWBackgrDark();

                Window_StartNextOrderManually winNextOrder = new Window_StartNextOrderManually();
                winNextOrder.ShowDialog();

                if (winNextOrder.DialogResult == true)
                    bLoadingNewOrderSuccesful = true;

                GlobalFunc.PopUp_SetMainWBackgrNormal();
            }
            else
            {
                /*
                GlobalVar.ActOrderInPLC.sParamSetname = "-";
                GlobalVar.ActOrderInPLC.iQuantity = 0;
                GlobalVar.ActOrderInPLC.dtSentToPLC = DateTime.MinValue;
                GlobalVar.ActOrderInPLC.sAddedBy = "-";
                GlobalVar.ActOrderInPLC.dtAddedOn = DateTime.MinValue;
                */
            }
            RefreshDatabaseDisplays();
            bShowStartNextOrderWindow = false; // nur für den automatischen Aufruf relevant
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_actual dbAct = context.db_actual.Single(x => x.id == 1);

                if (rbStartAutomatic.IsChecked == true)
                {
                    dbAct.iOrderStartMode = (int)eOrderStartMode.Automatically;
                    GlobalVar.eOrderStartMode = eOrderStartMode.Automatically;
                    BtnStartNextOrder.Visibility = Visibility.Hidden;

                }
                else if (rbStartManually.IsChecked == true)
                {
                    dbAct.iOrderStartMode = (int)eOrderStartMode.Manually;
                    GlobalVar.eOrderStartMode = eOrderStartMode.Manually;
                    BtnStartNextOrder.Visibility = Visibility.Visible;
                }

                context.SaveChanges();
            }
        }
    }
}
