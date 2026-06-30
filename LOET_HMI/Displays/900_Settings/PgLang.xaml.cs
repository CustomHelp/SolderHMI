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
using System.Globalization;
using System.Threading;
using System.Windows.Threading;

namespace LOET_HMI.Displays
{
    /// <summary>
    /// Interaktionslogik für PgLang.xaml
    /// </summary>
    public partial class PgLang : Page
    {
        // ADS Verbindung
        IADSConnection VarCon = new ADSService();
        private List<ADSItem> ItemList = new List<ADSItem>();
        ADSItem tempItem;

        // Variablen
        Languages iActLanguage;


        public PgLang()
        {
            InitializeComponent();

            ItemList.Add(VarCon.AddItem("GVL_Basic.giLanguage", typeof(Int16)));

            VarCon.EnableCallbackEvent();
            VarCon.ItemChangeEvent += VarCon_ItemChangeEvent;
        }


        private void VarCon_ItemChangeEvent(object sender, ADSItemChangeEventArgs e)
        {
            bool bItemChanged = false;
            for (int j = 0; j < e.Item.Count; j++)
            {
                for (int i = 0; i < ItemList.Count; i++)
                {
                    if (ItemList[i].iHandle == e.Item[j].iHandle)
                    {
                        ItemList[i].Value = e.Item[j].Value;
                        bItemChanged = true;
                    }
                }
            }

            if (bItemChanged)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    tempItem = new ADSItem();
                    tempItem = ItemList.Find(s => s.sName == "GVL_Basic.giLanguage");
                    if (tempItem.Value != null)
                        iActLanguage = (Languages)(Int16)tempItem.Value;
                    else
                        iActLanguage = (Languages)1;


                    /*var brush = new ImageBrush();
                    switch (iActLanguage)
                    {
                        case Languages.deutsch:
                            brush.ImageSource = new BitmapImage(new Uri("Resources/flag_deutschland.png", UriKind.Relative));
                            break;

                        case Languages.englisch:
                            brush.ImageSource = new BitmapImage(new Uri("Resources/flag_usa.png", UriKind.Relative));
                            break;
                    }
                    btnSprache.Background = brush;
                    */
                }));
            }
        }

        private void BtnDeutsch_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();

            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "de-DE";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.deutsch); // Sprachumschaltung SPS (beeinflusst die Sprache der PopUp-Fenster)
            App.ChangeCulture(new CultureInfo("de-DE"));// Sprachumschaltung HMI (Umschaltung zwischen Resource-Dateien)
            GlobalVar.Language = Languages.deutsch; // Sprachumschaltung bei Messages 
        }

        private void BtnEnglish_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "en-US";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.englisch); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("en-US"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.englisch; // Sprachumschaltung bei Messages 
        }

        private void BtnChinese_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "zh-CN";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.chinesisch); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("zh-CN"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.chinesisch; // Sprachumschaltung bei Messages 
        }

        private void BtnSlovak_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "sk-SK";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.slovakian); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("sk-SK"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.slovakian; // Sprachumschaltung bei Messages 
        }

        public void Message()
        {
            MessageBox.Show(
                Properties.Resources.MsgBox_LanguageRestart_Text,//"The HMI restarts with the selected language. Please wait.",
                Properties.Resources.MsgBox_LanguageRestart_Caption,//"Language selected",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }

        private void BtnBotswana_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "en-US";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.englisch); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("en-US"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.englisch; // Sprachumschaltung bei Messages 
        }

        private void BtnArabic_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "ar-AE";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.arabisch); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("ar-AE"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.arabisch; // Sprachumschaltung bei Messages
        }

        private void BtnFrench_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "fr-FR";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.french); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("fr-FR"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.french; // Sprachumschaltung bei Messages
        }

        private void BtnSpanish_Click(object sender, RoutedEventArgs e)
        {
            GlobalFunc.PopUp_SetMainWBackgrDark();
            Message();
            LOET_HMI.Properties.Settings.Default.strstartUpCulture = "es-ES";
            Properties.Settings.Default.Save();
            ADSMain.ADSComServer.WriteItem("GVL_Basic.giLanguage", Languages.spanish); // Sprachumschaltung SPS
            App.ChangeCulture(new CultureInfo("es-ES"));// Sprachumschaltung HMI
            GlobalVar.Language = Languages.spanish; // Sprachumschaltung bei Messages
        }
    }
}
