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

namespace LOET_HMI.UserControls.Display_UCs
{
    /// <summary>
    /// Interaktionslogik für UcOverviewDispl.xaml
    /// </summary>
    public partial class UcOverviewDispl : UserControl
    {
        private bool _Ready;
        public bool Ready
        {
            get { return _Ready; }
            set
            {
                _Ready = value;
                cbReady.IsChecked = value;
            }
        }

        private bool _PowerIsOn;
        public bool PowerIsOn
        {
            get { return _PowerIsOn; }
            set
            {
                _PowerIsOn = value;
                cbPowerOn.IsChecked = value;
            }
        }

        private bool _Error;
        public bool Error
        {
            get { return _Error; }
            set
            {
                _Error = value;
                cbError.IsChecked = value;
            }
        }

        private bool _SafteySTOActive;
        public bool SafteySTOActive
        {
            get { return _SafteySTOActive; }
            set
            {
                _SafteySTOActive = value;
                cbSafetySTOActive.IsChecked = value;
            }
        }


        public UcOverviewDispl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ready = !Ready;
        }
    }
}
