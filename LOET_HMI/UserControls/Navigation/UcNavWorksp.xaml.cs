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
using LOET_HMI.PLC_Com_Classes;
using LOET_HMI.SystemPages.PopUps;


namespace LOET_HMI.UserControls
{
    /// <summary>
    /// Interaktionslogik für UcNavWorksp.xaml
    /// </summary>
    public partial class UcNavWorksp : UserControl, INotifyPropertyChanged
    {
        private string _Station_Name;
        public string Station_Name
        {
            get  { return _Station_Name; }
            set
            {
                _Station_Name = value;
                OnPropertyChanged();
            }
        }

        private Frame _Frame_of_Nav;
        public Frame Frame_of_Nav
        {
            get { return _Frame_of_Nav; }
            set
            {
                _Frame_of_Nav = value;
                OnPropertyChanged();
            }
        }

        public NavCluster ActCluster_InRow1 { get; set; }


        private Button _Act_Button_Row1;
        public Button ActButton_Row1
        {
            get { return _Act_Button_Row1; }
            set
            {
                if(_Act_Button_Row1 != null)
                    _Act_Button_Row1.FontWeight = FontWeights.Regular;

                _Act_Button_Row1=value;
                _Act_Button_Row1.FontWeight = FontWeights.Bold; 
            }
        }

        private Button _Act_Button_Row2;
        public Button ActButton_Row2
        {
            get { return _Act_Button_Row2; }
            set
            {
                if (_Act_Button_Row2 != null)
                    _Act_Button_Row2.FontWeight = FontWeights.Regular;

                _Act_Button_Row2 = value;
                _Act_Button_Row2.FontWeight = FontWeights.Bold;
            }
        }


        public List<NavCluster> ClusterListRow1; // Die Clusters (Cluster=Button+Display), die bei der aktuellen Station verwendet werden, werden in diese List geladen.
        public List<NavCluster> ClusterListRow2;

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public UcNavWorksp()
        {
            ClusterListRow1 = new List<NavCluster>();
            ClusterListRow2 = new List<NavCluster>();

            Frame_of_Nav = new Frame();


            Station_Name = "...";

            InitializeComponent();
            this.DataContext = this;

            ProbeButton1.Visibility = Visibility.Collapsed;
            ProbeButton2.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Diese Methode lädt die Displays in den horizontalen Navigation des Tasters,
        /// der im vertikalen angeklickt wurde
        /// </summary>
        /// <param name="station"></param>
        public void LinkWithStDisplays(StationVertNav station)
        {
            ClusterListRow1.Clear();
            stackPanelRow1.Children.Clear();
            ClusterListRow2.Clear();
            stackPanelRow2.Children.Clear();

            LoadNavigationRow(station.ClusterList1stRow, ClusterListRow1, stackPanelRow1, BtnRow1_Click);

            Frame_of_Nav.Navigate(ClusterListRow1[0].Display); // OverviewDisplay als 1. Display eines Moduls anzeigen
            ActButton_Row1 = ClusterListRow1[0].ButtonOfDispl; // Aktuelle Button dem Display entsprechend auswählen.  Dann wird das FontWeight des Buttons automatisch auf Bold gesetzt.
       
            // Balog 25.06:
            BtnRow1_Click(ActButton_Row1, null);     
        }

        public void InitNextRow() 
        {
            LoadNavigationRow(ActCluster_InRow1.ClusterNextRow, ClusterListRow2, stackPanelRow2, BtnRow2_Click);

            Frame_of_Nav.Navigate(ClusterListRow2[0].Display); // OverviewDisplay als 1. Display eines Moduls anzeigen
            ActButton_Row2 = ClusterListRow2[0].ButtonOfDispl; // Aktuelle Button dem Display entsprechend auswählen.  Dann wird das FontWeight des Buttons automatisch auf Bold gesetzt.
        }

        public void LoadNavigationRow(List<NavCluster> BaseList , List<NavCluster> TargetList, StackPanel stackPanelOfRow, RoutedEventHandler btnClickHandler )
        {
            for (int i = 0; i < BaseList.Count; i++)
            {
                TargetList.Add(BaseList[i]); // Diese Clusters in NavClusterList ablegen.
                TargetList[i].ButtonOfDispl.Style = Application.Current.FindResource("RENA_NavWorkspaceButtonStyle") as Style;
                // Wurzelursache "Access Denied mehrfach": die Button-Objekte sind pro Station persistent und
                // LoadNavigationRow wird bei jedem Stationswechsel erneut aufgerufen. Ohne vorheriges -=
                // sammeln sich mehrere Click-Abos an, sodass ein einziger Klick den Handler mehrfach ausloest.
                TargetList[i].ButtonOfDispl.Click -= btnClickHandler;
                TargetList[i].ButtonOfDispl.Click += btnClickHandler;
                stackPanelOfRow.Children.Add(TargetList[i].ButtonOfDispl);
            }
        }

        // Verhindert, dass die "Kein Zugriff"-Meldung mehrfach erscheint, falls doch mehrere Events feuern.
        private bool _accessDeniedShown = false;

        /// <summary>Zeigt die "Kein Zugriff"-Meldung genau EINMAL an. Das Flag wird nach dem Wegklicken zurueckgesetzt.</summary>
        private void ShowAccessDeniedOnce()
        {
            if (_accessDeniedShown)
                return;

            _accessDeniedShown = true;
            GlobalFunc.ShowNoAuthorization(); // modal: blockiert bis der Bediener mit OK bestaetigt
            _accessDeniedShown = false;
        }

        /// <summary>Nach einer verweigerten Navigation zurueck auf die zuletzt erlaubte Seite (kein Verbleib auf der gesperrten Seite).</summary>
        private void NavigateBackToAllowedDisplay()
        {
            if (ActCluster_InRow1 != null && Frame_of_Nav.Content != ActCluster_InRow1.Display)
            {
                Frame_of_Nav.Navigate(ActCluster_InRow1.Display);
                ActButton_Row1 = ActCluster_InRow1.ButtonOfDispl;
            }
        }

        public void BtnRow1_Click(object sender, RoutedEventArgs e)
        {
            ClusterListRow2.Clear();
            stackPanelRow2.Children.Clear();

            Button btn = (Button)sender;

            for (int i = 0; i < ClusterListRow1.Count; i++)
            {
                if (ClusterListRow1[i].ButtonOfDispl.Content == btn.Content)
                {
                    if(btn.Content == Properties.Resources.NavHoriz_btnAllMachinePar || btn.Content == Properties.Resources.NavHoriz_btnUserManagement)
                    {
                        if(GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Supervisor.iLevel)
                        {
                            ShowAccessDeniedOnce();
                            NavigateBackToAllowedDisplay();
                            return;
                        }
                    }
                    if(btn.Content.ToString().Contains(Properties.Resources.NavHoriz_btnRFID_WT) == true)
                    {
                        if(GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Quality.iLevel)
                        {
                            ShowAccessDeniedOnce();
                            NavigateBackToAllowedDisplay();
                            return;
                        }
                    }
                    if(btn.Content.ToString().Contains(Properties.Resources.NavVert_btnManualMenu) == true || btn.Content == Properties.Resources.NavVert_btnGlobal)
                    {
                        if (GlobalVar.ActUser.iUserLevel < GlobalVar.Userlevels.Maintenance.iLevel)
                        {
                            ShowAccessDeniedOnce();
                            NavigateBackToAllowedDisplay();
                            return;
                        }
                    }
                    ActCluster_InRow1 = ClusterListRow1[i];
                    ActButton_Row1 = ClusterListRow1[i].ButtonOfDispl;   // Das Property "Act_Button" setzen. Dies ändert anschließend das FontWeight Property auf das aktuelle geklickte Button auf Bold.
                    Frame_of_Nav.Navigate(ClusterListRow1[i].Display); // Display in Frame laden

                    if (ClusterListRow1[i].ClusterNextRow.Count > 0)
                        InitNextRow();
                }
            }

            try
            {
                ActButton_Row2 = ClusterListRow2[0].ButtonOfDispl;
                BtnRow2_Click(ActButton_Row2, null);
            }
            catch
            {
                ;
            }


        }

        public void BtnRow2_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            for (int i = 0; i < ClusterListRow2.Count; i++)
            {
                if (ClusterListRow2[i].ButtonOfDispl.Content == btn.Content)
                {
                    //ActCluster_InRow1 = ClusterListRow1[i];
                    ActButton_Row2 = ClusterListRow2[i].ButtonOfDispl;   // Das Property "Act_Button" setzen. Dies ändert anschließend das FontWeight Property auf das aktuelle geklickte Button auf Bold.
                    Frame_of_Nav.Navigate(ClusterListRow2[i].Display); // Display in Frame laden
                }
            }
        }


    }
}
