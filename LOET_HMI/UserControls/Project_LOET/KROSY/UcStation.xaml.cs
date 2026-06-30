using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using LOET_HMI.PLC_Com_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using LOET_HMI.SystemPages.PopUps;


namespace LOET_HMI.UserControls.LOET
{
    /// <summary>
    /// Interaktionslogik für UcStationObject.xaml
    /// </summary>
    public partial class UcStation : UserControl, INotifyPropertyChanged
    {
        // ********************************************************************************************
        // *****************************   UserControl-Inputs   ***************************************
        // ********************************************************************************************
        private int _UCStationNr;
        public int UCStationNr
        {
            get { return _UCStationNr; }
            set
            {
                _UCStationNr = value;
                OnPropertyChanged();
            }
        }


        private string _UCStationName;
        public string UCStationName
        {
            get { return _UCStationName; }
            set
            {
                _UCStationName = value;
                OnPropertyChanged();
            }
        }



        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ********************************************************************************************
        // ******************************** Sonstige Properties ***************************************
        // ********************************************************************************************
        public List<TextBox> listAllTextBox { get; set; }

        private int _iActObject;
        public int iActObject
        {
            get { return _iActObject; }
            set
            {
                _iActObject = value;
                OnPropertyChanged();
            }
        }



        public UcStation()
        {
            InitializeComponent();

            listAllTextBox = new List<TextBox>();
            listAllTextBox.Add(tbScancode);
            listAllTextBox.Add(tbKSIdent);
            listAllTextBox.Add(tbAmount);
            listAllTextBox.Add(tbAmountOK);
            listAllTextBox.Add(tbksidnetObject);
            listAllTextBox.Add(tbKROSYState_ObjType);
            listAllTextBox.Add(tbKROSYState_ObjState);
            listAllTextBox.Add(tbPLCState_ObjType);
            listAllTextBox.Add(tbPLCState_ObjState);
            listAllTextBox.Add(tbWTNr);
            listAllTextBox.Add(tbINDNr);
            listAllTextBox.Add(tbOverride);
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UcStatObj_Loaded(object sender, RoutedEventArgs e)
        {
            LOET_RFID_Functions.FormatTextBox_READ(listAllTextBox);
        }
    }
}
