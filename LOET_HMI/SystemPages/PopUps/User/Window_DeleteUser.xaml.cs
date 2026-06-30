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

    public partial class Window_DeleteUser : Window
    {
        private UserHMI _User;

        public Window_DeleteUser(UserHMI user)
        {
            InitializeComponent();

            _User = user;

            tblName.Text = _User.sUserName;
        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    db_user changeUser = context.db_user.FirstOrDefault(u => u.id == _User.id);

                    if (GlobalVar.ActUser.iUserLevel >= _User.id   &&   GlobalVar.ActUser.sUserName!=_User.sUserName)
                        context.db_user.Remove(changeUser);
                    else
                    {
                        MessageBox.Show(
                            "You don't have permission to perform this operation.",
                            "No permission", MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }

                    context.SaveChanges();
                }
            }
            catch { }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
