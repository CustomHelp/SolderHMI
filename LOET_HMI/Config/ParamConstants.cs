using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOPA_HMI
{
    /*
    internal static class FilterByStation
    {
        public const int Modul1 = 1;
        public const int Modul2 = 2;
        public const int Modul3 = 3;
        public const int Modul4 = 4;
        public const int Modul5 = 5;
        public const int Tool           = 6;
        public const int TransportUnit  = 7;
    }
    */


    internal static class ParamNamesRecipe
    {
        
        // ****************************************************************
        // ********** DropDown-Menüs(=ComboBox), Item-Typ string **********
        public const string CellType = "Cell Type";
        public const string Filling = "Filling";

        //public const string Prepressure         = "Prepressure";            // Vordruck
        public const string Afterpressure = "Afterpressure";          // Nachdruck
        public const string Afterrinse1Select = "Rinse 1 select";    // Nachspülen 1 (Spülen/Nachspülen, Rinse/Afterrinse sind gleich)
        public const string Afterrinse2Select = "Rinse 2 select";    // Nachspülen 2 (Spülen/Nachspülen, Rinse/Afterrinse sind gleich)    

        // ***************************************************************
        // ******************** TextBoxes, Typ DINT **********************
        public const string V_electr = "Filling volume electrolyte"; // Menge Elektrolyt
        public const string V_SO2 = "Filling volume  SO2";        // Menge SO2
        public const string T_electr = "Filling time electrolyte";   // Füllzyklus Elektrolyt
        public const string T_SO2 = "Filling time SO2";           // Füllzyklus SO2

        public const string T_Afterrinse1 = "Rinse 1 time";          // Zeit für Nachspülen 1 (Spülen/Nachspülen, Rinse/Afterrinse sind gleich)
        public const string T_Afterrinse2 = "Rinse 2 time";          // Zeit für Nachspülen 2 (Spülen/Nachspülen, Rinse/Afterrinse sind gleich)

        // ***************************************************************
        // ****************** RadioButton, Typ BOOL **********************
        //public const string LeakTestBeforeFill      = "Leakage test before filling"; // Dichtigkeit vor Befüllung
        public const string IntermediateEvacuation1 = "Intermediate evacuation 1";     // Zwischenevakuieren
        public const string IntermediateEvacuation2 = "Intermediate evacuation 2";     // Zwischenevakuieren
        //public const string LeakTestAfterFill       = "Leakage test after filling";  // Dichtigkeit nach Befüllung
        public const string ResisteWelding = "Resistance welding";          // Widerstandsschweißen
        public const string Pressing3 = "Pressing 3";                       // Pressen 3
        public const string LaserWelding = "Laser welding";                 // Laserschweißen
        

    }



    #region Dropdown-Menüs (Combobox in XAML)
    internal static class ParamDropDowns
    {
        //********************************************
        //******************* Rezept *****************
        //********************************************
        public static readonly string[] arrCellType =
        {
            "HPC",      // kleine Zelle, mit Endplatten
            "HPCM",     // kleine Zelle, ohne Endplatten
            "LFC",      // große Zelle
            "ESM8"      // großes 8-fach Modul


            //"LFCM",
            //"8er Modul"
        };



        public static readonly string[] arrFillingWith =
        {
            "SO2/Electrolyte",
            "N2"
        };


        /*public static readonly string[] arrPrepressure = //Vordruck
        {
            "Vacuum",
            "SO2 liquid",
            "SO2 gas"
        };*/

        public static readonly string[] arrAfterpressure = //Nachdruck
        {
            "Off",
            //"SO2 liquid",
            "SO2 gas",
        };

        public static readonly string[] arrAfterrinse1 = //Nachspülen 1
        {
            "Off",
            //"SO2 liquid",
            "SO2 gas",
            "N2 gas"
        };

        public static readonly string[] arrAfterrinse2 = //Nachspülen 2
        {
            "Off",
            //"SO2 liquid",
            "SO2 gas",
            "N2 gas"
        };


        //********************************************
        //*************** Maschinenpar. **************
        //********************************************
        public static readonly string[] arrDosStrategyRough = //
        {
            "1-1-1-1-1-1-1-1",
            "2-2-2-2",
            "4-4",
            "8"        
         };



    }

    #endregion



    internal static class ParamNamesModul3
    {
        // Pressing 1
        public const string Pressing1_pX0_setpoint = "Pressing 1, pressure setpoint X=0"; // pressure setpoint at X=0
        public const string Pressing1_pX1_setpoint = "Pressing 1, pressure setpoint X=1"; // pressure setpoint at X=1
        public const string Pressing1_pX5_setpoint = "Pressing 1, pressure setpoint X=5"; // pressure setpoint at X=5


        // Evakuierung
        public const string Evac_p_setpoint = "Evacuation, pressure setpoint"; // Sollwert
        public const string Evac_delta_p_max = "Evacuation, Δp_max";
        public const string Evac_delta_t1 = "Evacuation, Δt_1";
        public const string Evac_delta_t2 = "Evacuation, Δt_2";
        public const string Evac_delta_t3 = "Evacuation, Δt_3";

        // Dichtheitsprüfung trocken
        public const string LeakTestDry_PiA60_111 = "Leak test dry, PiA60.111 pressure setpoint";
        public const string LeakTestDry_PiAx_1_3 = "Leak test dry, PiAx.1.3 pressure setpoint ";
        public const string LeakTestDry_WaitTime = "Leak test dry, waiting time";

        // Batterie befüllen
        public const string FillN2_Pressure_PiAx_1_3 = "Filling, PiAx.1.3 pressure setpoint ";
        public const string FillN2_MinFillTime = "Filling, min. filling time";
        public const string Fill_Afterpress_PiAx_1_3 = "Filling, PiAx.1.3 afterpressure setpoint";
    }



    internal static class ParamNamePressure
    {
        public const string lrPiA_X1        = "X1:  Maximaldruck Vakuumleitung";
	    public const string lrPiA_X2		= "X2:  Zieldruck Vakuumprüfung";
        public const string lrPiA_X3		= "X3:  Maximaler Solldruck Vakuumprüfung";
        public const string lrPiA_X4		= "X4:  Gegendruck Wastetank";
        public const string lrPiA_X5		= "X5:  Gegendruck Puffertank Elektrolyt 1";
        public const string lrPiA_X6		= "X6:  Gegendruck Puffertank Elektrolyt 2";
        public const string lrPiA_X7		= "X7:  Gegendruck Puffertank SO2f";
        public const string lrPiA_X8		= "X8:  Druck Verpressen 1";
        public const string lrPiA_X9		= "X9:  Druckdifferenz beim Umschalten von Verpressen 1";
        public const string lrPiA_X10		= "X10: Maximaler Solldruck Vakuumprüfung Zellen trocken";
        public const string lrPiA_X11		= "X11: Maximaler Solldruck Vakuumprüfung verschlossene Röhrchen";
        public const string lrPiA_X12		= "X12: Zieldruck Evakuierung verschlossene Röhrchen";
        public const string lrPiA_X13		= "X13: Druck für Dosierung SO2 Gas";
        public const string lrPiA_X14		= "X14: Zieldruck Befüllung Zellen mit N2";
        public const string lrPiA_X15		= "X15: Druck Abschneiden";
    }


    internal static class ParamNameTime
    {                                      
        public const string lrTime_Z1  = "Z1:  Dauer Dosierbehälter über Min-Level befüllen";
        public const string lrTime_Z2  = "Z2:  Maximale Zeit Erzeugung Vorvakuum";
        public const string lrTime_Z3  = "Z3:  Dauer Dichtheitsprüfung Anlage";
        public const string lrTime_Z4  = "Z4:  Zeit füR das Erreichen des Drucks 'X2' füR das nasse Evakuieren nach dem Verpressen der Röhrchen";
        public const string lrTime_Z5  = "Z5:  Timeout f. erreichen Lx.1.1+Lx.1.2 beim Erstbefüllen";
        public const string lrTime_Z6  = "Z6:  Timeout f. entleeren Dosierbehälter auf Referenzstand";
        public const string lrTime_Z7  = "Z7:  Zeit für aufheizen aller Heizungen";
        public const string lrTime_Z8  = "Z8:  Zeit füR Spülen mit SO2fl nach kleiner Wartung";
        public const string lrTime_Z9  = "Z9:  Zeit füR Spülen mit N2 nach kleiner Wartung";
        public const string lrTime_Z10 = "Z10: Zeit füR Belüften Dosierbehälter nach Vakuum";
        public const string lrTime_Z11 = "Z11: Zeit bis Druck Verpressen 1 'X8' erreicht sein muss";
        public const string lrTime_Z12 = "Z12: Zeit innerhalb der die Endlagensensoren beim Andocken erwartet werden";
        public const string lrTime_Z13 = "Z13: max. Zeit in der der Solldruck 'X10´' nicht überschritten werden darf";
        public const string lrTime_Z14 = "Z14: Zeit, in der der gemessene Druck gem. SPEC <= dem einstellbaren Druck 'X3' bleiben muss";
        public const string lrTime_Z15 = "Z15: Max. zul. Zeit für Dosierung Elektrolyt";
        public const string lrTime_Z16 = "Z16: Timer für Dosierung SO2 flüssig";
        public const string lrTime_Z17 = "Z17: Zeit für das Erreichen des Drucks 'X10' für das Dosieren von SO2 Gas";
        public const string lrTime_Z18 = "Z18: Zeit gem. SPEC, um SO2 in den Drain zu spülen";
        public const string lrTime_Z19 = "Z19: Zeit für Spülen auswählbar zwischen SO2 flüssig, SO2 gasförmig, N2";
        public const string lrTime_Z20 = "Z20: Zeit füR Leckagetest Röhrchen nach dem Befüllen";
        public const string lrTime_Z21 = "Z21: Maximale Zeit Erzeugung Trockenvakuum Zelle";
        public const string lrTime_Z22 = "Z22: Zeit gem. SPEC zum Evakuieren der verpressten Röhrchen";
        public const string lrTime_Z23 = "Z23: Zeit innerhalb der die Endlagensensoren beim Abdocken erwartet werden";
        public const string lrTime_Z24 = "Z24: Blockventile mit SO2 spülen";
        public const string lrTime_Z25 = "Z25: Max. zulässige Zeit in der der Zieldruck 'X14# beim Befüllen mit N2 erreicht werden muss";
        public const string lrTime_Z26 = "Z26: Zeit für 'Entlüften' N2 bei Befüllprozedur mit N2";
        public const string lrTime_Z27 = "Z27: Maximale Zeit für das Befüllen der Dosierbehälter";
        public const string lrTime_Z28 = "Z28: Zeit füR das komplette Entleeren der Dosierbehälter";
        public const string lrTime_Z29 = "Z29: Zeit füR das Be- und Entlüften der Dosierbehälter beim kompletten Entleeren und Belüften Querleitung N2";
        public const string lrTime_Z30 = "Z30: Zeit Spülen Dosiertanks Elektrolyt mit SO2fl";
        public const string lrTime_Z31 = "Z31: Zeit um Dosierbehälter Elektrolyt mit N2 leer zudrücken";
        public const string lrTime_Z32 = "Z32: Timer für das Warten bis alle Drucksensoren der Anlage beim Fluten mit N2 ≥ 2,5 bar erreichen";
        public const string lrTime_Z33 = "Z33: Zeit in der ein Druck von 3,5 bar in den Puffertanks nach fluten mit N2 (2,5 bar) erwartet wird";
        public const string lrTime_Z34 = "Z34: Haltezeit Verpressen 2";
        public const string lrTime_Z35 = "Z35: Zeit bis Druck Abschneiden 'X15' erreicht sein muss";
        public const string lrTime_Z36 = "Z36: Haltezeit Druck 'X15' beim Abschneiden";
        public const string lrTime_Z37 = "Z37: Zeitraum bis zum Abwerfen des Röhrchenabschnitt nachdem Zylinder Z9.12.1 gestartet hat";
        public const string lrTime_Z38 = "Z38: Vor- und Nachlauf Argon beim Laserschweißen";
        public const string lrTime_Z39 = "Z39: Zeit Entlüften Puffertanks auf 3.5 bar";
        public const string lrTime_ZM  = "ZM:  Maximale Zeit fluten Medien";
    }


    internal static class ParamNameTemp
    {
        public const string lrTIC_TM = "Betriebstemperatur Medien";
    }
    internal static class ParamNameElse
    {
        public const string diNr_A1 = "Anzahl Widerstandsschweißungen nach Elektroden - Wechsel / Reinigung";
    }






    internal static class ParamNameTranspUnit

    {
        public const string AxisSetpointSt1  = "ESM8 and LFC Axis pos. - Station 1";
        public const string AxisSetpointSt2  = "ESM8 and LFC Axis pos. - Station 2";
        public const string AxisSetpointSt3  = "ESM8 and LFC Axis pos. - Station 3";
        public const string AxisSetpointSt4  = "ESM8 and LFC Axis pos. - Station 4";
        public const string AxisSetpointSt5  = "ESM8 and LFC Axis pos. - Station 5";
        public const string AxisOffsetCell   = "Offset 'Tube to tube' (Station 4 and 5, all cell type)";

        public const string AxisSetpointHome = "Axis pos. - Home";

        public const string AxisSetpointSt1_S = "HPC and HPCM Axis pos. - Station 1";
        public const string AxisSetpointSt2_S = "HPC and HPCM Axis pos. - Station 2";
        public const string AxisSetpointSt3_S = "HPC and HPCM Axis pos. - Station 3";
        public const string AxisSetpointSt4_S = "HPC and HPCM Axis pos. - Station 4";
        public const string AxisSetpointSt5_S = "HPC and HPCM Axis pos. - Station 5";


        public const string OpModeVelFast = "Operation mode, Velocity fast";
        public const string OpModeVelSlow = "Operation mode, Velocity slow";
        public const string OpModeAccFast = "Operation mode, Acceleration fast";
        public const string OpModeAccSlow = "Operation mode, Acceleration slow";
        public const string OpModeDecFast = "Operation mode, Deceleration fast";
        public const string OpModeDecSlow = "Operation mode, Deceleration slow";

        public const string ServModeVelJogFast = "Service mode, Velocity jogging fast";
        public const string ServModeVelJogSlow = "Service mode, Velocity jogging slow";
        public const string ServModeVelToPos = "Service mode, Velocity to position";
        public const string ServModeAcc = "Service mode, Acceleration";
        public const string ServModeDec = "Service mode, Deceleration";


    }


}
