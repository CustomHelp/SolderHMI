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
using System.Windows.Shapes;
using LOET_HMI.Displays;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für Window_DeleteOrder.xaml
    /// </summary>
    public partial class Window_DeleteOrder : Window
    {

        public OrderHMI SelectedOrder { get; set; }

        public Window_DeleteOrder(OrderHMI selectedOrder)
        {
            InitializeComponent();
            SelectedOrder = selectedOrder;

            tblName.Text = selectedOrder.sParamSetname;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            /*
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                /////////// Den ausgewählter Auftrag entfernen
                db_order orderToRemove;

                orderToRemove = context.db_order.Single(x => x.id == SelectedOrder.id);

                context.db_order.Remove(orderToRemove);
                context.SaveChanges();

                /////////// Neuer Warteschlangennummer zuweisen
                List<db_order> listOrders;
                listOrders = context.db_order.OrderBy( o => o.iNrInQueue).ToList(); // Aufträge von der Datenbank sortiert nach "iNrInQueue" einlesen

                for (int i=0; i<listOrders.Count; i++)
                {
                    listOrders[i].iNrInQueue = i + 1;
                }
                context.SaveChanges();
                

                DialogResult = true;
            }
            */

            DBOrder.Handler.RemoveOrderFromQueue(SelectedOrder.id);
            DialogResult = true;

            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
