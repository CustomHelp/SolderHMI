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
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace LOET_HMI
{

    public partial class UserHMI
    {
        public int id { get; set; }
        public string sUserName { get; set; }
        public string sPassword { get; set; }
        public int    iUserLevel { get; set; }
        public string sUserLevelName { get; set; }

        public UserHMI(db_user dbUser)
        {
            id = dbUser.id;
            sUserName = dbUser.sUserName;
            sPassword = dbUser.sPassword;
            //sUserLevel = dbUser.db_userlevel.sLevel; 

            iUserLevel      = dbUser.iUserLevel; // Balog 22.6.2020

            try
            {
                sUserLevelName = GlobalVar.Userlevels.list.Single(u => u.iLevel == iUserLevel).sName; // Balog 22.6.2020
            }
            catch
            {
                MessageBox.Show("Userlevel wurde zu " + sUserName + " nicht gefunden.",
                                nameof(PgUserManagement),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                sUserLevelName = "error";
            }
        }
    }


    public partial class PgUserManagement : Page
    {
        List<db_user> ListUser = new List<db_user>();
        //List<db_userlevel> ListUserLevel = new List<db_userlevel>();

        List<UserHMI> ListUserHMI = new List<UserHMI>();
        public CollectionViewSource itemCollectionViewSource = new CollectionViewSource();
        private int iIndex;
        UserHMI SelectedUser;

        public PgUserManagement()
        {
            InitializeComponent();

            Refresh();
        }

        private void DG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            iIndex = dG.SelectedIndex;

            SelectedUser = dG.SelectedItem as UserHMI;
        }
        private void PopUpWindow_Closing(object sender, CancelEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    try
                    {
                        ListUser = context.db_user.ToList();
                        //ListUserLevel = context.db_userlevel.ToList();
                    }
                    catch
                    {
                        ;
                    }
                }

                ListUserHMI.Clear();
                for (int i = 0; i < ListUser.Count; i++)
                {
                    ListUserHMI.Add(new UserHMI(ListUser[i]));
                    ;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,
                                nameof(PgUserManagement) + " " + nameof(Refresh),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            itemCollectionViewSource.Source = null;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("itemCollectionViewSource"));
            itemCollectionViewSource.Source = ListUserHMI;

            dG.SelectedItem = null;
        }


        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();

            // if (GlobalVar.ActUser.iUserLevel >= UserRights.UserAdministration)
            if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel) // Balog 22.06.2020
            {
                

                WindowNewUser window = new WindowNewUser();
                window.Closing += PopUpWindow_Closing;
                window.ShowDialog();
            }
            else
                MessageBox.Show(                   
                    GlobalVar.Userlevels.Msg.sAccesDeniedText,
                    GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                    MessageBoxButton.OK, 
                    MessageBoxImage.Stop
                    );

            GlobalFunc.PopUp_SetMainWBackgrNormal();

        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            //if (GlobalVar.ActUser.iUserLevel >= UserRights.UserAdministrator)

            GlobalFunc.PopUp_SetMainWBackgrDark();

            if (SelectedUser != null)
            {
                if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel // der User, der die Änderung durchführt, mindestens RENA_Service-Level hat
                    &&
                    Convert.ToInt32(SelectedUser.iUserLevel) <= GlobalVar.ActUser.iUserLevel) // der User, der geändert wird, höchstens RENA_Service-Level hat   //Balog 22.06.2020
                {
                    

                    Window_DeleteUser window = new Window_DeleteUser(SelectedUser);
                    window.Closing += PopUpWindow_Closing;
                    window.ShowDialog();
                }
                else
                    MessageBox.Show(
                        GlobalVar.Userlevels.Msg.sAccesDeniedText,
                        GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop
                        );
            }
            else
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sNoSelectionText,
                    GlobalVar.Userlevels.Msg.sNoSelectionCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );

            GlobalFunc.PopUp_SetMainWBackgrNormal();

        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            //if (GlobalVar.ActUser.iUserLevel >= UserRights.UserAdministrator)

            GlobalFunc.PopUp_SetMainWBackgrDark();

            if (SelectedUser != null)
            {
                

                if (GlobalVar.ActUser.iUserLevel >= GlobalVar.Userlevels.Supervisor.iLevel // der User, der die Änderung durchführt, mindestens RENA_Service-Level hat
                    &&
                    Convert.ToInt32(SelectedUser.iUserLevel) <= GlobalVar.ActUser.iUserLevel) // der Uer, der geändert wird, höchstens RENA_Service-Level hat   //Balog 22.06.2020
                {
                    

                    WindowChangeUser window = new WindowChangeUser(SelectedUser);
                    window.Closing += PopUpWindow_Closing;
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show(
                        GlobalVar.Userlevels.Msg.sAccesDeniedText,
                        GlobalVar.Userlevels.Msg.sAccesDeniedCaption,
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop
                        );
                }
            }
            else
            {
                MessageBox.Show(
                    GlobalVar.Userlevels.Msg.sNoSelectionText,
                    GlobalVar.Userlevels.Msg.sNoSelectionCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop
                    );


            }


            GlobalFunc.PopUp_SetMainWBackgrNormal();


        }
    }
}