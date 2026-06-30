using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOET_HMI.UserControls;

namespace LOET_HMI
{
    public enum MessTypes
    {
        KeinTyp     = 0,
        ErrorQuit   = 1,
        ErrorGone   = 2,
        ErrorMess   = 3,
        Warning     = 4,
        Waiting     = 5,
        Message     = 6,
        Manual      = 7,
        ModulMess   = 8,
    }

    public class MessageArchivist
    {
        /*
        List<HMIMessage> NewMessageList = new List<HMIMessage>();
        List<db_message> ActiveMessageList = new List<db_message>();
        int index, i;

        public List<HMIMessage> SplitMessages(List<string> Messages)
        {
            List<HMIMessage> RetList = new List<HMIMessage>();

            string[] splitString = new string[2];

            for (int i = 0; i < Messages.Count; i++)
            {
                HMIMessage RetVal = new HMIMessage();
                splitString = Messages[i].Split('~');
                RetVal.sText = splitString[0];
                try
                {
                    RetVal.eTypeRiedl = (MessTypes)Convert.ToInt16(splitString[1]);
                }
                catch { RetVal.eTypeRiedl = 0; }
                RetList.Add(RetVal);
            }
            return RetList;
        }

        public List<db_message> SetMessages(List<string> ListMess)
        {
            // iType rausholen
            NewMessageList = SplitMessages(ListMess);

            // Neue Message suchen
            for (i = 0; i < NewMessageList.Count; i++)
            {
                // Suche aktuelle Messages in alter Liste
                index = ActiveMessageList.FindIndex(r => r.sMessage == NewMessageList[i].sText);

                if (index == -1)
                {
                    // nicht gefunden => neue Message
                    db_message newMess = new db_message();
                    newMess.dtCome = DateTime.Now;
                    newMess.dtGone = new DateTime();
                    newMess.dtQuit = new DateTime();
                    newMess.sMessage = NewMessageList[i].sText;
                    newMess.iType = (int)NewMessageList[i].eTypeRiedl;

                    ActiveMessageList.Add(newMess);
                }
                else
                {
                    // Typänderung wenn Gegangen erkannt wurde
                    if ((NewMessageList[i].eTypeRiedl == MessTypes.ErrorGone) && (ActiveMessageList[index].iType == 1))
                    {
                        ActiveMessageList[index].dtGone = DateTime.Now;
                        ActiveMessageList[index].iType = 2;
                    }
                }
            }

            // Quittierte Message suchen
            for (i = 0; i < ActiveMessageList.Count; i++)
            {
                // Suche alte Messages in neuer Liste
                index = NewMessageList.FindIndex(r => r.sText == ActiveMessageList[i].sMessage);

                if (index == -1)
                {
                    // nicht gefunden => Message wurde quittiert
                    using (CHP_HMIEntities context = new CHP_HMIEntities())
                    {
                        // Selbstquittierend
                        if ((ActiveMessageList[i].iType > (int)MessTypes.ErrorGone)
                            || (ActiveMessageList[i].dtGone == new DateTime())) /// Weiß nicht ob man das lassen sollte
                            ActiveMessageList[i].dtGone = DateTime.Now;

                        // Type wieder auf Fehler setzen für Anziege im Meldearchiv
                        if (ActiveMessageList[i].iType == (int)MessTypes.ErrorGone)
                            ActiveMessageList[i].iType = (int)MessTypes.ErrorQuit;

                        ActiveMessageList[i].dtQuit = DateTime.Now;
                        if (GlobalVar.ActUser != null)
                            ActiveMessageList[i].sUserName = GlobalVar.ActUser.sUserName;
                        else
                            ActiveMessageList[i].sUserName = "null";

                        context.db_message.Add(ActiveMessageList[i]);
                        context.SaveChanges();

                        ActiveMessageList.RemoveAt(i);

                    }
                }
                // else interessiert nicht, wenn gefunden ist die Message immer da und wird angezeigt
            }

            return ActiveMessageList;
        }
        */
    }
}
