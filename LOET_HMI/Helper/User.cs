using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOET_HMI
{
    public class User : INotifyPropertyChanged
    {
        private string _sUserName;
        public string sUserName
        {
            get { return _sUserName; }
            set { _sUserName = value; }
        }

        private string _sPassword;
        public string sPassword
        {
            get { return _sPassword; }
            set { _sPassword = value; }
        }

        private int _iUserLevel;
        public int iUserLevel
        {
            get { return _iUserLevel; }
            set { _iUserLevel = value; }
        }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public User(string username, string password, int level)
        {
            sUserName = username;
            sPassword = password;
            iUserLevel = level;
        }

    }


}
