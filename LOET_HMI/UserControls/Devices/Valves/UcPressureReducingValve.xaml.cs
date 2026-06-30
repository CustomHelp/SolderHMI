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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LOET_HMI.UserControls.Graphical_UCs
{
    /// <summary>
    /// Interaktionslogik für UcPressureReducingValve.xaml
    /// </summary>
    public partial class UcPressureReducingValve : UserControl, INotifyPropertyChanged
    {

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _ValveName;
        public string ValveName
        {
            get
            {
                return _ValveName;
            }
            set
            {
                _ValveName = value;
                OnPropertyChanged();
            }
        }

        private string _OutputPressure;
        public string OutputPressure
        {
            get
            {
                return _OutputPressure;
            }
            set
            {
                _OutputPressure = value;
                OnPropertyChanged();
            }
        }


        public UcPressureReducingValve()
        {
            InitializeComponent();
        }
    }
}
