using System;
using System.Collections.Generic;
using TwinCAT.Ads;
using System.Windows;
using System.IO;
using LOET_HMI;

namespace LOET_HMI
{
    // Define a class to hold custom event info
    public class ADSItemChangeEventArgs : EventArgs
    {
        public ADSItemChangeEventArgs(List<ADSItem> Item)
        {
            _Item = Item;
        }
        private List<ADSItem> _Item;
        public List<ADSItem> Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

    }

    public interface IADSConnection
    {
        // Hinzufügen
        ADSItem AddItem(string sName, Type type);
        void WriteItem(string ItemName, object Value);

        // Entfernen
        void RemoveItem(ADSItem Item);
        void RemoveItemList(List<ADSItem> ItemList);

        //Callbackevents
        void EnableCallbackEvent();
        event EventHandler<ADSItemChangeEventArgs> ItemChangeEvent;

        // Read PLC-Array 1x (MBA 19.01.2021)
        //List<string> ReadArray(string sNamePLCVar, int iArrayLength, int iSizeInBytes, eArrayType eType);
        void ReadArray(string sNamePLCVar, int iArrayLength, int iSizeInBytes, eArrayType eType, ref List<string> listResult); //  
    }

    public partial class ADSItem
    {
        public string sName { get; set; } // PLC-Name der Variable (Main.text)
        public Type Type { get; set; }  // Datentyp
        public int iHandle { get; set; }
        public object Value { get; set; } // was wir gelesen haben
    }

    static public class ADSMain
    {
        static private ADSConnection _ADSComServer;
        static public ADSConnection ADSComServer
        {
            get { return _ADSComServer; }
        }

        static ADSMain()
        {
            _ADSComServer = new ADSConnection();
        }
    }


    public class ADSConnection
    {
        private TcAdsClient adsClient = new TcAdsClient();
        public List<ADSItem> ItemList = new List<ADSItem>();
        private string _sAmsNetId;
        private int _iPort;

        public void SetAmsNetId(string sAmsNetId, int iPort)
        {
            _sAmsNetId = sAmsNetId;
            _iPort = iPort;
        }

        private System.Windows.Threading.DispatcherTimer timerRequestFast = new System.Windows.Threading.DispatcherTimer();
        // Verbinden 
        public bool Connect()
        {
            adsClient.Connect(_sAmsNetId, _iPort);
            

            if (Connected)
            {
                adsClient.AdsNotificationEx += new AdsNotificationExEventHandler(adsClient_AdsNotificationEx);

                timerRequestFast.Interval = new TimeSpan(0, 0, 0, 0, 50);
                timerRequestFast.Tick += new System.EventHandler(this.timerRequestFast_Tick);
                timerRequestFast.Start();

                return true;
            }
            else
                return false;
        }

        private void timerRequestFast_Tick(object sender, EventArgs e)
        { // Riedl: Anzahl Liste prüfen --- 875 letzes Max.
            ADSItemChangeEventArgs eCHP = new ADSItemChangeEventArgs(ItemList);
            OnItemChangeEvent(eCHP);
        }

        // Itemchange-Event
        private void adsClient_AdsNotificationEx(object sender, AdsNotificationExEventArgs e)
        {

            try
            {
                int index = ItemList.FindIndex(s => (s.iHandle == e.NotificationHandle));

                ItemList[index].Value = e.Value;
            }
            catch (Exception ex)
            {
                AppLogger.Log("ADSConnection.adsClient_AdsNotificationEx", ex);
            }
        }



        // Abfragen ob wir verbunden sind
        public bool Connected
        {
            get
            {
                try
                {
                    StateInfo State = adsClient.ReadState();
                    return State.AdsState == AdsState.Run;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Item hinzufügen
        private object value;
        public int AddItem(string sName, Type type)
        {
            ADSItem tempItem = new ADSItem();
            tempItem.sName = sName;
            tempItem.Type = type;

            try
            {
                if (type == typeof(string))
                    tempItem.iHandle = adsClient.AddDeviceNotificationEx(sName, AdsTransMode.OnChange, 100, 500, value, typeof(String), new int[] { 80 });
                else
                    tempItem.iHandle = adsClient.AddDeviceNotificationEx(sName, AdsTransMode.OnChange, 100, 500, value, type);

                ItemList.Add(tempItem);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message + "\n\nItem name: " + tempItem.sName,// MBA 12.11.2020: Itemname hinzugefügt
                                ex.Source, //Titelzeile
                                MessageBoxButton.OK, 
                                MessageBoxImage.Exclamation
                                );
            }
            return tempItem.iHandle;
        }


        // Item schreiben
        public void WriteItem(string ItemName, object Value)
        {
            if (!Connected)
                Connect();
            try
            {
                adsClient.WriteSymbol(ItemName, Value, true);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\n\n" + ItemName, 
                                "Write PLC variable", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            }

        }

        // Item deaktivieren
        public void RemoveItem(int iHandle)
        {
            try
            {
                adsClient.DeleteDeviceNotification(iHandle);
                int Index = ItemList.FindIndex(s => (s.iHandle == iHandle));
                ItemList.RemoveAt(Index);
            }
            catch (Exception ex)
            {
                AppLogger.Log("ADSConnection.RemoveItem", ex);
            }
        }


        //Callbackevent 
        public delegate void CHPAdsNotificationEventHandler(object sender, ADSItemChangeEventArgs e);
        public event EventHandler<ADSItemChangeEventArgs> ItemChangeEvent;
        protected virtual void OnItemChangeEvent(ADSItemChangeEventArgs e)
        {
            EventHandler<ADSItemChangeEventArgs> _ItemChangeEvent = ItemChangeEvent;
            if (_ItemChangeEvent != null)
            {
                _ItemChangeEvent(this, e);
            }
        }

        // Verbindung Trennen
        public void Disconnect()
        {
            adsClient.Dispose();
        }


        public List<ADSItem> GetItemList()
        {
            return ItemList;
        }

        // Read PLC-Array 1x
        public void ReadArray(string sNamePLCVar, int iArrayLength, int iSizeInBytes, eArrayType eType, ref List<string> listResult)// 
        {
            //List<string> listString = new List<string>();
            //List<string> listResult = new List<string>();

            int iHandle = adsClient.CreateVariableHandle(sNamePLCVar);
            try
            {
                AdsStream dataStream = new AdsStream(iArrayLength * iSizeInBytes);
                BinaryReader binRead = new BinaryReader(dataStream);

                //Array komplett auslesen	
                adsClient.Read(iHandle, dataStream);


                listResult.Clear();
                dataStream.Position = 0;

                for (int i = 0; i < iArrayLength; i++)
                {
                    switch(eType)
                    {
                        case eArrayType.AT_100_BOOL:
                            listResult.Add(binRead.ReadBoolean().ToString());
                            break;

                        case eArrayType.AT_200_INT:
                            listResult.Add(binRead.ReadInt16().ToString());
                            break;

                        case eArrayType.AT_210_UINT:
                            listResult.Add(binRead.ReadUInt16().ToString());
                            break;

                        case eArrayType.AT_220_DINT:
                            listResult.Add(binRead.ReadInt32().ToString());
                            break;

                        case eArrayType.AT_300_REAL:
                            listResult.Add(binRead.ReadSingle().ToString());
                            break;

                        case eArrayType.AT_310_LREAL:
                            listResult.Add(binRead.ReadDouble().ToString());
                            break;
                    }
                    
                }

                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }



    }
}
