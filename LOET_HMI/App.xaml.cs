using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;

namespace LOET_HMI
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        // 1. Konstruktor hinzufügen
        public App()
        {
            // Dieser Switch MUSS hier stehen, ganz am Anfang!
            AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport", true);
        }


        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        public static bool cultureIsBeingChanged { get; set; }
        private Mutex mutex;
        /// <summary>
        /// Quelle: https://jeremybytes.blogspot.com/2013/07/changing-culture-in-wpf.html
        /// </summary>
        /// <param name=""></param>
        public static void ChangeCulture(CultureInfo newCulture)
        {
            cultureIsBeingChanged = true;

            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            GlobalFunc.RestartHMI(newCulture);

            cultureIsBeingChanged = false;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Zentralen Datei-Logger moeglichst frueh initialisieren (wirft nie).
            AppLogger.Init();

            //MessageBox.Show("Programm Opens");
            bool newInstance = false;
            mutex = new Mutex(true, "LOET_HMI", out newInstance);
            if (!newInstance)
            {
                //MessageBox.Show("Mutex Handle");
                App.Current.Shutdown();
            }
            try
            {
                string culture = LOET_HMI.Properties.Settings.Default.strstartUpCulture;
                CultureInfo startUpCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = startUpCulture;
                Thread.CurrentThread.CurrentUICulture = startUpCulture;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim laden der Sprache " + ex.Message);
                CultureInfo startUpCulture = CultureInfo.GetCultureInfo("es-ES");
                Thread.CurrentThread.CurrentCulture = startUpCulture;
                Thread.CurrentThread.CurrentUICulture = startUpCulture;
            }
            #region Old language selection
            //if (e.Args.Length > 0)
            //{
            //    var test = e.Args[0];
            //    string culture = test.ToString();
            //    CultureInfo startUpCulture = CultureInfo.GetCultureInfo(culture);
            //    Thread.CurrentThread.CurrentCulture = startUpCulture;
            //    Thread.CurrentThread.CurrentUICulture = startUpCulture;
            //    switch (startUpCulture.TwoLetterISOLanguageName.ToString())
            //    {
            //        case "ar":
            //            GlobalVar.Language = Languages.arabisch;
            //            break;

            //        case "fr":
            //            GlobalVar.Language = Languages.french;
            //            break;

            //        default:
            //            GlobalVar.Language = Languages.englisch;
            //            break;
            //    }
            //}
            //else
            //{
            //    //CultureInfo startUpCulture = CultureInfo.GetCultureInfo("fr-FR");
            //    //Thread.CurrentThread.CurrentCulture = startUpCulture;
            //    //Thread.CurrentThread.CurrentUICulture = startUpCulture;
            //    //GlobalVar.Language = Languages.french;
            //}
            ////MessageBox.Show("Selected Culture: " + Thread.CurrentThread.CurrentCulture.DisplayName);
            #endregion
            #region Check Instruction Paths
            try
            {
                // Laufwerk-Fallback (D: sonst C:) + automatisches Anlegen der
                // Documentation_Videos\ / Documentation_Documents\-Ordnerstruktur.
                DocumentationPaths.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Fetching Data: " + ex.Message);
            }
            #endregion
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //MessageBox.Show(sender.ToString());
            IntPtr pHandle = GetCurrentProcess();
            SetProcessWorkingSetSize(pHandle, -1, -1);
        }
    }
}
