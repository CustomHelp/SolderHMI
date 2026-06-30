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
using LOET_HMI.SystemPages.PopUps;

namespace LOET_HMI
{

    public partial class Window_NewParamSet : Window
    {

        private ParamSetTypes _ParamSet;

        public Window_NewParamSet(ParamSetTypes paramSet)
        {
            InitializeComponent();

            _ParamSet = paramSet;
        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CheckEntries();

            /*
            ((MainWindow)Application.Current.MainWindow).rena_HMI.PopUp_SetMainWBackgrDark((MainWindow)Application.Current.MainWindow);
            WindowPopUpNewRecipe_v2 winEditAllParam = new WindowPopUpNewRecipe_v2(_ParamSet);
            winEditAllParam.ShowDialog();
            ((MainWindow)Application.Current.MainWindow).rena_HMI.PopUp_SetMainWBackgrNormal((MainWindow)Application.Current.MainWindow);
            */
        }

        private void CheckEntries()
        {
            // Name schon vergeben?
            string sName = tbName.Text;

            if (sName != "")
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
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
                        //DBParam.Handler.NewParamSet(sName, _ParamSet,   new DBParamHandler.TypeParameterInitValues());
                        DBParam.Handler.NewParamSet(sName, _ParamSet);

                        DialogResult = true; //Balog
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

        private void TbName_GotFocus(object sender, RoutedEventArgs e)
        {
            GlobalFunc.VirtualKeyboardInputToTextBox(Mouse.GetPosition(this), tbName);
        }
    }
}
