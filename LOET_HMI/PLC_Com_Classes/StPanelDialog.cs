using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace LOET_HMI
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public class ST_PopUp
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bShow;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string strBtnName_1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string strBtnName_2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string strBtnName_3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string strBtnName_4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string strBtnName_5;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string strDialogHead;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDialogText1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDialogText2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDialogText3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDialogText4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 201)]
        public string strDialogText5;
        [MarshalAs(UnmanagedType.I1)]

        public bool bTextInputEnable;
        [MarshalAs(UnmanagedType.I2)]
        public Int16 iCountTextInput;


        // **************************************************************
        // ************************* HMI-to-PLC *************************
        // **************************************************************
        // Hier nicht relevant

        // [MarshalAs(UnmanagedType.I2)]
        // public Int16 iAnswer;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput1;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput2;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput3;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput4;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput5;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput6;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput7;
        // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        // public string strTextInput8;

    }
}