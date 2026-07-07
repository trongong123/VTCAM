using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.Helpers
{
    public static class DisplayHelpers
    {
        public static IList<MonitorInfo> GetValidMonitors()
        {
            var result = new List<MonitorInfo>();
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdc, ref RECT rect, IntPtr data) =>
                {
                    result.Add(new MonitorInfo(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, hMonitor.ToString()));
                    return true;
                }, IntPtr.Zero);
            return result;
        }

        public record MonitorInfo(int Left, int Top, int Width, int Height, string Infor);

        private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdc, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
