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
using LOET_HMI.PLC_Com_Classes;
using System.Windows.Threading;
using System.Threading;
using LOET_HMI.SystemPages.PopUps;

namespace LOET_HMI
{

    public partial class Window_LogIn : Window
    {
        IList<db_user> listUser;
        public db_user userToLogIn; // früher ActUser

        private VirtualKeyboard virtualKeyboard;
        private bool bLoggedOut { get; set; }
        private bool bUserSelected { get; set; }

        // ADS Verbindung
        IADSConnection VarCon = new ADSService();

        public Window_LogIn()
        {
            InitializeComponent();


            try // MBA 19.8.2020
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    listUser = context.db_user.ToList();

                    if(listUser.Count == 0) // wenn das HMI an der SPS zum 1. Mal gestartet wird und die Datenbank noch leer ist   -> chp-Benutzer hier anlegen.
                    {
                        MessageBox.Show("Die Datenbank enthält aktuell keine Benutzer. Der standard CHP-Benutzer wird zur Datenbank hinzugefügt",// Text
                                        "Erster Benutzer", // Überschrift
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation
                                        );

                        db_user userIfDBempty = new db_user();
                        userIfDBempty.sUserName = "chp (DB empty)";
                        userIfDBempty.sPassword = "chpchp";
                        userIfDBempty.iUserLevel = GlobalVar.Userlevels.Administrator.iLevel;

                        context.db_user.Add(userIfDBempty);
                        context.SaveChanges();

                        listUser.Add(userIfDBempty);
                    }



                }


                Wrap.Children.RemoveRange(0, Wrap.Children.Count);
                if (listUser.Count > 0)
                {
                    for (int i = 0; i < listUser.Count; i++)
                    {
                        Button btn = new Button();
                        btn.Style = BtnTemplate.Style;
                        btn.Width = BtnTemplate.Width;
                        btn.Height = BtnTemplate.Height;
                        btn.Margin = BtnTemplate.Margin;
                        btn.Name = "User" + (i + 1).ToString();
                        btn.Content = listUser[i].sUserName;
                        btn.Click += BtnAnyUser_Click;
                        Wrap.Children.Add(btn);
                    }
                }

                tblLogOut.Visibility = Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("Problem mit der Datenbank", // Text 
                                "", // Überschrift
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
            }


        }

        private void BtnAnyUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (CHP_HMIEntities context = new CHP_HMIEntities())
                {
                    userToLogIn = context.db_user.Single(u => u.sUserName == ((Button)sender).Content.ToString());
                }
                bUserSelected = true;

                tblName.Text = userToLogIn.sUserName;
                //tblLevel.Text = ActUser.db_userlevel.sLevel; // Balog 22.6.2020
                tblLevel.Text = GlobalVar.Userlevels.list.Single(u => u.iLevel == userToLogIn.iUserLevel).sName; // Balog 22.6.2020 IBN, DB_TEST!!!
            }
            catch
            {
                MessageBox.Show("Laden der Benutzerinformationen fehlgeschlagen.", // Text
                                "Fehler", // Überschrift
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (bLoggedOut && !bUserSelected) // Ausloggen
                Close();
            else // Einloggen
                CheckEntries();
        }

        public void CheckEntries()
        {
            // Einloggen
            if(userToLogIn.sUserName != Properties.Resources.DlgUser_tbName) // "Kein Benutzer"/"no user"
            {
                if (passwordBox.Password == userToLogIn.sPassword )
                {
                    tblPasswort.Foreground = Brushes.White;
                    passwordBox.BorderBrush = Brushes.White;
                    if (GlobalVar.ActUser == null)
                        GlobalVar.ActUser = new User("", "", 0);

                    GlobalVar.ActUser.sUserName  = userToLogIn.sUserName;
                    GlobalVar.ActUser.iUserLevel = userToLogIn.iUserLevel;

                    if(GlobalVar.debugADS.bWantPLCConnect) // MB 7.8.2020
                    {
                      VarCon.WriteItem("GVL_Basic.gstrUserName", userToLogIn.sUserName);
                      VarCon.WriteItem("GVL_Basic.giUserLevel", userToLogIn.iUserLevel); // Balog 22.6.2020 
                      VarCon.WriteItem("GVL_Basic.gbLoggedIn", true);
                    }

                    DBLog.Handler.User(userToLogIn.sUserName, userToLogIn.iUserLevel, true);

                    if(IsActive) // MBA 25.5.2021: die Methode CheckEntries() wird auch vom Main aufgerufen, aber das Login-Fenster wird da nicht angezeigt. So kann man das DialogResult-Bit nicht schreiben 
                        DialogResult = true;
                    this.Close();
                }
                else
                {
                    tblPasswort.Foreground = Brushes.Red;
                    passwordBox.BorderBrush = Brushes.Red;
                }
            }
            else
            {
                tblPasswort.Foreground = Brushes.Red;
                passwordBox.BorderBrush = Brushes.Red;
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

        private void PasswordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            TextFieldClicked();
        }

        private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //FrameworkElement element = sender as FrameworkElement;
            //var pos = e.GetPosition(spStatus);
            //// Zur Übersicht für Position und Größe der Textbox
            //var buf = passwordBox.TransformToAncestor(spStatus).Transform(new Point(0, 0)); // 132, 62.5
            //var height = passwordBox.ActualHeight; //35
            //var width = passwordBox.ActualWidth; //180
            //
            //int bufZoneWidth = 20;
            //
            //// Positionsabfrage ob Mouse-Klick innerhalb Pufferzone (10px um das Textfeld)
            //if ((pos.X >= buf.X - bufZoneWidth && pos.X <= (buf.X + width + bufZoneWidth)) && (pos.Y >= buf.Y - bufZoneWidth && pos.Y <= (buf.Y + height + bufZoneWidth))) // 
            //{
            //    if (!((pos.X >= buf.X && pos.X <= (buf.X + width)) && (pos.Y >= buf.Y && pos.Y <= (buf.Y + height)))) // ClickLogic() nur im Rahmen um dem Textfeld herum aufrufen. Im Textfeld nicht mehr
            //    {
            //        TextFieldClicked();
            //    }
            //}
        }

        private void TextFieldClicked()
        {
             DispatcherOperation dispOperation = Dispatcher.BeginInvoke(
                new Action(() =>
                {                   
                    virtualKeyboard = new VirtualKeyboard(Mouse.GetPosition(this), passwordBox.Password, typeof(PasswordBox));
                    virtualKeyboard.ShowDialog();

                    if (virtualKeyboard.DialogResult == true)
                        passwordBox.Password = virtualKeyboard.AnswerPasswordBox;


                }
                ), DispatcherPriority.Normal, null);
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bUserSelected = false;

            tblLevel.Text = Properties.Resources.DlgUser_lblSelectAUser;
            tblLevel.Text = "";

            using (CHP_HMIEntities context = new CHP_HMIEntities())
            {

                // MBA 11.12.2020:
                if(GlobalVar.ActUser == null)
                    GlobalVar.ActUser = new User("", "", 0);

                try
                {
                    userToLogIn = context.db_user.Single(u => u.sUserName == GlobalVar.ActUser.sUserName);
                    //loginUser.db_userlevel = context.db_userlevel.Single(u => u.id == loginUser.Userlevel); //Balog 22.6.2020 DB_TEST!!!
                    
                }
                catch { }

            }


            if ( userToLogIn != null)
            {
                try
                {
                    tblName.Text = userToLogIn.sUserName;
                    //tblLevel.Text = loginUser.db_userlevel.sLevel.ToString();
                    tblLevel.Text = GlobalVar.Userlevels.list.Single(u => u.iLevel == userToLogIn.iUserLevel).sName; // Balog 22.6.2020

                    for (int i = 0; i < Wrap.Children.Count; i++)
                    {
                        Button btn = Wrap.Children[i] as Button;
                        btn.IsEnabled = false;
                    }

                    tblLogOut.Visibility = Visibility.Visible;
                }
                catch
                {
                    ;
                }

            }
            bLoggedOut = false;         
        }

        private void TblLogOut_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVar.debugADS.bWantPLCConnect)
            {
                VarCon.WriteItem("GVL_Basic.gstrUserName", "");
                VarCon.WriteItem("GVL_Basic.giUserLevel", 0);
                VarCon.WriteItem("GVL_Basic.gbLoggedIn", false);
            }


            DBLog.Handler.User(GlobalVar.ActUser.sUserName, GlobalVar.ActUser.iUserLevel, false);

            GlobalVar.ActUser = new User("", "", 0);


            for (int i = 0; i < Wrap.Children.Count; i++)
            {
                Button btn = Wrap.Children[i] as Button;
                btn.IsEnabled = true;
            }

            tblLogOut.Visibility = Visibility.Collapsed;
            tblName.Text = Properties.Resources.DlgUser_lblSelectAUser;
            tblLevel.Text = "";

            bLoggedOut = true; // Merker, damit das Fenster anschließend mit OK geschlossen werden kann

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GlobalVar.ActUser.sUserName == "")
                    GlobalFunc.ActivateNoUserRect();
                else
                    GlobalFunc.DeactivateNoUserRect();
            }
            catch { GlobalFunc.ActivateNoUserRect(); }
        }
    }
}
