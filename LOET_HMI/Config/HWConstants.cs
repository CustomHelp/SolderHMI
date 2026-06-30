using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOET_HMI
{
    internal static class GVLRefArrays
    {
        public static string cCylinder  = "GVL_Cylinder.g_arrCylRef";
        public static string cSensor    = "GVL_Sensor.g_arrSensorBoolRef";
        public static string cFeeder    = "GVL_Feeder.g_arrFeederHMIRef";
        public static string cAxis      = "GVL_Axis.g_arrAxisHMI";
        public static string cDevices   = "GVL_DeviceOnOff.g_arrDevOnOffRef";
        public static string cCounter   = "GVL_Counter.g_arrCounterXOfY";
    }

    internal static class GVLarrNr_Cylinder
    {
        public static int gcCYL_St10_Dock_Inductor				= 01;		// Induktor zustellen
	    public static int gcCYL_St10_Clamp_Terminal				= 02;		// Kabelschuh klemmen
	    public static int gcCYL_St10_Lock_Drawer				= 03;		// Schub verriegeln
	    public static int gcCYL_St10_Maintenance_Solder			= 04;		// Lötdrahtvorschub Wartungsposition
	    public static int gcCYL_St10_Dock_Solder				= 05;		// Lötdrahtvorschub andocken
	    public static int gcCYL_St10_SwitchCooling				= 06;		// Küchlluft Umschalten Oben / Unten
	    public static int gcCYL_St20_Dock_Inductor				= 11;		// Induktor zustellen
	    public static int gcCYL_St20_Clamp_Terminal				= 12;		// Kabelschuh klemmen
	    public static int gcCYL_St20_Lock_Drawer				= 13;		// Schub verriegeln
	    public static int gcCYL_St20_Maintenance_Solder			= 14;		// Lötdrahtvorschub Wartungsposition
	    public static int gcCYL_St20_Dock_Solder				= 15;		// Lötdrahtvorschub andocken
	    public static int gcCYL_St20_SwitchCooling				= 16;		// Küchlluft Umschalten Oben / Unten

    }


    internal static class GVLarrNr_DevOnOff
    {
        public static int gcDEV_St0_Vaccum = 01;
        public static int gcDEV_St0_Generator = 02;
        public static int gcDEV_St0_Cooler = 03;
        public static int gcDEV_St1_SwitchCooling = 04;
        public static int gcDEV_St2_SwitchCooling = 05;

    }


    internal static class GVLarrNr_Button
    {

    }


    internal static class SensorBOOLenums
    {

    }

    internal static class ButtonEnums
    {


    }








    


}
