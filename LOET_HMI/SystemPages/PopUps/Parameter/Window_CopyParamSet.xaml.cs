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

    public partial class Window_CopyParamSet : Window
    {

        private ParamSetTypes _ParamSet;
        private string _sName;

        public Window_CopyParamSet(string sName, ParamSetTypes paramSet)
        {
            InitializeComponent();

            _ParamSet = paramSet;
            _sName = sName;
            tbActName.Text = sName;
            tbName.Text = sName + " - Copy";
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
                        // Set zum kopieren holen
                        List<db_parameter> ParamList;

                        ParamSet = context.db_paramset.Single(r => ((r.sName == _sName) && ((r.iType == (int)_ParamSet))));
                        ParamList = ParamSet.db_parameter.ToList();
                       
                        db_paramset CopyParamSet = new db_paramset();
                        CopyParamSet.sName = sName;
                        CopyParamSet.iType = ParamSet.iType;
                        CopyParamSet.dtCreatedOn = DateTime.Now; // Balog 22.10.2019
                        CopyParamSet.sCreatedBy = GlobalVar.ActUser.sUserName; // Balog 22.10.2019
                        CopyParamSet.dtLastModified = CopyParamSet.dtCreatedOn; // Balog 22.10.2019


                        for (int i = 0; i < ParamList.Count; i++)
                        {
                            db_parameter NewParameter  = new db_parameter();

                            NewParameter.sValue = ParamList[i].sValue;
                            // MB 3.8.2020: auskommentiert wegen DB-Modellaktualisierung
                            //NewParameter.sUnit = ParamList[i].sUnit;
                            //NewParameter.sMin = ParamList[i].sMin;
                            //NewParameter.sMax = ParamList[i].sMax;
                            //NewParameter.sHMIName = ParamList[i].sHMIName;
                            NewParameter.sADSName = ParamList[i].sADSName;
                            //NewParameter.iUserLevel = ParamList[i].iUserLevel;
                            //NewParameter.iType = ParamList[i].iType;
                            //NewParameter.iStation = ParamList[i].iStation; // Balog 22.10.2019

                            CopyParamSet.db_parameter.Add(NewParameter);
                        }
                        
                        context.db_paramset.Add(CopyParamSet);
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
