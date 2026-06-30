using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOET_HMI
{
    public enum ModulIDs
    {
        NoModul = 0,
        Modul1 = 1,     // Modul 1 // Laser
        Modul2 = 2,     // Modul 2 // Karton Aufstellen, Dosen Befüllen, Karton schließen und Bereitstellen
        Modul3 = 3,     // Modul 3 // Roboter und Wickler 
    }

    public enum StationIDs
    {
        NoStation = 0,

        // Modul 1                                                         
        gc_St1 = 1, // Station 1

    }

    public enum eComponentState
    {
        // OperationMode
        CS_00_Normal = 0,   // 00 - Komponente OK										// Lila
        CS_10_Fault = 10,   // 10 - Komponente hat Fehler								// Rot
        CS_20_Manual = 20,  // 20 - Bei der Komponente ist eine Handfunktion aktiv		// Blau
        CS_30_Wait = 30,    // 30 - Komponente wartet 									// Gelb
        CS_40_Warn = 40,    // 40 - Komponente meldet Warnung							// Gelb
        CS_50_Mess = 50     // 50 - Komponente Meldet Meldung
    }

    public enum eDeviceOnOffType
    {
        DT_00_NoType 		= 0,
        DT_10_ToggleSwitch 	= 10, // 10 - Umschalter: als Eingang braucht es eine Flanke und es kippt den Ausgang.
        DT_20_PushButton	= 20  // 20 - Schalter: der aktuelle Eingangswert wird direkt an den Ausgang geleitet
    }

    public enum eCounterType
    {
        CT_10_Total_ShowDetail		= 10,  // Angezeigt werden: Total, i.O, n.i.O, Quote (schlecht von Gesamt)
        CT_11_Total_DontShowDetail	= 11,  // Angezeigt werden: Total
        CT_20_Act					= 20   // Angezeigt werden: Act, Target
    }


    public static class ComponentStateTxtsDE
    {
        public static string strCS_00_Normal    = "OK"; 
        public static string strCS_10_Fault     = "Fehler"; 
        public static string strCS_20_Manual    = "Handbetrieb";
        public static string strCS_30_Wait      = "Warten";  
        public static string strCS_40_Warn      = "Warnung";  
        public static string strCS_50_Mess      = "Meldung";  
    }

    public static class ComponentStateTxtsEN
    {
        public static string strCS_00_Normal    = "OK"; 
        public static string strCS_10_Fault     = "Error"; 
        public static string strCS_20_Manual    = "Manual on";
        public static string strCS_30_Wait      = "Waiting";  
        public static string strCS_40_Warn      = "Warning";  
        public static string strCS_50_Mess      = "Message";  
    }

}
