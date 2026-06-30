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
using System.Windows.Controls.Primitives;
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcRFID.xaml
    /// </summary>
    public partial class UcWTVariant : UserControl, INotifyPropertyChanged
    {
        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        //public static readonly DependencyProperty _DpRFIDStructVariant = DependencyProperty.Register(
        //    "DpRFIDStructVariant", typeof(StRFID_WT), typeof(UcWTVariant), new PropertyMetadata(new StRFID_WT()));
        //
        //public StRFID_WT DpRFIDStructVariant
        //{
        //    get { return (StRFID_WT)GetValue(_DpRFIDStructVariant); }
        //    set { SetValue(_DpRFIDStructVariant, value); }
        //}


        //public StRFID stRFID_WT { get; set; }


        // ********************************************************************************************
        // *****************************   UserControl-Inputs   ***************************************
        // ********************************************************************************************
        public static readonly DependencyProperty _RFIDMode = DependencyProperty.Register(
            "RFIDMode", typeof(eRFIDMode), typeof(UcWTVariant), new PropertyMetadata(eRFIDMode.Read));

        public eRFIDMode RFIDMode
        {
            get { return (eRFIDMode)GetValue(_RFIDMode); }
            set { SetValue(_RFIDMode, value); }
        }


        public static readonly DependencyProperty _ColumnsSelect = DependencyProperty.Register(
            "ColumnsSelect", typeof(eColumnSelect), typeof(UcWTVariant), new PropertyMetadata(eColumnSelect.TagAndDataColumn));

        public eColumnSelect ColumnsSelect
        {
            get { return (eColumnSelect)GetValue(_ColumnsSelect); }
            set { SetValue(_ColumnsSelect, value); }
        }

        public enum eColumnSelect
        {
            OnlyTagColumn,      // nur die 1. Spalte anzeigen, in der die 
            OnlyDataColumn,     // nur die 2. Spalte anzeigen, in der die 
            TagAndDataColumn    // sowohl die 1. als auch die 2. Spalte anzeigen
        }

        private int _iGVLArrInd_Station;
        public int iGVLArrInd_Station
        {
            get
            {
                return _iGVLArrInd_Station;
            }
            set
            {
                _iGVLArrInd_Station = value;
                OnPropertyChanged();
            }
        }

        private int _iVariantNumber;
        public int iVariantNumber
        {
            get
            {
                return _iVariantNumber;
            }
            set
            {
                _iVariantNumber = value;
                tbVariantName.Text = "Variant " + _iVariantNumber.ToString();
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
        // ***************************** Sonstige Properties    ***************************************
        // ********************************************************************************************
        private bool bInitialized{get; set;}




        
        
        public List<ToggleButton> listAllToggleBtns { get; set; }
        public List<UcLamp> listAllLEDs{ get; set; }
        public List<TextBox> listAllTextBox { get; set; } // alle Textboxen hinzugefügt

        public List<TextBox> listTextBox_STRING { get; set; }
        public List<TextBox> listTextBox_BYTE   { get; set; }
        public List<TextBox> listTextBox_DINT { get; set; }
        public List<TextBox> listTextBox_UINT { get; set; }
        public List<TextBox> listTextBox_REAL   { get; set; }


        public UcWTVariant()
        {

            //stRFID_WT = _stRFID_WT;

            bInitialized = false;


            InitializeComponent();
            bInitialized = true;
            rectBackground.Visibility = Visibility.Collapsed;

            //Binding myBinding = new Binding("Text");
            //myBinding.Source = DpRFIDStruct.sBMK;
            //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //tbName.SetBinding(TextBox.TextProperty, myBinding);


            listAllToggleBtns = new List<ToggleButton>();
            listAllToggleBtns.Add(btnEnabled);
            listAllToggleBtns.Add(btnCopperMeasOn);
            listAllToggleBtns.Add(btnPreCoolOn);
            listAllToggleBtns.Add(btnCoolOn);
            listAllToggleBtns.Add(btnClamp1);
            listAllToggleBtns.Add(btnClamp2);
            listAllToggleBtns.Add(btnDVS1);
            listAllToggleBtns.Add(btnDVS2);
            listAllToggleBtns.Add(btnDVS3);

            listAllLEDs = new List<UcLamp>();
            listAllLEDs.Add(lampEnabled);
            listAllLEDs.Add(lampCopperMeasOn);
            listAllLEDs.Add(lampPreCoolOn);
            listAllLEDs.Add(lampCoolOn);
            listAllLEDs.Add(lampClamp1);
            listAllLEDs.Add(lampClamp2);
            listAllLEDs.Add(lampDVS1);
            listAllLEDs.Add(lampDVS2);
            listAllLEDs.Add(lampDVS3);


            listAllTextBox = new List<TextBox>();
            listAllTextBox.Add(tbName);
            listAllTextBox.Add(tbTempMelt);
            listAllTextBox.Add(tbTempSolder);
            listAllTextBox.Add(tbTempTune);
            listAllTextBox.Add(tbTempRelease);
            listAllTextBox.Add(tbTempOffset);
            listAllTextBox.Add(tbReheatTime);
            listAllTextBox.Add(tbPowerPreCool);
            listAllTextBox.Add(tbPowerCool);
            listAllTextBox.Add(tbCoolType);

            listAllTextBox.Add(tbDVS1Speed);
            listAllTextBox.Add(tbDVS1FeedLength);
            listAllTextBox.Add(tbDVS1DrawbLength);

            listAllTextBox.Add(tbDVS2Speed);
            listAllTextBox.Add(tbDVS2FeedLength);
            listAllTextBox.Add(tbDVS2DrawbLength);

            listAllTextBox.Add(tbDVS3Speed);
            listAllTextBox.Add(tbDVS3FeedLength);
            listAllTextBox.Add(tbDVS3DrawbLength);

            listAllTextBox.Add(tbOverride);

            listTextBox_STRING  = new List<TextBox>();
            listTextBox_BYTE    = new List<TextBox>();
            listTextBox_DINT    = new List<TextBox>();
            listTextBox_UINT = new List<TextBox>();
            listTextBox_REAL    = new List<TextBox>();

            listTextBox_STRING.Add(tbName);

            listTextBox_BYTE.Add(tbOverride);
            listTextBox_BYTE.Add(tbDVS1Speed);
            listTextBox_BYTE.Add(tbDVS1DrawbLength);
            listTextBox_BYTE.Add(tbDVS2Speed);
            listTextBox_BYTE.Add(tbDVS2DrawbLength);
            listTextBox_BYTE.Add(tbDVS3Speed);
            listTextBox_BYTE.Add(tbDVS3DrawbLength);

            //listTextBox_DINT.Add(tbLegalInductor);

            listTextBox_REAL.Add(tbTempMelt     );
            listTextBox_REAL.Add(tbTempSolder   );
            listTextBox_REAL.Add(tbTempTune     );
            listTextBox_REAL.Add(tbTempRelease  );
            listTextBox_REAL.Add(tbTempOffset);
            listTextBox_REAL.Add(tbPowerPreCool);
            listTextBox_REAL.Add(tbPowerCool);
            listTextBox_REAL.Add(tbDVS1FeedLength);
            listTextBox_REAL.Add(tbDVS2FeedLength);
            listTextBox_REAL.Add(tbDVS3FeedLength);

            listTextBox_UINT.Add(tbReheatTime);
            listTextBox_UINT.Add(tbCoolType);
        }

        private void UcRFID_Loaded(object sender, RoutedEventArgs e)
        {

            UpdateDVS1Enables();
            UpdateDVS2Enables();
            UpdateDVS3Enables();

            switch (ColumnsSelect)
            {
                case eColumnSelect.OnlyTagColumn:
                    col1.Width = new GridLength(0);
                    break;
                case eColumnSelect.OnlyDataColumn:
                    col0.Width = new GridLength(0);
                    break;
                case eColumnSelect.TagAndDataColumn:
                    ;
                    break;
            }


            //for (int i = 0; i < listTextBox.Count; i++)
            //{
            //    if (RFIDMode == eRFIDMode.Read)
            //    {
            //        listTextBox[i].IsReadOnly = true;
            //        listTextBox[i].Background = Brushes.Gray;
            //        listTextBox[i].IsEnabled = false;
            //    }
            //    else if (RFIDMode == eRFIDMode.Write)
            //    {
            //        listTextBox[i].IsReadOnly = false;
            //    }
            //}
            //
            //for (int i = 0; i < listAllToggleBtns.Count; i++)
            //{
            //    if (RFIDMode == eRFIDMode.Read)
            //    {
            //        listAllLEDs[i].Visibility = Visibility.Visible;
            //        listAllToggleBtns[i].Visibility = Visibility.Collapsed;
            //    }
            //    else if (RFIDMode == eRFIDMode.Write)
            //    {
            //        listAllLEDs[i].Visibility = Visibility.Collapsed;
            //        listAllToggleBtns[i].Visibility = Visibility.Visible;
            //    }
            //}
            //
            //if (RFIDMode == eRFIDMode.Read)
            //{
            //
            //    Binding myBinding = new Binding("DpRFIDStruct.sBMK");
            //    myBinding.Source = ucRFID;
            //    myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //    tbName.SetBinding(TextBox.TextProperty, myBinding);
            //
            //
            //    if(bInitialized)
            //    {
            //        //DpRFIDStruct.Register("GVL_PF_RFID.g_arrRFID_WT[" + iGVLArrInd_Station.ToString() + "]");
            //    }
            //}

        }

        private void UcRFID_Unloaded(object sender, RoutedEventArgs e)
        {
            if (RFIDMode == eRFIDMode.Read)
            {
                //DpRFIDStruct.Deregister();
            }
        }

        private void BtnDVS1_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDVS1Enables();
        }
        private void BtnDVS1_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDVS1Enables();
        }

        private void BtnDVS2_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDVS2Enables();
        }

        private void BtnDVS2_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDVS2Enables();
        }

        private void BtnDVS3_Checked(object sender, RoutedEventArgs e)
        {
            UpdateDVS3Enables();
        }

        private void BtnDVS3_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateDVS3Enables();
        }

        private void UpdateDVS1Enables()
        {
            if ((bool)btnDVS1.IsChecked)
            {
                tbDVS1Speed.IsEnabled = true;
                tbDVS1FeedLength.IsEnabled = true;
                tbDVS1DrawbLength.IsEnabled = true;
                tbDVS2Speed.IsEnabled = false;
                tbDVS2FeedLength.IsEnabled = false;
                tbDVS2DrawbLength.IsEnabled = false;
                btnDVS2.IsChecked = false;
            }
            else
            {
                tbDVS1Speed.IsEnabled = false;
                tbDVS1FeedLength.IsEnabled = false;
                tbDVS1DrawbLength.IsEnabled = false;
                tbDVS2Speed.IsEnabled = true;
                tbDVS2FeedLength.IsEnabled = true;
                tbDVS2DrawbLength.IsEnabled = true;
                btnDVS2.IsChecked = true;
            }
        }
        private void UpdateDVS2Enables()
        {
            if ((bool)btnDVS2.IsChecked)
            {
                tbDVS2Speed.IsEnabled = true;
                tbDVS2FeedLength.IsEnabled = true;
                tbDVS2DrawbLength.IsEnabled = true;
                tbDVS1Speed.IsEnabled = false;
                tbDVS1FeedLength.IsEnabled = false;
                tbDVS1DrawbLength.IsEnabled = false;
                btnDVS1.IsChecked = false;
            }
            else
            {
                tbDVS2Speed.IsEnabled = false;
                tbDVS2FeedLength.IsEnabled = false;
                tbDVS2DrawbLength.IsEnabled = false;
                tbDVS1Speed.IsEnabled = true;
                tbDVS1FeedLength.IsEnabled = true;
                tbDVS1DrawbLength.IsEnabled = true;
                btnDVS1.IsChecked = true;
            }
        }

        private void UpdateDVS3Enables()
        {
            if ((bool)btnDVS3.IsChecked)
            {
                tbDVS3Speed.IsEnabled = true;
                tbDVS3FeedLength.IsEnabled = true;
                tbDVS3DrawbLength.IsEnabled = true;
            }
            else
            {
                tbDVS3Speed.IsEnabled = false;
                tbDVS3FeedLength.IsEnabled = false;
                tbDVS3DrawbLength.IsEnabled = false;
            }
        }

        private void BtnEnabled_Checked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }

        private void BtnEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateEnable();
        }
        
        private void UpdateEnable()
        {

            if((bool)btnEnabled.IsChecked)
            {
                //btnPreCoolOn.IsEnabled = true;
            }
            else
            {
                //btnPreCoolOn.IsEnabled = false;
            }
            

        }

    }
}
