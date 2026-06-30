using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LOET_HMI.Displays;
using System.Threading;
using System.Windows.Data;

namespace LOET_HMI
{

    static public class DBOrder
    {
        static private DBOrderHandler _DBOrderHandler;
        static public DBOrderHandler Handler
        {
            get { return _DBOrderHandler; }
        }

        static DBOrder()
        {
            _DBOrderHandler = new DBOrderHandler();
        }
    }




    public class DBOrderHandler
    {

        public bool AddNewOrder(int iParamsetID, int iQuant, int iCartonQuantProPallet)
        {
            bool bResult = false;

            List<db_orderqueue> listOrders = new List<db_orderqueue>();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                int iActMaxQueue = 0; ;
                try
                {
                    if (context.db_orderqueue.Count() > 0)
                    {
                        iActMaxQueue = context.db_orderqueue.Max(x => x.iNrInQueue);
                    }
                }
                catch
                {
                    ;
                }

                db_orderqueue NewOrder = new db_orderqueue();
                NewOrder.iNrInQueue = iActMaxQueue + 1;
                NewOrder.iParamSetId = iParamsetID;
                NewOrder.iQuantity = iQuant;
                NewOrder.sAddedBy = GlobalVar.ActUser.sUserName;
                NewOrder.dtAddedOn = DateTime.Now;
                NewOrder.iCartonQuantProPallet = iCartonQuantProPallet;


                List<db_parameter> listParam = DBParam.Handler.GetParamListOverId(iParamsetID);
                try
                {
                    db_parameter paramCartoonQuant = listParam.Single(x => (x.iParamSetId == iParamsetID && x.sADSName == GlobalVar.Orders.sCartonProPallet_ADSName));
                }
                catch
                {
                    MessageBox.Show("Der zum Auftrag verwendete Sonderparameter 'Kartonanzahl pro Palette' wurde im Parametersatz nicht gefunden.", // Text
                                    "Typparameter nicht gefunden", // Überschrift
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }


                context.db_orderqueue.Add(NewOrder);
                context.SaveChanges();

                bResult = true;
            }
            return bResult;
        }

        public List<db_orderqueue> LoadOrdersFromDB()
        {
            List<db_orderqueue> listOrders = new List<db_orderqueue>();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    listOrders = context.db_orderqueue.ToList();
                }
                catch
                {
                    MessageBox.Show("Der Auftrag konnte nicht aus der Datenbank geladen werden.", // Text
                                    "Auftrag aus Datenbank laden", // Überschrift
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            return listOrders;
        }

        public List<OrderHMI> FindNameToOrdersInQueue(List<db_orderqueue> listDBOrders)
        {
            List<OrderHMI> listOrdersWithNames = new List<OrderHMI>();
            List<db_paramset> listParamsets = new List<db_paramset>();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                listParamsets = context.db_paramset.ToList();

                for (int i = 0; i < listDBOrders.Count; i++)
                {
                    listOrdersWithNames.Add(new OrderHMI());

                    try
                    {
                        listOrdersWithNames[i].id = listDBOrders[i].id;
                        listOrdersWithNames[i].iNrInQueue = listDBOrders[i].iNrInQueue;
                        listOrdersWithNames[i].iParamSetId = listDBOrders[i].iParamSetId;
                        listOrdersWithNames[i].iQuantity = listDBOrders[i].iQuantity;
                        listOrdersWithNames[i].dtAddedOn = listDBOrders[i].dtAddedOn;
                        listOrdersWithNames[i].sAddedBy = listDBOrders[i].sAddedBy;
                        listOrdersWithNames[i].iCartonQuantProPallet = listDBOrders[i].iCartonQuantProPallet;

                        listOrdersWithNames[i].sParamSetname = listParamsets.Single(x => x.id == listDBOrders[i].iParamSetId).sName;
                    }
                    catch
                    {
                        MessageBox.Show("Der Auftrag mit ID="
                                        + listOrdersWithNames[i].id.ToString()
                                        + " konnte nicht zu einem Parametersatz zugeordnet werdeen.", // Text

                                        "Zuordnung Auftrag zu Parametersatz", // Überschrift
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }

                }
                return listOrdersWithNames;
            }
        }

        public bool SendNextOrderToPLC()
        {
            bool bResult = false;
            OrderHMI nextOrderHMI = new OrderHMI(); ;
            string sOrderName;

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {

                List<db_paramset> listParamsets = new List<db_paramset>();
                List<db_orderqueue> listOrdersQueue = new List<db_orderqueue>();

                db_paramset nextParamset;
                db_orderqueue nextOrderQueue;
                try
                {
                    // Auftragswarteschlange und Parametersetze aus DB holen                                      
                    listOrdersQueue = context.db_orderqueue.ToList();
                    listParamsets   = context.db_paramset.ToList();

                    // Den zu sendenden Auftrag und den dazugehörigen Parametersatz von der jeweiligen Liste holen
                    nextOrderQueue  = listOrdersQueue.Single(x => x.iNrInQueue == 1);
                    nextParamset    = listParamsets.Single(x => x.id == nextOrderQueue.iParamSetId);

                    // "Kartonanzahl pro Palette" zuerst sicherheitshalber vom Auftrag in den Parametersatz laden (falls es durch einen anderen Auftrag, der den gleichen Parametersatz nutzt, überschrieben wurde)
                    db_parameter paramCarton = context.db_parameter.Single(x =>     x.sADSName == GlobalVar.Orders.sCartonProPallet_ADSName     &&      x.iParamSetId == nextParamset.id);
                    paramCarton.sValue       = nextOrderQueue.iCartonQuantProPallet.ToString();
                    context.SaveChanges(); // Änderungen in der Datenbank speichern

                    // Auftrag für das Senden an SPS und das Archivieren in der DB vorbereiten:
                    db_orderarchiv orderToSendAndArchive           = new db_orderarchiv();
                    orderToSendAndArchive.sParamsetName            = nextParamset.sName;
                    orderToSendAndArchive.iQuantityTarget          = nextOrderQueue.iQuantity;
                    orderToSendAndArchive.iCartonQuantProPallet    = nextOrderQueue.iCartonQuantProPallet;
                    orderToSendAndArchive.sAddedByToQueue          = nextOrderQueue.sAddedBy;
                    orderToSendAndArchive.dtAddedOnToQueue         = nextOrderQueue.dtAddedOn;
                    orderToSendAndArchive.dtLoadedToPLCOn          = DateTime.Now;
                    orderToSendAndArchive.iState                   = (int)eOrderArchivStates.OA_10_Sent;
                    orderToSendAndArchive.bActiveInPLC             = true;

                    // Auftrag in die SPS laden Teil 1/2 (Parametersatz):
                    DBParam.Handler.LoadParamSetToPLC(nextParamset.id, ParamSetTypes.Type);


                    // Eindeutigen Namen für den Auftrag generieren:
                    /*  HEINRICH
                    sOrderName = "Q"
                                + orderToSendAndArchive.iQuantityTarget.ToString()
                                + " - "
                                + orderToSendAndArchive.sParamsetName;

                    if (sOrderName.Length > 30) // Name begrenzen, falls nötig
                        sOrderName = sOrderName.Substring(0, 30);
                    */
                    sOrderName = orderToSendAndArchive.iQuantityTarget.ToString();
                    // Auftrag in die SPS laden Teil 2/2 (Menge und Name):
                    IADSConnection VarCon = new ADSService();
                    VarCon.WriteItem("GVL_Order.orderHMI.HMI_TO_PLC.udiOrderCountTarget", orderToSendAndArchive.iQuantityTarget); //
                    VarCon.WriteItem("GVL_Order.orderHMI.HMI_TO_PLC.strOrderName", sOrderName); // string-Länge darf max. 30 sein...


                    // Den zuvor aktuellen Auftrag der Archiv-Tabelle der DB auf "nicht mehr aktiv" setzen  
                    try
                    {
                        List<db_orderarchiv> listArchivOrders = context.db_orderarchiv.ToList();
                        if (listArchivOrders.Count > 0)
                        {
                            db_orderarchiv lastArchivOrder = listArchivOrders[listArchivOrders.Count - 1];
                            lastArchivOrder.bActiveInPLC = false;
                            context.SaveChanges(); // Änderungen in der Datenbank speichern
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Der bisher in der SPS geladene Auftrag konnte in der Datenbank nicht aktualisiert werdn." +
                                        "In der Datenbank hat er immer noch den Status: 'In der SPS aktiv'.", 
                                        "Datenbankfehler", 
                                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    // Den aktuell gesendeten Auftrag zur Archiv-Tabelle der DB hinzufügen 
                    context.db_orderarchiv.Add(orderToSendAndArchive);
                    context.SaveChanges(); // Änderungen in der Datenbank speichern

                    // Daten in globalen HMI-Variable speichern:
                    GlobalVar.UpdateActOrderInPLC();
                    GlobalVar.UpdatePrevOrderInPLC();
                    DBParam.Handler.LoadParamSetToGlobalVar(nextParamset.id, ParamSetTypes.Type);


                    // Den geladenen Auftrag von der Warteschlange entfernen und die anderen Aufträge nach vorne verschieben
                    RemoveOrderFromQueue(nextOrderQueue.id);

                    // Rückkehrwert der Funktion sezten
                    bResult = true;
                }
                catch
                {
                    MessageBox.Show("Fehler beim Laden des neuen Auftrages in die SPS.", // Text
                                    "Fehler", // Überschrift
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

            }
            return bResult;
        }




        public void RemoveOrderFromQueue(long lID)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                /////////// Den ausgewählter Auftrag entfernen
                db_orderqueue orderToRemove;

                orderToRemove = context.db_orderqueue.Single(x => x.id == lID);

                context.db_orderqueue.Remove(orderToRemove);
                context.SaveChanges();

                /////////// Neue Warteschlangennummer zuweisen
                List<db_orderqueue> listOrders;
                listOrders = context.db_orderqueue.OrderBy(o => o.iNrInQueue).ToList(); // Aufträge von der Datenbank sortiert nach "iNrInQueue" einlesen

                for (int i = 0; i < listOrders.Count; i++)
                {
                    listOrders[i].iNrInQueue = i + 1;
                }
                context.SaveChanges();


            }
        }


        public void UpdateActOrderInDB_State(eOrderArchivStates eOrderState)
        {

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    List<db_orderarchiv> listOrderarchiv = context.db_orderarchiv.ToList();
                    db_orderarchiv lastEntry = listOrderarchiv[listOrderarchiv.Count - 1];

                    if(eOrderState == eOrderArchivStates.OA_20_Started)
                    {
                        lastEntry.dtStartedOn = DateTime.Now;
                        lastEntry.iState = (int)eOrderArchivStates.OA_20_Started;
                        context.SaveChanges();
                    }
                    else if (eOrderState == eOrderArchivStates.OA_30_Finished)
                    {
                        if(lastEntry.iState != (int)eOrderArchivStates.OA_90_Cancelled)
                        {
                            lastEntry.dtFinishedOn = DateTime.Now;
                            lastEntry.iState = (int)eOrderArchivStates.OA_30_Finished;
                            context.SaveChanges();
                        }
                    }
                    else if (eOrderState == eOrderArchivStates.OA_90_Cancelled)
                    {
                        lastEntry.dtCancelledOn = DateTime.Now;
                        lastEntry.iState = (int)eOrderArchivStates.OA_90_Cancelled;
                        context.SaveChanges();
                    }
                }
                catch
                {
                    string sActionType = "";
                    if(      eOrderState == eOrderArchivStates.OA_20_Started)
                        sActionType = "Auftrag gestartet";
                    else if (eOrderState == eOrderArchivStates.OA_30_Finished)
                        sActionType = "Auftrag fertig";
                    else if (eOrderState == eOrderArchivStates.OA_90_Cancelled)
                        sActionType = "Auftrag gelöscht";

                    MessageBox.Show("Aktualisierung des aktuellen Auftrages in der Datenbank fehlgeschlagen.\n\n" +
                                    "Folgende Aktualisierung konnte nicht durchgeführt werden: \n" +
                                    "'" + sActionType + "'"
                                      , //Text
                                    "Datenbankfehler", //Caption
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

            }

            GlobalVar.UpdateActOrderInPLC();
            GlobalVar.UpdatePrevOrderInPLC();
        }

        public void UpdateActOrderInDB_Quant(int iNewCount)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    List<db_orderarchiv> listOrderarchiv = context.db_orderarchiv.ToList();
                    db_orderarchiv lastEntry = listOrderarchiv[listOrderarchiv.Count - 1];

                    lastEntry.iQuantityDone = iNewCount;
                    context.SaveChanges();

                    GlobalVar.UpdateActOrderInPLC();
                }
                catch
                {
                    MessageBox.Show("Die Stückzahl des aktuellen Auftrages konnte in der Datenbank nicht aktualisiert werden.", //Text
                                    "Datenbankfehler", //Caption
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        public void UpdateActOrderInDB_Carton(int iNewCount)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    List<db_orderarchiv> listOrderarchiv = context.db_orderarchiv.ToList();
                    db_orderarchiv lastEntry = listOrderarchiv[listOrderarchiv.Count - 1];

                    lastEntry.iCartonDone = iNewCount;
                    context.SaveChanges();

                    GlobalVar.UpdateActOrderInPLC();
                }
                catch
                {
                    MessageBox.Show("Die Kartonzahl des aktuellen Auftrages konnte in der Datenbank nicht aktualisiert werden.", //Text
                                    "Datenbankfehler", //Caption
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }


        public void ArchivOrder_MakeEmpty(db_orderarchiv orderToMakeEmpty)
        {
            if (orderToMakeEmpty != null)       // Heinrich
            {
                orderToMakeEmpty.sParamsetName = "-";
                orderToMakeEmpty.iQuantityTarget = 0;
                orderToMakeEmpty.iCartonQuantProPallet = 0;
                orderToMakeEmpty.sAddedByToQueue = "-";
                orderToMakeEmpty.dtAddedOnToQueue = default(DateTime); //DateTime.MinValue;
                orderToMakeEmpty.dtLoadedToPLCOn = default(DateTime); //DateTime.MinValue;
                orderToMakeEmpty.iState = (int)eOrderArchivStates.OA_00_NoState;
                orderToMakeEmpty.bActiveInPLC = false;

                orderToMakeEmpty.iCartonDone = 0;
                orderToMakeEmpty.iQuantityDone = 0;
                orderToMakeEmpty.dtStartedOn = null;
                orderToMakeEmpty.dtFinishedOn = null;
                orderToMakeEmpty.dtCancelledOn = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">Der Ziel-Auftrag, in den es kopiert werden soll.</param>
        /// <param name="toCopy">Der zu koierende Auftrag</param>
        public void ArchivOrder_Copy(db_orderarchiv target, db_orderarchiv toCopy)
        {
            target.sParamsetName            = toCopy.sParamsetName;
            target.iState                   = toCopy.iState;

            target.iQuantityTarget          = toCopy.iQuantityTarget;
            target.iQuantityDone            = toCopy.iQuantityDone;
            target.iCartonQuantProPallet    = toCopy.iCartonQuantProPallet;
            target.iCartonDone              = toCopy.iCartonDone;

            target.sAddedByToQueue          = toCopy.sAddedByToQueue;
            target.dtAddedOnToQueue         = toCopy.dtAddedOnToQueue;
            target.dtLoadedToPLCOn          = toCopy.dtLoadedToPLCOn;            
            target.dtStartedOn              = toCopy.dtStartedOn;
            target.dtFinishedOn             = toCopy.dtFinishedOn;
            target.dtCancelledOn            = toCopy.dtCancelledOn;
        }

        public List<db_orderarchiv> GetArchivOrderList()
        {
            List<db_orderarchiv> listReturn = new List<db_orderarchiv>();

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    listReturn = context.db_orderarchiv.ToList();
                }
                catch
                {
                    MessageBox.Show("Die Archiv-Auftrag-Tabelle konnte nicht aus der Datenbank geladen werde.", // Text
                                    "Fehler beim laden der Datenbank.", // Caption 
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            return listReturn;
        }

    }




}
