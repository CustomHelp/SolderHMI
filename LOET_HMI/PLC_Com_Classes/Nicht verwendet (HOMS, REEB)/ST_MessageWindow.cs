using System;
using System.Runtime.InteropServices;

namespace LOET_HMI
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    // Info Balog 29.7.2020:
    //      Ursprünglicher Name:            ST_MessageWindow
    //      Neuer Name vom Projekt HOMS:    StMessageWindow
    //      Name danach                     ST_MessagesModul (MBA 10.8.20209
    public class ST_MessagesModul
    {
        [MarshalAs(UnmanagedType.I2)]
        public Int16 iMessageCount = 0;

        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)] // auskommentiert : MBA 10.8.2020
        //public string sName = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)] // MBA 13.8.2020: erhöht auf 141 von 101
        public string sMessage1 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage2 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage3 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage4 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage5 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage6 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage7 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage8 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage9 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage10 = "";


        // MBA 23.7.2020: für Rena haben die ursprünglichen 10 Nachrichten nicht ausgereicht
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage11 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage12 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage13 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage14 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage15 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage16 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage17 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage18 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage19 = "";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 141)]
        public string sMessage20 = "";

        //MBA 10.8.2020: KLAUS:  evt. sollten hier 30 Komponenten stehen.... (???)

    }

}
