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


namespace LOET_HMI
{

    public partial class Window_RenameParamSet : Window
    {

        private ParamSetTypes _ParamSet;
        private string _sName;

        public Window_RenameParamSet(string sName, ParamSetTypes paramSet)
        {
            InitializeComponent();

            _ParamSet = paramSet;
            _sName = sName;
            tbActName.Text = sName;
            tbName.Text = sName;


        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CheckEntries();
        }

        private void CheckEntries()
        {
            // Name schon vergeben?
            string sName = tbName.Text;

            if (sName != "")
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    // Schauen ob der neue Name noch frei ist
                    db_paramset ParamSet;
                    try
                    {
                        ParamSet = context.db_paramset.First(r => ((r.sName == sName) && ((r.iType == (int)_ParamSet))));
                    }
                    catch
                    {
                        ParamSet = null;
                    }

                    if (ParamSet == null)
                    {
                        // Set zum umbenennen holen
                        ParamSet = context.db_paramset.Single(r => ((r.sName == _sName) && ((r.iType == (int)_ParamSet))));
                        ParamSet.sName = sName;
                        context.SaveChanges();

                        DialogResult = true;

                        this.Close();
                    }
                    else
                    {
                        tblName.Foreground = Brushes.Red;
                    }
                }
            }
            else
            {
                tblName.Foreground = Brushes.Red;
            }


        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PasswordBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CheckEntries();
            }
        }

        private void TbName_GotFocus(object sender, RoutedEventArgs e)
        {
            GlobalFunc.VirtualKeyboardInputToTextBox(Mouse.GetPosition(this), tbName);
        }
    }
}
