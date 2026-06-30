using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using LOET_HMI;




namespace LOET_HMI.Klassen
{
    [Serializable]
    public partial class RENA_User : INotifyPropertyChanged
    {
        private string _Username;
        public string Username
        {
            get { return _Username; }
            set { _Username = value;}
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private int _UserLevel;
        public int UserLevel
        {
            get { return _UserLevel; }
            set { _UserLevel = value; }
        }
        
        private DateTime _DateOfCreation;
        public DateTime DateOfCreation
        {
            get { return _DateOfCreation; }
            set { _DateOfCreation = value;}
        }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RENA_User(string username, string password, int level)
        {
            Username = username;
            Password = password;
            UserLevel = level;
            DateOfCreation = DateTime.Now;
        }



    }

     
}
