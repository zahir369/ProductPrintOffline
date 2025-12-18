using System;
using System.Runtime.InteropServices;

namespace MKSS.APP.SemiTester
{
    public class KeyBoardExe {


        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, ref INPUT pInputs, int cbSize);




        public const int INPUT_KEYBOARD = 1;

        public const int KEYEVENTF_KEYUP = 0x0002;


    }



    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT

    {

        [FieldOffset(0)]
        public Int32 type;

        [FieldOffset(4)]
        public KEYBDINPUT ki;

        [FieldOffset(4)]
        public MOUSEINPUT mi;

        [FieldOffset(4)]
        public HARDWAREINPUT hi;

    }



    [StructLayout(LayoutKind.Sequential)]

    public struct MOUSEINPUT

    {

        public Int32 dx;

        public Int32 dy;

        public Int32 mouseData;

        public Int32 dwFlags;

        public Int32 time;

        public IntPtr dwExtraInfo;

    }

    [StructLayout(LayoutKind.Sequential)]

    public struct KEYBDINPUT

    {

        public Int16 wVk;

        public Int16 wScan;

        public Int32 dwFlags;

        public Int32 time;

        public IntPtr dwExtraInfo;

    }

    [StructLayout(LayoutKind.Sequential)]

    public struct HARDWAREINPUT

    {

        public Int32 uMsg;

        public Int16 wParamL;

        public Int16 wParamH;

    }


}
