using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LOET_HMI.PLC_Com_Classes;

namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcRFID_IND.xaml
    /// </summary>
    public partial class UcRFID_IND : UserControl, INotifyPropertyChanged
    {
        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpRFIDStruct = DependencyProperty.Register(
            "DpRFIDStruct", typeof(StRFID_IND), typeof(UcRFID_IND), new PropertyMetadata()); // Hinweis, warum die PropertyMetadata null ist: https://stackoverflow.com/questions/20946705/why-do-different-instances-have-same-dependency-property-value

        public StRFID_IND DpRFIDStruct
        {
            get { return (StRFID_IND)GetValue(_DpRFIDStruct); }
            set { SetValue(_DpRFIDStruct, value); }
        }

        // ********************************************************************************************
        // *****************************   UserControl-Inputs   ***************************************
        // ********************************************************************************************
        public static readonly DependencyProperty _RFIDMode = DependencyProperty.Register(
            "RFIDMode", typeof(eRFIDMode), typeof(UcRFID_IND), new PropertyMetadata(eRFIDMode.NoMode));

        public eRFIDMode RFIDMode
        {
            get { return (eRFIDMode)GetValue(_RFIDMode); }
            set { SetValue(_RFIDMode, value); }
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

        private UcRFID_IND _ucReadDataLink;
        public UcRFID_IND ucReadDataLink // ein Link zur ReadData-Struktur (nur wenn RFIDMode == eRFIDMode.Write  )
        {
            get
            {
                return _ucReadDataLink;
            }
            set
            {
                _ucReadDataLink = value;
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
        public List<TextBox> listAllTextBox_BYTE { get; set; }
        public List<TextBox> listAllTextBox_DINT { get; set; }



        public UcRFID_IND()
        {
            DpRFIDStruct = new StRFID_IND();

            InitializeComponent();

            listAllTextBox_BYTE = new List<TextBox>();
            listAllTextBox_BYTE.Add(tbDataversion);

            listAllTextBox_DINT = new List<TextBox>();
            listAllTextBox_DINT.Add(tbIndNumber);
        }

        private void UcRFID_IND_Loaded(object sender, RoutedEventArgs e)
        {

            if (RFIDMode == eRFIDMode.Write)
            {
                rectBackground.Fill = Brushes.Orange;

                LOET_RFID_Functions.FormatTextBox_WRITE(listAllTextBox_BYTE, LOET_RFID_Functions.eTypeOfWriteVar.BYTE);
                LOET_RFID_Functions.FormatTextBox_WRITE(listAllTextBox_DINT, LOET_RFID_Functions.eTypeOfWriteVar.DINT);


                tblDataversion.Visibility = Visibility.Hidden;
                tblIndPresent.Visibility = Visibility.Hidden;
                tbDataversion.Visibility = Visibility.Hidden;
                lampIndPresent.Visibility = Visibility.Hidden;
            }
            else if (RFIDMode == eRFIDMode.Read)
            {
                btnSetReadData.Visibility = Visibility.Collapsed;
                btnWrite.Visibility = Visibility.Collapsed;
                btnClearAll.Visibility = Visibility.Collapsed;

                LOET_RFID_Functions.FormatTextBox_READ(listAllTextBox_BYTE);
                LOET_RFID_Functions.FormatTextBox_READ(listAllTextBox_DINT);
            }

            SetBindings();           
        }

        private void SetBindings()
        {
            try
            {
                string strPath = nameof(DpRFIDStruct) + "." + nameof(DpRFIDStruct.stInduktor_Data) + ".";
                var tempWTData = new StRFID_IND.StInduktor_Data();

                Binding myBinding1 = new Binding(strPath + nameof(tempWTData.iInduktorNumber));
                myBinding1.Source = ucRFID_IND;
                myBinding1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tbIndNumber.SetBinding(TextBox.TextProperty, myBinding1);

                Binding myBinding2 = new Binding(strPath + nameof(tempWTData.byDataVersion));
                myBinding2.Source = ucRFID_IND;
                myBinding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tbDataversion.SetBinding(TextBox.TextProperty, myBinding2);

                Binding myBinding3 = new Binding(strPath + nameof(tempWTData.bRealised));
                myBinding3.Source = ucRFID_IND;
                myBinding3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lampIndPresent.SetBinding(UcLamp._IsON, myBinding3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erstellung der Bindings in " + nameof(ucRFID_IND) + " fehlgeschlagen\n\n" + ex.Message,
                                nameof(ucRFID_IND) + " Parent",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        public void ADSRegister()
        {
            if (RFIDMode == eRFIDMode.Read)
            {
                DpRFIDStruct.Register("GVL_PF_RFID.g_arrRFID_IND[" + iGVLArrInd_Station.ToString() + "]");
            }
        }

        public void ADSDeregister()
        {
            if (RFIDMode == eRFIDMode.Read)
            {
                DpRFIDStruct.Deregister();
            }
        }

        private void UcRFID_IND_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnSetReadData_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                DpRFIDStruct.stInduktor_Data.iInduktorNumber = ucReadDataLink.DpRFIDStruct.stInduktor_Data.iInduktorNumber;
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }

        private void BtnWrite_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                DpRFIDStruct.ADSName = ucReadDataLink.DpRFIDStruct.ADSName;
                DpRFIDStruct.WriteDataToPLC();
                GlobalFunc.ShowAnimationBusy(1);
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {
                DpRFIDStruct.stInduktor_Data.iInduktorNumber = 0;
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );
            }
        }
    }
}
