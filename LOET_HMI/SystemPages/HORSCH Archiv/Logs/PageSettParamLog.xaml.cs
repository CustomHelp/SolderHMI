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

namespace LOET_HMI
{

    public partial class PageSettParamLog : Page
    {
        // Elemente
        List<db_parameterlog> List = new List<db_parameterlog>();
        public CollectionViewSource itemCollectionViewSourceMessages = new CollectionViewSource();

        public PageSettParamLog()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today;
            tbStartTime.Text = new TimeSpan(0, 0, 0).ToString();
            tbEndTime.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss");
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            DateTime StartDate, EndDate;

            try
            {
                StartDate = DateTime.Parse(dpStartDate.Text);
            }
            catch
            {
                StartDate = new DateTime(1, 1, 1);
            }


            try
            {
                StartDate = StartDate + TimeSpan.Parse(tbStartTime.Text);
            }
            catch
            {
                StartDate = StartDate + new TimeSpan(0, 0, 0);
            }

            try
            {
                EndDate = DateTime.Parse(dpEndDate.Text);
            }
            catch
            {
                EndDate = new DateTime(9999, 12, 31);
            }

            try
            {
                EndDate = EndDate + TimeSpan.Parse(tbEndTime.Text);
            }
            catch
            {
                EndDate = EndDate + new TimeSpan(23, 59, 59);
            }

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                List = context.db_parameterlog.Where(r => ((r.dtTime >= StartDate) && (r.dtTime <= EndDate))).ToList();
            }

            itemCollectionViewSourceMessages.Source = null;
            itemCollectionViewSourceMessages = (CollectionViewSource)(FindResource("itemCollectionViewSourceMessages"));
            itemCollectionViewSourceMessages.Source = List;

        }
    }
}
