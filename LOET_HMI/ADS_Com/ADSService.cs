using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;

namespace LOET_HMI
{
    public class ADSService : IADSConnection
    {
        // Hinzufügen
        public ADSItem AddItem(string sName, Type type)
        {
            ADSItem tempItem = new ADSItem();
            tempItem.sName = sName;
            tempItem.Type = type;
            tempItem.iHandle = ADSMain.ADSComServer.AddItem(sName, type);

            return tempItem;
        }


        // Schreiben 
        void IADSConnection.WriteItem(string ItemName, object Value)
        {
            ADSMain.ADSComServer.WriteItem(ItemName, Value);
        }

        // Deaktivieren
        void IADSConnection.RemoveItem(ADSItem Item)
        {
            ADSMain.ADSComServer.RemoveItem(Item.iHandle);
        }

        void IADSConnection.RemoveItemList(List<ADSItem> ItemList)
        {
            for (int i = 0; i < ItemList.Count; i++)
                ADSMain.ADSComServer.RemoveItem(ItemList[i].iHandle);
        }

        //Callbackevent
        public event EventHandler<ADSItemChangeEventArgs> ItemChangeEvent;

        protected virtual void OnItemChangeEvent(ADSItemChangeEventArgs e)
        {
            EventHandler<ADSItemChangeEventArgs> _ItemChangeEvent = ItemChangeEvent;
            if (_ItemChangeEvent != null)
            {
                _ItemChangeEvent(this, e);
            }
        }

        // Read PLC-Array 1x (MBA 19.01.2021: ??????????????? ob es hier passt, weiß ich nicht.... da es durch eine Schnittstelle implementiert wird, musste es hier so geschrieben werden, damit es kompiliert werden kann.......)
        public void ReadArray(string sNamePLCVar, int iArrayLength, int iSizeInBytes, eArrayType eType, ref List<string> listResult) // 
        {
            ADSMain.ADSComServer.ReadArray(sNamePLCVar, iArrayLength, iSizeInBytes, eType, ref listResult);
        }




        public void ADSCon_ItemChangeEvent(object objSender, ADSItemChangeEventArgs e)
        {
            OnItemChangeEvent(e);
        }

        public void EnableCallbackEvent()
        {
            ADSMain.ADSComServer.ItemChangeEvent += ADSCon_ItemChangeEvent;
        }

        public bool IsConnected()
        {
            return ADSMain.ADSComServer.Connected;
        }
    }
}
