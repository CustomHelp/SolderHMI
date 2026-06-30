using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOET_HMI
{
    public enum LEDColors
    {
        Green = 1,
        Red = 2,
        Yellow = 3,
        Blue = 4,
        CHP = 5,
    }

    public enum eOrderStartMode
    {
        NoSelection     = 0,
        Automatically   = 1, // Wenn der aktuelle Auftrag zu Ende ist und die SPS bereit ist, wird gleich der nächste Auftrag von der Warteschlange in die SPS geladen und gestarte
        Manually        = 2, // Wenn der aktuelle Auftrag zu Ende ist, muss der nächste immer per Hand gestartet werden
    }


    public enum eOrderArchivStates
    {
        OA_00_NoState     = 0,
        OA_10_Sent        = 10,
        OA_20_Started     = 20,
        OA_30_Finished    = 30,
        OA_90_Cancelled   = 90,

    }


    public enum ParamSetTypes
    {
        Type = 0,
        Machine = 1,
    }


    public enum ParamTypes
    {
        BOOL = 1,
        INT = 2,
        DINT = 3,
        REAL = 4,
        LREAL = 5,
        STRING = 6,

        ST_Setting_BOOL = 51,
        ST_Setting_DINT = 52,
        ST_Setting_REAL = 53,
        ST_Setting_LREAL = 54,
    }

    public enum TwinCATTypes
    {
        DT_000_NoType   = 000,
        DT_010_BOOL     = 010,
        DT_020_BYTE     = 020,
        DT_030_WORD     = 030,
        DT_040_DWORD    = 040,
        DT_050_SINT     = 050,
        DT_060_USINT    = 060,
        DT_070_INT      = 070,
        DT_080_UINT     = 080,
        DT_090_DINT     = 090,
        DT_100_UDINT    = 100,
        //DT_110_LINT     = 110,    (64 bit integer, currently not supported by TwinCAT)
        //DT_120_ULINT    = 120,    (Unsigned 64 bit integer, currently not supported by TwinCAT)
        DT_130_REAL     = 130,
        DT_140_LREAL    = 140,
        DT_150_STRING   = 150,
    }

    /// <summary>
    /// TwinCAT DataTyp-Länge in Bytes
    /// </summary>
    internal static class TwinCATTypeLengths
    {
        public static int BOOL    = 1;
        public static int BYTE    = 1;
        public static int WORD    = 2;
        public static int DWORD   = 4;
        public static int SINT    = 1;
        public static int USINT   = 1;
        public static int INT     = 2;
        public static int UINT    = 2;
        public static int DINT    = 4;
        public static int UDINT   = 4;
        public static int REAL    = 4;
        public static int LREAL   = 8;
        //public static int STRING  = 0;
    }


    public enum eStationsHMI
    {
        _NoID                                  = 0,

        // Modul 1
        M1_01_Station1                        = 101,
        M1_02_Station2                        = 102,
        M1_03_Global                          = 103,
    }


}
