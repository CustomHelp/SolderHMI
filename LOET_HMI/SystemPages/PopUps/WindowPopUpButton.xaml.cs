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
using System.Globalization;
using System.Reflection;
using System.Threading;



namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpSetOutput.xaml
    /// </summary>
    public partial class WindowPopUpButton : Window
    {





        public WindowPopUpButton(string sHeader, string sText, string sOKbtnText)
        {
           
            InitializeComponent();

            tbHeader.Text   = sHeader;
            tbText1.Text    = sText;
            if(sOKbtnText != "")
                btnYes.Content  = sOKbtnText;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             


        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {         
                   
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
