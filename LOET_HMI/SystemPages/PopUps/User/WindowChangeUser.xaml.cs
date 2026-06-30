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
using System.Windows.Threading;
using LOET_HMI.SystemPages.PopUps;

namespace LOET_HMI
{

    public partial class WindowChangeUser : Window
    {
        private UserHMI User;
        private List<db_user> ListUser = new List<db_user>();


        public WindowChangeUser(UserHMI user)
        {
            InitializeComponent();
            User = user;
            
            tbUserName.Text = User.sUserName;
            passwordBox.Password = User.sPassword;


            cbLevel.Items.Add(GlobalVar.Userlevels.Default.sName);
            cbLevel.Items.Add(GlobalVar.Userlevels.Operator.sName);
            cbLevel.Items.Add(GlobalVar.Userlevels.Quality.sName);

            //if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Maintenance.iLevel)
            cbLevel.Items.Add(GlobalVar.Userlevels.Maintenance.sName);

            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
                cbLevel.Items.Add(GlobalVar.Userlevels.Supervisor.sName);

            if (GlobalVar.ActUser.iUserLevel == GlobalVar.Userlevels.Administrator.iLevel)
                cbLevel.Items.Add(GlobalVar.Userlevels.Administrator.sName);
            //// Balog 22.6.2020
            //for (int i=0; i < GlobalVar.Userlevels.list.Count; i++)
            //{
            //    if(GlobalVar.ActUser.iUserLevel         >=  GlobalVar.Userlevels.Supervisor.iLevel
            //        &&
            //       GlobalVar.Userlevels.list[i].iLevel  <= GlobalVar.Userlevels.Supervisor.iLevel) 
            //    {
            //        cbLevel.Items.Add(GlobalVar.Userlevels.list[i].sName); // Zur Combobox die User-Level optionen als Text hinzufügen
            //    }
            //}

            int Index = 0;
            for (int i = 0; i < cbLevel.Items.Count; i++)
            {
                if ((string)cbLevel.Items[i] == User.sUserLevelName)
                    Index = i;
            }
            cbLevel.SelectedIndex = Index;
        }



        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CheckEntries();
        }

        private void CheckEntries()
        {
            if ((tbUserName.Text != ""))
            {
                if ((passwordBox.Password != ""))
                {
                    using (CHP_HMIEntities context = new CHP_HMIEntities())
                    {
                        db_user changeUser = context.db_user.FirstOrDefault(u => u.id == User.id);
                        changeUser.sUserName = tbUserName.Text;
                        changeUser.sPassword = passwordBox.Password;

                        var cbSelectedItem = (string)cbLevel.SelectedItem; // Combo-Box, ausgewählter Eintrag

                        var selectedUserLevel = GlobalVar.Userlevels.list.Single(u => u.sName == cbSelectedItem);

                        changeUser.iUserLevel = selectedUserLevel.iLevel; // // Balog 22.6.2020 DB_TEST!!!

                        context.SaveChanges();
                    }
                    this.Close();
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TbUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
              new Action(() =>
              {
                  VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), tbUserName.Text, typeof(TextBox));
                  virtualKeyboard.ShowDialog();

                  if (virtualKeyboard.DialogResult == true)
                      tbUserName.Text = virtualKeyboard.AnswerTextBox;


              }
              ), DispatcherPriority.Normal, null);
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                   new Action(() =>
                   {
                       VirtualKeyboard virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), passwordBox.Password, typeof(PasswordBox));
                       virtualKeyboard.ShowDialog();

                       if (virtualKeyboard.DialogResult == true)
                           passwordBox.Password = virtualKeyboard.AnswerPasswordBox;


                   }
                   ), DispatcherPriority.Normal, null);
        }
    }
}
