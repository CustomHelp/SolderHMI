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
    /// Interaktionslogik für UcTank.xaml
    /// </summary>
    public partial class UcTank : UserControl, INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _FluidName;
        public string FluidName
        {
            get
            {
                return _FluidName;
            }
            set
            {
                _FluidName = value;
                OnPropertyChanged();
            }
        }


        public UcTank()
        {
            InitializeComponent();
        }
    }
}
