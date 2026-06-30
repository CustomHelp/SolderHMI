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

    public partial class WindowNewUser : Window
    {
        private List<db_user> ListUser = new List<db_user>();

        public List<db_userlevel> ListUserlevels = new List<db_userlevel>();

        public WindowNewUser()
        {
            InitializeComponent();


            cbLevel.Items.Add(GlobalVar.Userlevels.Default.sName);
            cbLevel.Items.Add(GlobalVar.Userlevels.Operator.sName);
            cbLevel.Items.Add(GlobalVar.Userlevels.Quality.sName);

            //if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Maintenance.iLevel)
            cbLevel.Items.Add(GlobalVar.Userlevels.Maintenance.sName);

            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel)
                cbLevel.Items.Add(GlobalVar.Userlevels.Supervisor.sName);

            if (GlobalVar.ActUser.iUserLevel == GlobalVar.Userlevels.Administrator.iLevel)
                cbLevel.Items.Add(GlobalVar.Userlevels.Administrator.sName);

            cbLevel.SelectedIndex = 0;
        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {
                ListUser = context.db_user.ToList();
            }

            if ((tbUserName.Text != ""))
            {
                if (ListUser.FindIndex(u => u.sUserName == tbUserName.Text) < 0)
                {
                    if ((passwordBox.Password != ""))
                    {
                        using (CHP_HMIEntities context = new CHP_HMIEntities())
                        {
                            //int dbid = GlobalVar.ListUserLevel.Find(u => u.sLevel == cbLevel.Text).id;
                            //db_userlevel lvl = context.db_userlevel.Single(c => c.id == dbid);

                            int lvl = GlobalVar.Userlevels.list.Find(u => u.sName == cbLevel.Text).iLevel;

                            db_user User = new db_user();
                            User.sUserName = tbUserName.Text;
                            User.sPassword = passwordBox.Password;
                            //User.db_userlevel = lvl;
                            User.iUserLevel = lvl; // Balog 22.6.2020

                            context.db_user.Add(User);
                            context.SaveChanges();
                        }

                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(
                    "User already exists",
                    "New user",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
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
