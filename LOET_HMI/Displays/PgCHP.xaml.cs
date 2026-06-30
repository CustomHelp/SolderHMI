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

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgCHP.xaml
    /// </summary>
    public partial class PgStatCHP : Page
    {

        public static readonly DependencyProperty _iCountCyl = DependencyProperty.Register(
            "iCountCyl", typeof(int), typeof(PgStatCHP), new PropertyMetadata(new int()));

        public int iCountCyl
        {
            get { return (int)GetValue(_iCountCyl); }
            set { SetValue(_iCountCyl, value); }
        }

        public static readonly DependencyProperty _iStartIndCylArr = DependencyProperty.Register(
                "iStartIndCylArr", typeof(int), typeof(PgStatCHP), new PropertyMetadata(new int()));

        public int iStartIndCylArr
        {
            get { return (int)GetValue(_iStartIndCylArr); }
            set { SetValue(_iStartIndCylArr, value); }
        }




        //********************************************************************************************
        // *****************************   Zylinder   ************************************************
        public static readonly DependencyProperty _CylList = DependencyProperty.Register(
            "CylList", typeof(List<StCylinder>), typeof(PgStatCHP), new PropertyMetadata(new List<StCylinder>()));

        public List<StCylinder> CylList
        {
            get { return (List<StCylinder>)GetValue(_CylList); }
            set { SetValue(_CylList, value); }
        }


        //********************************************************************************************
        //********************************************************************************************

        // Create the OnPropertyChanged method to raise the event
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public PgStatCHP()
        {

            // Zylinder
            for (int i = 1; i <= iCountCyl; i++)
                CylList.Add(new StCylinder());




            InitializeComponent();
        }

        private void Pg_chp_Loaded(object sender, RoutedEventArgs e)
        {
            int iTmp = iStartIndCylArr;


            for (int i = 0; i < CylList.Count; i++)
            {
                CylList[0].Register("GVL_Cylinder.g_arrCylRef[" + iTmp.ToString() + "]");
                iTmp++;
            }

        }

        private void Pg_chp_Unloaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CylList.Count; i++)
                CylList[i].Deregister();

        }
    }
}
