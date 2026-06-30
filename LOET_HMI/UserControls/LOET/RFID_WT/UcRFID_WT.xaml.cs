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
    /// Interaktionslogik für UcVariant.xaml
    /// </summary>
    /// 
    public enum eRFIDMode { NoMode, Read, Write }


    public partial class UcRFID_WT : UserControl, INotifyPropertyChanged
    {

        // *******************************************************************************************
        // ********************* DependencyProperty für die Struktur von der SPS *********************
        // *******************************************************************************************
        public static readonly DependencyProperty _DpRFIDStruct = DependencyProperty.Register(
            "DpRFIDStruct", typeof(StRFID_WT), typeof(UcRFID_WT), new PropertyMetadata()); // Hinweis, warum die PropertyMetadata null ist: https://stackoverflow.com/questions/20946705/why-do-different-instances-have-same-dependency-property-value

        public StRFID_WT DpRFIDStruct
        {
            get { return (StRFID_WT)GetValue(_DpRFIDStruct); }
            set { SetValue(_DpRFIDStruct, value); }
        }


        public static readonly DependencyProperty _DpVariantList = DependencyProperty.Register(
            "DpVariantList", typeof(List<UcWTVariant>), typeof(UcRFID_WT), new PropertyMetadata()); // Hinweis, warum die PropertyMetadata null ist: https://stackoverflow.com/questions/20946705/why-do-different-instances-have-same-dependency-property-value

        public List<UcWTVariant> DpVariantList
        {
            get { return (List<UcWTVariant>)GetValue(_DpVariantList); }
            set { SetValue(_DpVariantList, value); }
        }


        // ********************************************************************************************
        // *****************************   UserControl-Inputs   ***************************************
        // ********************************************************************************************
        public static readonly DependencyProperty _RFIDMode = DependencyProperty.Register(
            "RFIDMode", typeof(eRFIDMode), typeof(UcRFID_WT), new PropertyMetadata(eRFIDMode.NoMode));

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

        private UcRFID_WT _ucReadDataLink;
        public UcRFID_WT ucReadDataLink // ein Link zur ReadData-Struktur (nur wenn RFIDMode == eRFIDMode.Write  )
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
        //public List<UcWTVariant> listVariantUC { get; set; }
        public List<TextBox> listAllTextBox { get; set; }
        public List<UcLamp> listAllLEDs { get; set; }

        public List<Binding>[] arrOfListBindings { get; set; }

        UcWTVariant tmpVariant { get; set; } // Eine temporäre Referenz/Zeiger-Variable für die aktuell bearbeitende Variante

        public UcRFID_WT()
        {
            DpRFIDStruct = new StRFID_WT(); // Hinweis, warum es hier stehen soll: https://stackoverflow.com/questions/20946705/why-do-different-instances-have-same-dependency-property-value

            DpVariantList = new List<UcWTVariant>();
            DpVariantList.Add(new UcWTVariant());
            DpVariantList.Add(new UcWTVariant());
            DpVariantList.Add(new UcWTVariant());
            DpVariantList.Add(new UcWTVariant());
            DpVariantList.Add(new UcWTVariant());

            InitializeComponent();

            DpVariantList[0] = variant1;
            DpVariantList[1] = variant2;
            DpVariantList[2] = variant3;
            DpVariantList[3] = variant4;
            DpVariantList[4] = variant5;

            listAllTextBox = new List<TextBox>();
            listAllTextBox.Add(tbWTNumber);
            listAllTextBox.Add(tbDataversion);

            arrOfListBindings = new List<Binding>[5];
            tmpVariant = new UcWTVariant();
        }

        private void UcRFID_WT_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (RFIDMode == eRFIDMode.Write)
            {
                // Aktuelle Usercontrol (Parent Usercontrol)            
                rectBackground.Fill = Brushes.Orange;

                LOET_RFID_Functions.FormatTextBox_WRITE(listAllTextBox, LOET_RFID_Functions.eTypeOfWriteVar.BYTE);

                tblDataversion.Visibility = Visibility.Hidden;
                tblWTPresen.Visibility = Visibility.Hidden;
                tbDataversion.Visibility = Visibility.Hidden;
                lampWTPresent.Visibility = Visibility.Hidden;

                btnWrite.Visibility = Visibility.Visible; // MBA 5.5.2021

                // Varianten (Sub-Usercontrols)
                for (int v = 0; v < DpVariantList.Count; v++)
                {
                    LOET_RFID_Functions.FormatTextBox_WRITE(DpVariantList[v].listTextBox_BYTE, LOET_RFID_Functions.eTypeOfWriteVar.BYTE);
                    LOET_RFID_Functions.FormatTextBox_WRITE(DpVariantList[v].listTextBox_DINT, LOET_RFID_Functions.eTypeOfWriteVar.DINT);
                    LOET_RFID_Functions.FormatTextBox_WRITE(DpVariantList[v].listTextBox_UINT, LOET_RFID_Functions.eTypeOfWriteVar.DINT);
                    LOET_RFID_Functions.FormatTextBox_WRITE(DpVariantList[v].listTextBox_REAL, LOET_RFID_Functions.eTypeOfWriteVar.REAL);
                    LOET_RFID_Functions.FormatTextBox_WRITE(DpVariantList[v].listTextBox_STRING, LOET_RFID_Functions.eTypeOfWriteVar.STRING);
                    LOET_RFID_Functions.FormatToggleBtnAndLED_WRITE(DpVariantList[v].listAllToggleBtns, DpVariantList[v].listAllLEDs);
                }
            }
            else if (RFIDMode == eRFIDMode.Read)
            {
                btnSetReadData.Visibility = Visibility.Collapsed;
                btnWrite.Visibility = Visibility.Collapsed;
                btnClearAll.Visibility = Visibility.Collapsed;

                // Aktuelle Usercontrol
                LOET_RFID_Functions.FormatTextBox_READ(listAllTextBox);

                // Varianten (Sub-Usercontrols)
                for (int v = 0; v < DpVariantList.Count; v++)
                {
                    LOET_RFID_Functions.FormatTextBox_READ(DpVariantList[v].listAllTextBox);
                    LOET_RFID_Functions.FormatToggleBtnAndLED_READ(DpVariantList[v].listAllToggleBtns, DpVariantList[v].listAllLEDs);
                }
            }
            SetBindingsOfParent();
            SetBindingsOfVariants();
            
        }



        private void SetBindingsOfParent()
        {
            try
            {
                string strPathRFID_WT = nameof(DpRFIDStruct) + ".";
                string strPathWTData  = nameof(DpRFIDStruct) + "." + nameof(DpRFIDStruct.stWT_Data) + ".";
                var tempRFID_WT = new StRFID_WT();
                var tempWTData  = new StRFID_WT.StWT_DATA();
                

                Binding myBinding1 = new Binding(strPathWTData + nameof(tempWTData.byWTNummber));
                myBinding1.Source = ucRFID_WT;
                myBinding1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tbWTNumber.SetBinding(TextBox.TextProperty, myBinding1);

                Binding myBinding2 = new Binding(strPathWTData + nameof(tempWTData.byDataVersion));
                myBinding2.Source = ucRFID_WT;
                myBinding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                tbDataversion.SetBinding(TextBox.TextProperty, myBinding2);

                Binding myBinding3 = new Binding(strPathWTData + nameof(tempWTData.bRealised));
                myBinding3.Source = ucRFID_WT;
                myBinding3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lampWTPresent.SetBinding(UcLamp._IsON, myBinding3);

                //Binding myBinding4 =new Binding(strPathRFID_WT + nameof(tempRFID_WT.bWriteAllowed));
                //myBinding4.Source = ucRFID_WT;
                //myBinding4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                //btnWrite.SetBinding(Button.IsEnabledProperty, myBinding4);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Erstellung der Bindings in " + nameof(ucRFID_WT) + " fehlgeschlagen\n\n" + ex.Message,
                                nameof(ucRFID_WT) + " Parent",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Hinwes: um nachvollzuziehen, was hier passert, bitte zuerst SetBindingsOfParent() anschauen.
        /// Hier passiert das gleiche, bloß alles mit Arrays, Listen usw. und wirkt deswegen komplizierter
        /// </summary>
        private void SetBindingsOfVariants()
        {
            try
            {
                for (int i = 0; i < DpVariantList.Count; i++)
                {
                    DpVariantList[i].iGVLArrInd_Station = iGVLArrInd_Station;

                    //******************************
                    int iTmpVariantNr = (i + 1);
                    string strVariantArrIndDP = "[" + i.ToString() + "].";
                    string strVariantPathDP = "DpRFIDStruct.stWT_Data.stVariant";
                    var tempStVariant = new StRFID_WT.StVariant(iTmpVariantNr); // erstellt nur, damit auf die Strukturkomponenten zugegriffern werden kann und diese in String umgewandelt werdenkönnen
                    //**************************    

                    arrOfListBindings[i] = new List<Binding>();
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bEnabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.strIdent)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempMelt)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempSolder)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempTune)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempRelease)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempOffset)));
                    //arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.iLegalInduktor)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fPowerPreCool)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fPowerCool)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.uiCoolingType)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.uiReheatTime)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCopperMeasurement)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS1Enabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS2Enabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS3Enabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp1Enabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp2Enabled)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bPreCoolOn)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCoolOn)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS1FeedLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS1FeedSpeed)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS1DrawbackLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS2FeedLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS2FeedSpeed)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS2DrawbackLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS3FeedLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS3FeedSpeed)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS3DrawbackLength)));
                    arrOfListBindings[i].Add(new Binding(strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byOverride)));

                    for (int k = 0; k < arrOfListBindings[i].Count; k++)
                    {
                        arrOfListBindings[i][k].Source = ucRFID_WT;
                        arrOfListBindings[i][k].UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    }

                    try
                    {
                        tmpVariant = DpVariantList.Single(v => v.iVariantNumber == iTmpVariantNr);

                        tmpVariant.tbName.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.strIdent))));
                        tmpVariant.tbTempMelt.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempMelt))));
                        tmpVariant.tbTempSolder.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempSolder))));
                        tmpVariant.tbTempTune.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempTune))));
                        tmpVariant.tbTempRelease.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempRelease))));
                        tmpVariant.tbTempOffset.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fTempOffset))));

                        tmpVariant.tbPowerPreCool.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fPowerPreCool))));
                        tmpVariant.tbPowerCool.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fPowerCool))));
                        tmpVariant.tbCoolType.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.uiCoolingType))));
                        tmpVariant.tbReheatTime.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.uiReheatTime))));


                        //tmpVariant.tbLegalInductor.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.iLegalInduktor))));
                        tmpVariant.tbDVS1FeedLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS1FeedLength))));
                        tmpVariant.tbDVS1Speed.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS1FeedSpeed))));
                        tmpVariant.tbDVS1DrawbLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS1DrawbackLength))));
                        tmpVariant.tbDVS2FeedLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS2FeedLength))));
                        tmpVariant.tbDVS2Speed.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS2FeedSpeed))));
                        tmpVariant.tbDVS2DrawbLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS2DrawbackLength))));
                        tmpVariant.tbDVS3FeedLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.fDVS3FeedLength))));
                        tmpVariant.tbDVS3Speed.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS3FeedSpeed))));
                        tmpVariant.tbDVS3DrawbLength.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byDVS3DrawbackLength))));
                        tmpVariant.tbOverride.SetBinding(TextBox.TextProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.byOverride))));

                        if (RFIDMode == eRFIDMode.Read)
                        {
                            tmpVariant.lampEnabled.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bEnabled))));

                            tmpVariant.lampCopperMeasOn.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCopperMeasurement))));

                            tmpVariant.lampPreCoolOn.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bPreCoolOn))));
                            tmpVariant.lampCoolOn.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCoolOn))));

                            tmpVariant.lampClamp1.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp1Enabled))));
                            tmpVariant.lampClamp2.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp2Enabled))));

                            tmpVariant.lampDVS1.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS1Enabled))));
                            tmpVariant.lampDVS2.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS2Enabled))));
                            tmpVariant.lampDVS3.SetBinding(UcLamp._IsON, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS3Enabled))));
                        }
                        else if (RFIDMode == eRFIDMode.Write)
                        {
                            tmpVariant.btnEnabled.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bEnabled))));

                            tmpVariant.btnCopperMeasOn.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCopperMeasurement))));

                            tmpVariant.btnPreCoolOn.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bPreCoolOn))));
                            tmpVariant.btnCoolOn.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bCoolOn))));

                            tmpVariant.btnClamp1.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp1Enabled))));
                            tmpVariant.btnClamp2.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bClamp2Enabled))));

                            tmpVariant.btnDVS1.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS1Enabled))));
                            tmpVariant.btnDVS2.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS2Enabled))));
                            tmpVariant.btnDVS3.SetBinding(ToggleButton.IsCheckedProperty, arrOfListBindings[i].Single(b => b.Path.Path == (strVariantPathDP + strVariantArrIndDP + nameof(tempStVariant.bDVS3Enabled))));
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erstellung der Bindings in " + nameof(ucRFID_WT) + " fehlgeschlagen\n\n" + ex.Message,
                               nameof(ucRFID_WT) + " Variants, Aufruf SetBinding()",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erstellung der Bindings in " + nameof(ucRFID_WT) + " fehlgeschlagen\n\n" + ex.Message,
                               nameof(ucRFID_WT) + " Variants, Erstellung der Bindings",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }


        public void ADSRegister()
        {
            if (RFIDMode == eRFIDMode.Read)
            {
                DpRFIDStruct.Register("GVL_PF_RFID.g_arrRFID_WT[" + iGVLArrInd_Station.ToString() + "]");
            }
        }

        public void ADSDeregister()
        {
            if (RFIDMode == eRFIDMode.Read)
            {
                DpRFIDStruct.Deregister();
            }
        }

        private void UcRFID_WT_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ShowAnimationBusy()
        {
            Window win = new Window();
            win.Content = new UcAnimationBusy();
            win.Show();

        }

        #region Buttons
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

        private void BtnSetReadData_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
            {

                DpRFIDStruct.stWT_Data.byWTNummber = ucReadDataLink.DpRFIDStruct.stWT_Data.byWTNummber;

                for (int i = 0; i < DpVariantList.Count; i++)
                {
                    DpRFIDStruct.stWT_Data.stVariant[i].strIdent = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].strIdent;

                    DpRFIDStruct.stWT_Data.stVariant[i].fTempMelt = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fTempMelt;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempSolder = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fTempSolder;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempTune = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fTempTune;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempRelease = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fTempRelease;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempOffset = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fTempOffset;
                    //DpRFIDStruct.stWT_Data.stVariant[i].iLegalInduktor = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].iLegalInduktor;
                    DpRFIDStruct.stWT_Data.stVariant[i].fPowerPreCool = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fPowerPreCool;
                    DpRFIDStruct.stWT_Data.stVariant[i].fPowerCool = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fPowerCool;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS1FeedLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fDVS1FeedLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS1FeedSpeed = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS1FeedSpeed;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS1DrawbackLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS1DrawbackLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS2FeedLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fDVS2FeedLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS2FeedSpeed = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS2FeedSpeed;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS2DrawbackLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS2DrawbackLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS3FeedLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].fDVS3FeedLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS3FeedSpeed = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS3FeedSpeed;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS3DrawbackLength = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byDVS3DrawbackLength;
                    DpRFIDStruct.stWT_Data.stVariant[i].byOverride = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].byOverride;
                    DpRFIDStruct.stWT_Data.stVariant[i].uiReheatTime = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].uiReheatTime;


                    DpRFIDStruct.stWT_Data.stVariant[i].bEnabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bEnabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS1Enabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bDVS1Enabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS2Enabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bDVS2Enabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS3Enabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bDVS3Enabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bClamp1Enabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bClamp1Enabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bClamp2Enabled = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bClamp2Enabled;
                    DpRFIDStruct.stWT_Data.stVariant[i].bPreCoolOn = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bPreCoolOn;
                    DpRFIDStruct.stWT_Data.stVariant[i].bCoolOn = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bCoolOn;
                    DpRFIDStruct.stWT_Data.stVariant[i].bCopperMeasurement = ucReadDataLink.DpRFIDStruct.stWT_Data.stVariant[i].bCopperMeasurement;
                }
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
                DpRFIDStruct.stWT_Data.byWTNummber = 0;
                for (int i = 0; i < DpVariantList.Count; i++)
                {
                    DpRFIDStruct.stWT_Data.stVariant[i].strIdent = "";

                    DpRFIDStruct.stWT_Data.stVariant[i].bEnabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS1Enabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS2Enabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bDVS3Enabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bClamp1Enabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bClamp2Enabled = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bPreCoolOn = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bCoolOn = false;
                    DpRFIDStruct.stWT_Data.stVariant[i].bCopperMeasurement = false;

                    DpRFIDStruct.stWT_Data.stVariant[i].fTempMelt = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempSolder = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempTune = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempRelease = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fTempOffset = 0;
                    //DpRFIDStruct.stWT_Data.stVariant[i].iLegalInduktor = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fPowerPreCool = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fPowerCool = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS1FeedLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS1FeedSpeed = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS1DrawbackLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS2FeedLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS2FeedSpeed = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS2DrawbackLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].fDVS3FeedLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS3FeedSpeed = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byDVS3DrawbackLength = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].byOverride = 0;
                    DpRFIDStruct.stWT_Data.stVariant[i].uiReheatTime = 0;
                }
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
        #endregion
    }
}
