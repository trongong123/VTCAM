using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace FrontCameraAssembleEquipment.Helpers
{
    public static class WindowStateHelper
    {
        [DllImport("shell32.dll")]
        static extern int SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;
        }

        const int ABS_AUTOHIDE = 1;
        const int ABS_ALWAYSONTOP = 2;
        const uint ABM_SETSTATE = 0x0000000a;

        public static void HideTaskbar()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.lParam = ABS_AUTOHIDE;
            SHAppBarMessage(ABM_SETSTATE, ref abd);
        }

        public static void ShowTaskbar()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.lParam = ABS_ALWAYSONTOP;
            SHAppBarMessage(ABM_SETSTATE, ref abd);
        }
        const uint SWP_SHOWWINDOW = 0x0040;
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_ACTIVATE = 0x0006;

        private const int SC_RESTORE = 0xF120;
        private const int WA_CLICKACTIVE = 2;

        private static Action? _taskbarClickCallback;

        public static void RegisterTaskbarClick(Window window, Action callback)
        {
            _taskbarClickCallback = callback;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            var source = HwndSource.FromHwnd(hwnd);

            source.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SYSCOMMAND:
                    if ((int)wParam == SC_RESTORE)
                    {
                        _taskbarClickCallback?.Invoke();
                    }
                    break;

                case WM_ACTIVATE:
                    if ((int)wParam == WA_CLICKACTIVE)
                    {
                        _taskbarClickCallback?.Invoke();
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    }
}
