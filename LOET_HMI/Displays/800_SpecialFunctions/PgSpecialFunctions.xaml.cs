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
using LOET_HMI;
using LOET_HMI.PLC_Com_Classes;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgSpecialFunctions.xaml
    /// </summary>
    public partial class PgSpecialFunctions : Page, INotifyPropertyChanged
    {

        //********************************************************************************************
        // *****************************   CHP-Buttons   *********************************************
        public static readonly DependencyProperty _BtnListMain = DependencyProperty.Register(
            "BtnList", typeof(List<StButton>), typeof(MainWindow), new PropertyMetadata(new List<StButton>()));

        public List<StButton> BtnList
        {
            get { return (List<StButton>)GetValue(_BtnListMain); }
            set { SetValue(_BtnListMain, value); }
        }

        public int iBtnCount { get; set; }
        public int iBtnArrayStartInd { get; set; }
        //********************************************************************************************
        //********************************************************************************************


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        public PgSpecialFunctions()
        {

            // *******************************************************************************************
            // *****************************   CHP-Buttons   *********************************************
            iBtnCount = 1; // 8;
            iBtnArrayStartInd = 1; // GVLarrNr_Button.gcBtn_1;

            for (int i = 1; i <= iBtnCount; i++)
                BtnList.Add(new StButton());

            // *******************************************************************************************
            // *******************************************************************************************



            InitializeComponent();
        }

        private void PgStat_Loaded(object sender, RoutedEventArgs e)
        {
            // CHP-Buttons
            for (int i = 0; i < BtnList.Count; i++)
                BtnList[i].Register("GVL_Button.g_arrButtonsRef[" + Convert.ToString(iBtnArrayStartInd + i) + "]");
        }

        private void PgStat_Unloaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BtnList.Count; i++)
                BtnList[i].Deregister();
        }
    }
}
