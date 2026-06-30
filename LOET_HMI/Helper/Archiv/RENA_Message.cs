using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace LOET_HMI.Klassen
{
    [Serializable]
    public class RENA_Message 
    {
        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }
            set { _MessageText = value; }
        }

        private DateTime _DateCome;
        public DateTime DateCome
        {
            get { return _DateCome; }
            set { _DateCome = value; }
        }

        private DateTime _DateGone;
        public DateTime DateGone
        {
            get { return _DateGone; }
            set { _DateGone = value; }
        }

        //private DateTime _DateAcknowledged;
        public Nullable<DateTime> DateAcknowledged { get; set; }
        /*
        {
            get { return _DateAcknowledged; }
            set { _DateAcknowledged = value; }
        }
        */
        
        private string _Modulname;
        public string Modulname
        {
            get { return _Modulname; }
            set { _Modulname = value; }
        }
        

        //private string _Comment;
        public string Comment { get; set; }
        /*
        {
            get { return _Comment; }
            set { _Comment = value; }
        }
        */
        public Nullable<DateTime> CommentDate { get; set; }

        private MessageCategory _Category;
        public MessageCategory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        

        public RENA_Message(string text, MessageCategory category, string modulname)
        {
            MessageText = text;
            Category = category;
            Modulname = modulname;
            DateCome = DateTime.Now;
            DateGone = DateTime.Today;
            DateAcknowledged = null;
            Comment = null;
            CommentDate = null;

        }

        public RENA_Message()
        {

        }

               
        public enum MessageCategory
        {
            _1_CriticalError,  // Kritische Fehler, rot
            _2_MachineError,   // Maschinen Fehler, orange
            _3_Warning,        // Warnung, gelb
            _4_Note,           // Hinweis, keine Farbe 
            _5_Notification,   // Meldungen, keine Farbe
        }
        

    }
}
