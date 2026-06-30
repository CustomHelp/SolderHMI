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


namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgCounter.xaml
    /// </summary>
    public partial class PgCounter : Page
    {
        //********************************************************************************************
        // ***********************************   Counter   *******************************************
        public static readonly DependencyProperty _CounterList = DependencyProperty.Register(
            "CounterList", typeof(List<StCounter>), typeof(PgCounter), new PropertyMetadata(new List<StCounter>()));

        public List<StCounter> CounterList
        {
            get { return (List<StCounter>)GetValue(_CounterList); }
            set { SetValue(_CounterList, value); }
        }

        public int iCounter_Count { get; set; }
        public int iCounter_ArrayStartInd { get; set; }
        //********************************************************************************************
        //********************************************************************************************


        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public PgCounter()
        {
            //********************************************************************************************
            // ***********************************   Counter   *******************************************
            iCounter_Count = 10; // GlobalVar.GVL_Limits.gc_maxCounter;
            iCounter_ArrayStartInd  = 1;

            for (int i = 0; i < iCounter_Count; i++)
                CounterList.Add(new StCounter());

            //********************************************************************************************
            //********************************************************************************************


            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Counter
            for (int i = 0; i < CounterList.Count; i++)
                CounterList[i].Register("GVL_Counter.g_arrCounterRef[" + Convert.ToString(iCounter_ArrayStartInd + i) + "]");
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Counter
            for (int i = 0; i < CounterList.Count; i++)
                CounterList[i].Deregister();
        }
    }
}
