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
using LOET_HMI.SystemPages.PopUps;
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.UserControls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgMessageArchiv.xaml
    /// </summary>
    public partial class PgMessageArchiv : Page, INotifyPropertyChanged
    {
        // Elemente
        List<db_message> ListHMIAll       = new List<db_message>();
        List<db_message> ListHMIAllTransl = new List<db_message>();

        List<db_message> ListHMIFiltered  = new List<db_message>();
        //List<db_message> ListHMIFilteredTransl = new List<db_message>();


        public CollectionViewSource itemCollectionViewSourceMessages = new CollectionViewSource();

        // Tanslation
        ICHPTranslate Tanslater = new CHPTransService();

        // Filtereigenschaften
        private Nullable<System.DateTime> _dtStart;
        public Nullable<System.DateTime> dtStart
        {
            get { return _dtStart; }
            set
            {
                _dtStart = value;
                datePickerStart.SelectedDate = _dtStart.Value.Date;
                timePickerStart.iHour        = _dtStart.Value.Hour;
                timePickerStart.iMinutes     = _dtStart.Value.Minute;
                OnPropertyChanged();
            }
        }

        Nullable<System.DateTime> _dtEnd;
        Nullable<System.DateTime> dtEnd
        {
            get { return _dtEnd; }
            set
            {
                _dtEnd = value;
                datePickerEnd.SelectedDate = _dtEnd.Value.Date;
                timePickerEnd.iHour        = _dtEnd.Value.Hour;
                timePickerEnd.iMinutes     = _dtEnd.Value.Minute;
                OnPropertyChanged();
            }
        }

        public List<int> iListFilterType      { get; set; }
        public List<string> strListFilterMessage { get; set; }
        public List<string> strListFilterStation { get; set; }       
        public List<string> strListFilterBMK      { get; set; }
        public List<string> strListFilterUser     { get; set; }


        public db_message msgSelected { get; set; }


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public PgMessageArchiv()
        {
            InitializeComponent();

            dtStart = new DateTime();
            dtEnd = new DateTime();

            dtStart = DateTime.Today;
            dtEnd = DateTime.Now;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BtnFilter.IsEnabled = false;
            BtnReset.IsEnabled = false;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            BtnReset.IsEnabled = false;

            dtStart = datePickerStart.SelectedDate + new TimeSpan(timePickerStart.iHour, timePickerStart.iMinutes, 0);
            dtEnd   = datePickerEnd.SelectedDate   + new TimeSpan(timePickerEnd.iHour,   timePickerEnd.iMinutes, 0);

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    if(dtEnd == null)
                        ListHMIAll = context.db_message.Where(r => ((r.dtCome >= dtStart) )).ToList();
                    else
                        ListHMIAll = context.db_message.Where(r => (r.dtCome >= dtStart) && ((r.dtGone <= dtEnd) || (r.dtGone == null))).ToList();

                    

                    // Toggle-Buttons  TOFIX: beenden
                    //if (togModul1.IsChecked == false)
                    //{
                    //    ListHMIAll = context.db_message.Where(r => r.sStation)
                    //}
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, 
                                    "Fehler", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                }

            }

            if(GlobalVar.bUseTranslater)
            {
                ListHMIAllTransl = TranslateMsgList(ListHMIAll);

                UpdateDataGrid(ListHMIAllTransl);
                UpdateComboBox(ListHMIAllTransl);
            }
            else
            {
                RemoveSeparatorFromMsg(ListHMIAll);

                UpdateDataGrid(ListHMIAll);
                UpdateComboBox(ListHMIAll);
            }
        }

        public void RemoveSeparatorFromMsg(List<db_message> _listSource)
        {
            for(int i=0; i< _listSource.Count; i++)
            {
                string[] arrstrMsgSplit; 
                arrstrMsgSplit = _listSource[i].sMessage.Split('§');

                try
                {
                    _listSource[i].sMessage = arrstrMsgSplit[0];
                    _listSource[i].sMessage = _listSource[i].sMessage + " " + arrstrMsgSplit[1];
                    _listSource[i].sMessage = _listSource[i].sMessage + " " + arrstrMsgSplit[2];
                }
                catch
                {
                    ;
                }
            }
        }



        public List<db_message> TranslateMsgList(List<db_message> _listToTransl)
        {
            List<db_message> listTranslated = new List<db_message>();

            for (int i = 0; i < _listToTransl.Count; i++)
            {
                db_message tmpDBMessage = new db_message();

                tmpDBMessage.iType  = _listToTransl[i].iType;
                tmpDBMessage.sBMK   = _listToTransl[i].sBMK;
                tmpDBMessage.dtCome = _listToTransl[i].dtCome;
                tmpDBMessage.dtGone = _listToTransl[i].dtGone;
                tmpDBMessage.sStation = GlobalFunc.GetStationName_Translate(_listToTransl[i].iModul, _listToTransl[i].sStation);

                //string[] arrstrSeparator = { " ","-"," "};
                //string[] arrstrSeparator = { " - " };
                string[] arrstrMsgSplit; // = { "", "", "" };
                string strTmpMsg1 = "";
                string strTmpMsg2 = "";
                string strTmpMsg3 = "";

                //arrstrMsgSplit = _listToTransl[i].sMessage.Split(arrstrSeparator, StringSplitOptions.RemoveEmptyEntries);
                //arrstrMsgSplit = Regex.Split(_listToTransl[i].sMessage, @"\ \-\ ");
                arrstrMsgSplit = _listToTransl[i].sMessage.Split('§');

                try
                {
                    strTmpMsg1 = Tanslater.TransTxt(arrstrMsgSplit[0], eFBType.fb_Message);
                    strTmpMsg2 = Tanslater.TransTxt(arrstrMsgSplit[1], eFBType.fb_Message);
                    strTmpMsg3 = Tanslater.TransTxt(arrstrMsgSplit[2], eFBType.fb_Message);
                }
                catch
                {
                    ;
                }
                //tmpDBMessage.sMessage = strTmpMsg1 + " " + strTmpMsg2 + " " + strTmpMsg3;
                tmpDBMessage.sMessage = GlobalFunc.ConcatenateMessage(strTmpMsg1, strTmpMsg2, strTmpMsg3, GlobalFunc.eConcatenateMessageMode.ForHMI);

                listTranslated.Add(tmpDBMessage);
            }

            return listTranslated;
        }











        private void DGMeldungen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dGMeldungen.SelectedItem != null)
            {
                e.Handled = true;
                BtnFilter.IsEnabled = true;

                int indTmp_ParamSet = dGMeldungen.SelectedIndex; // Index der ParameterSet-Tabelle Zwischenspeichern.

                int index = dGMeldungen.SelectedIndex;
                var rowDataGrid = dGMeldungen.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;

                //var msgSelected = dGMeldungen.SelectedItem as db_message;

                msgSelected = new db_message();
                msgSelected = dGMeldungen.SelectedItem as db_message;

                tbSelectedMsgAll.Text = msgSelected.sMessage;


                tblSelectedMsg.Text     = msgSelected.sMessage; 
                tblSelectedMsgType.Text = Convert.ToString((eMsgType)msgSelected.iType); 
                tblSelectedStation.Text = msgSelected.sStation;
                tblSelectedBMK.Text     = msgSelected.sBMK;
                tblSelectedUser.Text    = msgSelected.sUserName;
            }
            else
            {
                ;
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            List<db_message> listTmpFilterSource;

            if (GlobalVar.bUseTranslater)
                listTmpFilterSource = ListHMIAllTransl;
            else
                listTmpFilterSource = ListHMIAll;

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                try
                {
                    ListHMIFiltered = listTmpFilterSource; // Zunäcsht ist die gefilterte Liste gleich die Ausgangsliste

                    if(cbMsg.Text != "")
                        ListHMIFiltered = listTmpFilterSource.Where(m => m.sMessage.Contains(cbMsg.Text)).ToList();

                    if (cbStation.Text != "")
                        ListHMIFiltered = ListHMIFiltered.Where(m => m.sStation == cbStation.Text).ToList();

                    if (cbMsgType.Text != "")
                        ListHMIFiltered = ListHMIFiltered.Where(m => m.iType == (int)Enum.Parse(typeof(eMsgType), cbMsgType.Text)).ToList();

                    if (cbBMK.Text != "")
                        ListHMIFiltered = ListHMIFiltered.Where(m => m.sBMK == cbBMK.Text).ToList();

                    if (cbUser.Text != "")
                        ListHMIFiltered = ListHMIFiltered.Where(m => m.sUserName == cbUser.Text).ToList();

                    BtnFilter.IsEnabled = false;
                    BtnReset.IsEnabled = true;
                }              
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Fehler beim Filtern", MessageBoxButton.OK, MessageBoxImage.Error);                    
                }
            }


            //if (GlobalVar.bUseTranslater)
            //{
            //    ListHMIFilteredTransl = TranslateMsgList(ListHMIFiltered);

            //    UpdateDataGrid(ListHMIFilteredTransl);
            //    UpdateComboBox(ListHMIFilteredTransl);
            //}
            //else
            //{
            //    UpdateDataGrid(ListHMIFiltered);
            //    UpdateComboBox(ListHMIFiltered);
            //}

            UpdateDataGrid(ListHMIFiltered);
            UpdateComboBox(ListHMIFiltered);

        }

        private void UpdateDataGrid(List<db_message> listToShow)
        {
            itemCollectionViewSourceMessages.Source = null;
            itemCollectionViewSourceMessages = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages"));
            itemCollectionViewSourceMessages.Source = listToShow; 
        }

        private void UpdateComboBox(List<db_message> msgListSource)
        {
            iListFilterType       = new List<int>();
            strListFilterStation  = new List<string>();
            strListFilterMessage  = new List<string>();
            strListFilterBMK      = new List<string>();
            strListFilterUser     = new List<string>();
            
            iListFilterType       = msgListSource.Select(m => m.iType).Distinct().ToList();
            strListFilterStation  = msgListSource.Select(m => m.sStation).Distinct().ToList();
            strListFilterMessage  = msgListSource.Select(m => m.sMessage).Distinct().ToList();
            strListFilterBMK      = msgListSource.Select(m => m.sBMK).Distinct().ToList();
            strListFilterUser     = msgListSource.Select(m => m.sUserName).Distinct().ToList();


            //cbMsgType.ItemsSource   = iListFilterType;
            cbMsgType.ItemsSource   = Enum.GetValues(typeof(eMsgType)).Cast<eMsgType>();
            cbStation.ItemsSource   = strListFilterStation;
            cbMsg.ItemsSource       = strListFilterMessage;
            cbBMK.ItemsSource       = strListFilterBMK;
            cbUser.ItemsSource      = strListFilterUser;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.bUseTranslater)
            {
                ListHMIAllTransl = TranslateMsgList(ListHMIAll);
                UpdateDataGrid(ListHMIAllTransl);
            }
            else
            {
                UpdateDataGrid(ListHMIAll);
            }

            BtnFilter.IsEnabled = true;
            BtnReset.IsEnabled = false;
        }

        private void BtnGetFromSelRow_Msg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbMsg.Text = msgSelected.sMessage;
            }
            catch
            {
                ;
            }
        }

        private void BtnGetFromSelRow_Type_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbMsgType.Text = Convert.ToString((eMsgType)msgSelected.iType);
            }
            catch
            {
                ;
            }
        }

        private void BtnGetFromSelRow_Stat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbStation.Text = msgSelected.sStation;
            }
            catch
            {
                ;
            }
        }

        private void BtnGetFromSelRow_BMK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbBMK.Text = msgSelected.sBMK;
            }
            catch
            {
                ;
            }
        }

        private void BtnGetFromSelRow_User_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbUser.Text = msgSelected.sUserName;
            }
            catch
            {
                ;
            }
        }


        private void BtnReset_Msg_Click(object sender, RoutedEventArgs e)
        {
            cbMsg.Text = "";
        }

        private void BtnReset_Type_Click(object sender, RoutedEventArgs e)
        {
            cbMsgType.Text = "";
        }

        private void BtnReset_Stat_Click(object sender, RoutedEventArgs e)
        {
            cbStation.Text = "";
        }

        private void BtnReset_BMK_Click(object sender, RoutedEventArgs e)
        {
            cbBMK.Text = "";
        }

        private void BtnReset_User_Click(object sender, RoutedEventArgs e)
        {
            cbUser.Text = "";
        }

        private void CbMsg_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), cbMsg.Text, typeof(TextBox));
            //virtualKeyboard.ShowDialog();
            //
            //if (virtualKeyboard.DialogResult == true)
            //    cbMsg.Text = virtualKeyboard.AnswerTextBox;
        }
    }
}
