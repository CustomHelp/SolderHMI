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

    public partial class Window_DeleteParamSet : Window
    {

        private ParamSetTypes _ParamSet;
        private string _sName;

        public Window_DeleteParamSet(string sName, ParamSetTypes paramSet)
        {
            InitializeComponent();

            _ParamSet = paramSet;
            _sName = sName;

            tblName.Text = _sName;
        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_sName != "")
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    db_paramset ParamSet;
                    List<db_parameter> ParamList;

                    ParamSet = context.db_paramset.First(r => ((r.sName == _sName) && ((r.iType == (int)_ParamSet))));
                    ParamList = ParamSet.db_parameter.ToList();

                    for (int i = 0; i < ParamList.Count; i++)
                    {
                        context.db_parameter.Remove(ParamList[i]);
                    }
                    context.SaveChanges();
                    context.db_paramset.Remove(ParamSet);
                    context.SaveChanges();

                    DialogResult = true;
                }
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
