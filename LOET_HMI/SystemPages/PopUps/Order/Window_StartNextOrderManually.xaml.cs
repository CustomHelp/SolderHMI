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

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für Window_StartNextOrderManually.xaml
    /// </summary>
    public partial class Window_StartNextOrderManually : Window
    {



        public Window_StartNextOrderManually()
        {
            InitializeComponent();
        }



        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = DBOrder.Handler.SendNextOrderToPLC();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                db_orderqueue orderNext    = new db_orderqueue();
                db_paramset paramSetNext = new db_paramset();
                try // Warteschlange der Aufträge ist nicht leer
                {
                    orderNext = context.db_orderqueue.Single(x => x.iNrInQueue == 1);
                    paramSetNext = context.db_paramset.Single(x => x.id == orderNext.iParamSetId);
                    db_parameter parCarton = context.db_parameter.Single(x => x.iParamSetId == orderNext.iParamSetId && x.sADSName == GlobalVar.Orders.sCartonProPallet_ADSName);

                    tblName.Text = paramSetNext.sName;
                    tblQuant.Text = orderNext.iQuantity.ToString();
                    tblCartProPalett.Text = orderNext.iCartonQuantProPallet.ToString();
                    tblCreatedOn.Text = orderNext.dtAddedOn.ToString("dd.MM.yyyy HH:mm"); ;
                    tblCreatedBy.Text = orderNext.sAddedBy;
                }
                catch // Warteschlange der Auftrage ist leer
                {
                    ;
                }


            }

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
