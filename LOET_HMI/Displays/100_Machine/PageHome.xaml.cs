using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HOPA_HMI.PLC_Com_Classes;
using HOPA_HMI.Klassen;
using HOPA_HMI.UserControls;

namespace HOPA_HMI
{

    public partial class PageHome : Page, INotifyPropertyChanged
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private List<ADSItem> ItemList = new List<ADSItem>();

        // Elemente
        List<string> ListMess = new List<string>();
        List<HMIMessage> ListHMITransMess = new List<HMIMessage>();
        List<HMIMessage> ListHMIMess = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceMessages = new CollectionViewSource();
        List<HMIMessage> ListHMIErr = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceErrors = new CollectionViewSource();

        List<HMIMessage> ListHMIMess2 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceMessages2 = new CollectionViewSource();
        List<HMIMessage> ListHMIErr2 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceErrors2 = new CollectionViewSource();

        List<HMIMessage> ListHMIMess3 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceMessages3 = new CollectionViewSource();
        List<HMIMessage> ListHMIErr3 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceErrors3 = new CollectionViewSource();

        List<HMIMessage> ListHMIMess4 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceMessages4 = new CollectionViewSource();
        List<HMIMessage> ListHMIErr4 = new List<HMIMessage>();
        public CollectionViewSource itemCollectionViewSourceErrors4 = new CollectionViewSource();
        // Variablen
        int i; Int16 iMessageCount;
        int iActOP, iActOP2, iActOP3, iActOP4;

        private string _sOPImage = "/CHP_HMI;component/Resources/Not-Aus.png";
        public string sOPImage
        {
            get
            {
                return _sOPImage;
            }

            set
            {
                _sOPImage = value;
                OnPropertyChanged();
            }
        }


        private string _sOPImage2 = "/CHP_HMI;component/Resources/Not-Aus.png";
        public string sOPImage2
        {
            get
            {
                return _sOPImage2;
            }

            set
            {
                _sOPImage2 = value;
                OnPropertyChanged();
            }
        }


        private string _sOPImage3 = "/CHP_HMI;component/Resources/Not-Aus.png";
        public string sOPImage3
        {
            get
            {
                return _sOPImage3;
            }

            set
            {
                _sOPImage3 = value;
                OnPropertyChanged();
            }
        }


        private string _sOPImage4 = "/CHP_HMI;component/Resources/Not-Aus.png";
        public string sOPImage4
        {
            get
            {
                return _sOPImage4;
            }

            set
            {
                _sOPImage4 = value;
                OnPropertyChanged();
            }
        }


        private int iModul, iStation;
        List<ST_Message> PLCMesList = new List<ST_Message>();
        List<ST_Message> PLCErrList = new List<ST_Message>();

        // Tanslation
        ICHPTranslate Tanslater = new CHPTransService();
        MessageArchivist messageArchivist1 = new MessageArchivist();
        MessageArchivist messageArchivist2 = new MessageArchivist();
        MessageArchivist messageArchivist3 = new MessageArchivist();
        MessageArchivist messageArchivist4 = new MessageArchivist();

        public PageHome(int _iModul, int _iStation)
        {
            iModul = _iModul;
            iStation = _iStation;
            InitializeComponent();

            if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectMsg) // MB 7.8.2020
            {
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[1]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[2]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[3]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[4]", typeof(ST_MessagesModul)));

                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[1].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[2].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[3].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[4].iOP", typeof(Int16)));

                VarCon.EnableCallbackEvent();
                VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
            }
        }
        public PageHome() // Balog 29.7.2020: Konstruktor ohne Eingabeparameter
        {
            InitializeComponent();

            if (GlobalVar.debugADS.bWantPLCConnect && GlobalVar.debugADS.bWantConnectMsg) // MB 7.8.2020
            {
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[1]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[2]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[3]", typeof(ST_MessagesModul)));
                ItemList.Add(VarCon.AddItem("GVL_Module.garrMesWindowHMI[4]", typeof(ST_MessagesModul)));

                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[1].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[2].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[3].iOP", typeof(Int16)));
                ItemList.Add(VarCon.AddItem("GVL_Module.gBedienstellen[4].iOP", typeof(Int16)));

                VarCon.EnableCallbackEvent();
                VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
            }
        }



        private void BtnStart2_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM2Start", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void BtnStop2_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM2Stop", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        private void BtnQuit2_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM2Quit", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnOP2_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Module.gBedienstellen[2].bSelectStepMode", !(iActOP2 == 4));
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnOP1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Module.gBedienstellen[1].bSelectStepMode", !(iActOP == 4));
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnStart1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM1Start", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnStop1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM1Stop", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnQuit1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM1Quit", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnOP3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Module.gBedienstellen[3].bSelectStepMode", !(iActOP3 == 4));
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnStart3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM3Start", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);
 
        }

        private void BtnStop3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM3Stop", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnQuit3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM3Quit", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnOP4_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Module.gBedienstellen[4].bSelectStepMode", !(iActOP4 == 4));
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnStart4_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM4Start", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnStop4_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM4Stop", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void BtnQuit4_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GlobalVar.ActUser.iUserLevel >= UserRights.Operate)
            {
                VarCon.WriteItem("GVL_Panel.bPANM4Quit", true);
            }
            else
                MessageBox.Show(UserRights.sAccessDenied, UserRights.sMesBoxCaption, MessageBoxButton.OK, MessageBoxImage.Stop);

        }

        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            for (int j = 0; j < e.Item.Count; j++)
            {
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (ItemList[i].iHandle == e.Item[j].iHandle)
                    {
                        ItemList[i].Value = e.Item[j].Value;
                    }
                }
            }

            if (ItemList[0].Value != null)
            {
                ST_MessagesModul tmp = (ST_MessagesModul)ItemList[0].Value;

                iMessageCount = tmp.iMessageCount;

                ListMess.Clear();

                if (iMessageCount > 0)
                    ListMess.Add(tmp.sMessage1);
                if (iMessageCount > 1)
                    ListMess.Add(tmp.sMessage2);
                if (iMessageCount > 2)
                    ListMess.Add(tmp.sMessage3);
                if (iMessageCount > 3)
                    ListMess.Add(tmp.sMessage4);
                if (iMessageCount > 4)
                    ListMess.Add(tmp.sMessage5);
                if (iMessageCount > 5)
                    ListMess.Add(tmp.sMessage6);
                if (iMessageCount > 6)
                    ListMess.Add(tmp.sMessage7);
                if (iMessageCount > 7)
                    ListMess.Add(tmp.sMessage8);
                if (iMessageCount > 8)
                    ListMess.Add(tmp.sMessage9);
                if (iMessageCount > 9)
                    ListMess.Add(tmp.sMessage10);
            }
            messageArchivist1.SetMessages(ListMess);

            // Übersetzen
            ListHMITransMess = Tanslater.TransMessageList(ListMess, GlobalVar.Language);

            ListHMIMess.Clear();
            ListHMIErr.Clear();
            for (i = 0; i < ListHMITransMess.Count; i++)
            {
                if (ListHMITransMess[i].iType <= MessTypes.ErrorMess)
                    ListHMIErr.Add(ListHMITransMess[i]);
                else
                    ListHMIMess.Add(ListHMITransMess[i]);
            }

            itemCollectionViewSourceMessages.Source = null;
            itemCollectionViewSourceMessages = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages"));
            itemCollectionViewSourceMessages.Source = ListHMIMess;

            itemCollectionViewSourceErrors.Source = null;
            itemCollectionViewSourceErrors = (CollectionViewSource)(FindResource("itemCollectionViewSourceErrors"));
            itemCollectionViewSourceErrors.Source = ListHMIErr;




            if (ItemList[1].Value != null)
            {
                ST_MessagesModul tmp = (ST_MessagesModul)ItemList[1].Value;

                iMessageCount = tmp.iMessageCount;

                ListMess.Clear();

                if (iMessageCount > 0)
                    ListMess.Add(tmp.sMessage1);
                if (iMessageCount > 1)
                    ListMess.Add(tmp.sMessage2);
                if (iMessageCount > 2)
                    ListMess.Add(tmp.sMessage3);
                if (iMessageCount > 3)
                    ListMess.Add(tmp.sMessage4);
                if (iMessageCount > 4)
                    ListMess.Add(tmp.sMessage5);
                if (iMessageCount > 5)
                    ListMess.Add(tmp.sMessage6);
                if (iMessageCount > 6)
                    ListMess.Add(tmp.sMessage7);
                if (iMessageCount > 7)
                    ListMess.Add(tmp.sMessage8);
                if (iMessageCount > 8)
                    ListMess.Add(tmp.sMessage9);
                if (iMessageCount > 9)
                    ListMess.Add(tmp.sMessage10);

            }
            messageArchivist2.SetMessages(ListMess);

            // Übersetzen
            ListHMITransMess = Tanslater.TransMessageList(ListMess, GlobalVar.Language);

            ListHMIMess2.Clear();
            ListHMIErr2.Clear();
            for (i = 0; i < ListHMITransMess.Count; i++)
            {
                if (ListHMITransMess[i].iType <= MessTypes.ErrorMess)
                    ListHMIErr2.Add(ListHMITransMess[i]);
                else
                    ListHMIMess2.Add(ListHMITransMess[i]);
            }

            itemCollectionViewSourceMessages2.Source = null;
            itemCollectionViewSourceMessages2 = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages2"));
            itemCollectionViewSourceMessages2.Source = ListHMIMess2;

            itemCollectionViewSourceErrors2.Source = null;
            itemCollectionViewSourceErrors2 = (CollectionViewSource)(FindResource("itemCollectionViewSourceErrors2"));
            itemCollectionViewSourceErrors2.Source = ListHMIErr2;


            if (ItemList[2].Value != null)
            {
                ST_MessagesModul tmp = (ST_MessagesModul)ItemList[2].Value;

                iMessageCount = tmp.iMessageCount;

                ListMess.Clear();

                if (iMessageCount > 0)
                    ListMess.Add(tmp.sMessage1);
                if (iMessageCount > 1)
                    ListMess.Add(tmp.sMessage2);
                if (iMessageCount > 2)
                    ListMess.Add(tmp.sMessage3);
                if (iMessageCount > 3)
                    ListMess.Add(tmp.sMessage4);
                if (iMessageCount > 4)
                    ListMess.Add(tmp.sMessage5);
                if (iMessageCount > 5)
                    ListMess.Add(tmp.sMessage6);
                if (iMessageCount > 6)
                    ListMess.Add(tmp.sMessage7);
                if (iMessageCount > 7)
                    ListMess.Add(tmp.sMessage8);
                if (iMessageCount > 8)
                    ListMess.Add(tmp.sMessage9);
                if (iMessageCount > 9)
                    ListMess.Add(tmp.sMessage10);

            }
            messageArchivist3.SetMessages(ListMess);

            // Übersetzen
            ListHMITransMess = Tanslater.TransMessageList(ListMess, GlobalVar.Language);

            ListHMIMess3.Clear();
            ListHMIErr3.Clear();
            for (i = 0; i < ListHMITransMess.Count; i++)
            {
                if (ListHMITransMess[i].iType <= MessTypes.ErrorMess)
                    ListHMIErr3.Add(ListHMITransMess[i]);
                else
                    ListHMIMess3.Add(ListHMITransMess[i]);
            }

            itemCollectionViewSourceMessages3.Source = null;
            itemCollectionViewSourceMessages3 = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages3"));
            itemCollectionViewSourceMessages3.Source = ListHMIMess3;

            itemCollectionViewSourceErrors3.Source = null;
            itemCollectionViewSourceErrors3 = (CollectionViewSource)(FindResource("itemCollectionViewSourceErrors3"));
            itemCollectionViewSourceErrors3.Source = ListHMIErr3;




            if (ItemList[3].Value != null)
            {
                ST_MessagesModul tmp = (ST_MessagesModul)ItemList[3].Value;

                iMessageCount = tmp.iMessageCount;

                ListMess.Clear();

                if (iMessageCount > 0)
                    ListMess.Add(tmp.sMessage1);
                if (iMessageCount > 1)
                    ListMess.Add(tmp.sMessage2);
                if (iMessageCount > 2)
                    ListMess.Add(tmp.sMessage3);
                if (iMessageCount > 3)
                    ListMess.Add(tmp.sMessage4);
                if (iMessageCount > 4)
                    ListMess.Add(tmp.sMessage5);
                if (iMessageCount > 5)
                    ListMess.Add(tmp.sMessage6);
                if (iMessageCount > 6)
                    ListMess.Add(tmp.sMessage7);
                if (iMessageCount > 7)
                    ListMess.Add(tmp.sMessage8);
                if (iMessageCount > 8)
                    ListMess.Add(tmp.sMessage9);
                if (iMessageCount > 9)
                    ListMess.Add(tmp.sMessage10);

            }
            messageArchivist4.SetMessages(ListMess);

            // Übersetzen
            ListHMITransMess = Tanslater.TransMessageList(ListMess, GlobalVar.Language);

            ListHMIMess4.Clear();
            ListHMIErr4.Clear();
            for (i = 0; i < ListHMITransMess.Count; i++)
            {
                if (ListHMITransMess[i].iType <= MessTypes.ErrorMess)
                    ListHMIErr4.Add(ListHMITransMess[i]);
                else
                    ListHMIMess4.Add(ListHMITransMess[i]);
            }

            itemCollectionViewSourceMessages4.Source = null;
            itemCollectionViewSourceMessages4 = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages4"));
            itemCollectionViewSourceMessages4.Source = ListHMIMess4;

            itemCollectionViewSourceErrors4.Source = null;
            itemCollectionViewSourceErrors4 = (CollectionViewSource)(FindResource("itemCollectionViewSourceErrors4"));
            itemCollectionViewSourceErrors4.Source = ListHMIErr4;



            try
            {
                iActOP = Convert.ToInt32(ItemList[4].Value);
                var brush = new ImageBrush();
                switch (iActOP)
                {
                    case 0:
                        sOPImage = "/CHP_HMI;component/Resources/Not-Aus.png";
                        break;

                    case 1:
                        sOPImage = "/CHP_HMI;component/Resources/System OFF.png";
                        break;

                    case 2:
                        sOPImage = "/CHP_HMI;component/Resources/Start UP.png";
                        break;

                    case 3:
                        sOPImage = "/CHP_HMI;component/Resources/Manual Mode - Select.png";
                        break;

                    case 4:
                        sOPImage = "/CHP_HMI;component/Resources/Step Mode.png";
                        break;

                    case 5:
                        sOPImage = "/CHP_HMI;component/Resources/Automatic OFF.png";
                        break;

                    case 6:
                        sOPImage = "/CHP_HMI;component/Resources/Automatic ON – Last Cycle.png";
                        break;

                    case 7:
                        sOPImage = "/CHP_HMI;component/Resources/Automatic ON.png";
                        break;
                }
                btnOP1.Background = brush;
                /////////////////////////
                iActOP2 = Convert.ToInt32(ItemList[5].Value);

                switch (iActOP2)
                {
                    case 0:
                        sOPImage2 = "/CHP_HMI;component/Resources/Not-Aus.png";
                        break;

                    case 1:
                        sOPImage2 = "/CHP_HMI;component/Resources/System OFF.png";
                        break;

                    case 2:
                        sOPImage2 = "/CHP_HMI;component/Resources/Start UP.png";
                        break;

                    case 3:
                        sOPImage2 = "/CHP_HMI;component/Resources/Manual Mode - Select.png";
                        break;

                    case 4:
                        sOPImage2 = "/CHP_HMI;component/Resources/Step Mode.png";
                        break;

                    case 5:
                        sOPImage2 = "/CHP_HMI;component/Resources/Automatic OFF.png";
                        break;

                    case 6:
                        sOPImage2 = "/CHP_HMI;component/Resources/Automatic ON – Last Cycle.png";
                        break;

                    case 7:
                        sOPImage2 = "/CHP_HMI;component/Resources/Automatic ON.png";
                        break;
                }
                /////////////////////////
                iActOP3 = Convert.ToInt32(ItemList[6].Value);

                switch (iActOP3)
                {
                    case 0:
                        sOPImage3 = "/CHP_HMI;component/Resources/Not-Aus.png";
                        break;

                    case 1:
                        sOPImage3 = "/CHP_HMI;component/Resources/System OFF.png";
                        break;

                    case 2:
                        sOPImage3 = "/CHP_HMI;component/Resources/Start UP.png";
                        break;

                    case 3:
                        sOPImage3 = "/CHP_HMI;component/Resources/Manual Mode - Select.png";
                        break;

                    case 4:
                        sOPImage3 = "/CHP_HMI;component/Resources/Step Mode.png";
                        break;

                    case 5:
                        sOPImage3 = "/CHP_HMI;component/Resources/Automatic OFF.png";
                        break;

                    case 6:
                        sOPImage3 = "/CHP_HMI;component/Resources/Automatic ON – Last Cycle.png";
                        break;

                    case 7:
                        sOPImage3 = "/CHP_HMI;component/Resources/Automatic ON.png";
                        break;
                }
                /////////////////////////
                iActOP4 = Convert.ToInt32(ItemList[7].Value);

                switch (iActOP4)
                {
                    case 0:
                        sOPImage4 = "/CHP_HMI;component/Resources/Not-Aus.png";
                        break;

                    case 1:
                        sOPImage4 = "/CHP_HMI;component/Resources/System OFF.png";
                        break;

                    case 2:
                        sOPImage4 = "/CHP_HMI;component/Resources/Start UP.png";
                        break;

                    case 3:
                        sOPImage4 = "/CHP_HMI;component/Resources/Manual Mode - Select.png";
                        break;

                    case 4:
                        sOPImage4 = "/CHP_HMI;component/Resources/Step Mode.png";
                        break;

                    case 5:
                        sOPImage4 = "/CHP_HMI;component/Resources/Automatic OFF.png";
                        break;

                    case 6:
                        sOPImage4 = "/CHP_HMI;component/Resources/Automatic ON – Last Cycle.png";
                        break;

                    case 7:
                        sOPImage4 = "/CHP_HMI;component/Resources/Automatic ON.png";
                        break;
                }
            }
            catch
            { }






            }
        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}