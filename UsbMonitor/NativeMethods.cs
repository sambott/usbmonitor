using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UsbMonitor
{
    [Flags]
    internal enum ExitWindows
    {
        EWX_LOGOFF = 0,
        EWX_SHUTDOWN = 0x1,
        EWX_REBOOT = 0x2,
        EWX_FORCE = 0x4,
        EWX_POWEROFF = 0x8,
        EWX_FORCEIFHUNG = 0x10,
        EWX_RESTARTAPPS = 0x40
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int CancelShutdown();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ExitWindowsEx(ExitWindows uFlag, UInt32 dwReserved);

        internal static bool ExitWindowsEx(ExitWindows uFlag)
        {
            return ExitWindowsEx(uFlag, 0);
        }

    }
}
