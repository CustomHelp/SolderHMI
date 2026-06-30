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
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcTimePickerCHP.xaml
    /// </summary>
    public partial class UcTimePickerCHP : UserControl, INotifyPropertyChanged
    {
        private double _FontSizeUC;
        public double FontSizeUC
        {
            get
            {
                return _FontSizeUC;
            }
            set
            {
                _FontSizeUC = value;
                OnPropertyChanged();
            }
        }



        private bool _bShowCurrentTime;
        public bool bShowCurrentTimeByLoading
        {
            get
            {
                return _bShowCurrentTime;
            }
            set
            {
                _bShowCurrentTime = value;
                OnPropertyChanged();
            }
        }


        private int _iHour;
        public int iHour
        {
            get
            {
                return _iHour;
            }
            set
            {
                _iHour = value;
                tbHour.Text = FormatIntToTimeString(_iHour);
                OnPropertyChanged();
            }
        }

        private int _iMinutes;
        public int iMinutes
        {
            get
            {
                return _iMinutes;
            }
            set
            {
                _iMinutes = value;
                tbMinutes.Text = FormatIntToTimeString(_iMinutes);
                OnPropertyChanged();
            }
        }


        private string FormatIntToTimeString(int iInput)
        {
            string sResult = "";

            if (iInput == 0)
                sResult = "00";
            else if (iInput > 0 && iInput < 10)
                sResult = '0' + iInput.ToString();
            else
                sResult = iInput.ToString();

            return sResult;
        }




        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UcTimePickerCHP()
        {
            InitializeComponent();

            iHour = 0;
            iMinutes = 0;

        }

        private void TimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (bShowCurrentTimeByLoading)
            {
                DateTime dtCurrentTime = DateTime.Now;
                iHour    = dtCurrentTime.Hour;
                iMinutes = dtCurrentTime.Minute;
            }
        }

        private void TbHour_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TbMinutes_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MoveFocus()
        {

            tbHour.Background    = Brushes.White;
            tbMinutes.Background = Brushes.White;
        }

        private void TbHour_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbHour.Background = Brushes.DeepSkyBlue;

            StParamPLCDB<Int32> stParam = new StParamPLCDB<Int32>();
            try
            {
                stParam.Val = iHour;
            }
            catch
            {
                stParam.Val = 0;
            }

            stParam.Min = 0;
            stParam.Max = 24;

            stParam.strName = "Stunden";
            stParam.strUnit = "h";

            Window_InputNum winNumInput = new Window_InputNum(stParam);
            winNumInput.Title = "Stunden";
            winNumInput.ShowDialog(); // Das Setting-Window wird nun geöffnet

            if (winNumInput.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
            {
                iHour = Convert.ToInt32(winNumInput.Answer);
            }

            MoveFocus();
        }

        private void TbMinutes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMinutes.Background = Brushes.DeepSkyBlue;

            StParamPLCDB<Int32> stParam = new StParamPLCDB<Int32>();
            try
            {
                stParam.Val = iMinutes;
            }
            catch
            {
                stParam.Val = 0;
            }

            stParam.Min = 0;
            stParam.Max = 60;
            stParam.strName = "Minuten";
            stParam.strUnit = "min";

            Window_InputNum winNumInput = new Window_InputNum(stParam);
            winNumInput.Title = "Minuten";
            winNumInput.ShowDialog(); // Das Setting-Window wird nun geöffnet

            if (winNumInput.DialogResult == true) // Das DialogResult Property wird durch das OK-Button des Setting-Windows gesetzt (wenn ein gültiger Wert im Dialog-Fenster angegeben wird)
            {
                iMinutes = Convert.ToInt32(winNumInput.Answer);
            }
            MoveFocus();
        }
    }
}
