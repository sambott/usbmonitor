using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;
using System.Globalization;

namespace UsbMonitor
{
    public partial class ShutterDowner : Form
    {
        private static int WM_DEVICECHANGE = 0x0219;
        private const int CP_NOCLOSE_BUTTON = 0x200;

        private Timer Countdown;
        private int Timeout;
        private string FormatText;
        
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                bool hasUSB = false;
                foreach (DriveInfo Drive in DriveInfo.GetDrives())
                {
                    if (Drive.DriveType == DriveType.Removable && !(Drive.Name == "A:\\" || Drive.Name == "B:\\"))
                    {
                        hasUSB = true;
                    }
                }
                if (!hasUSB)
                {
                    IssueLogoff();
                }
            }
            base.WndProc(ref m);
        }

        public ShutterDowner(int timeout, string formatText)
        {
            InitializeComponent();
            Timeout = timeout;
            FormatText = formatText;
            Countdown = new Timer();
            Countdown.Interval = 1000;
            Countdown.Tick += Countdown_Tick;
            Ticker();
        }

        private void ShutterDowner_Shown(object sender, EventArgs e)
        {
            this.TopMost = true;
            System.Media.SystemSounds.Exclamation.Play();
            Countdown.Start();
        }

        void Countdown_Tick(object sender, EventArgs e)
        {
            Ticker();
        }

        void Ticker()
        {
            if (Timeout > 0)
            {
                label2.Text = String.Format(CultureInfo.InvariantCulture, FormatText, Timeout);
                Timeout--;
            }
            else if (Timeout == 0)
            {
                IssueLogoff();
            }
        }

        private static bool IssueLogoff()
        {
            return NativeMethods.ExitWindowsEx(ExitWindows.EWX_LOGOFF);
        }
    }
}
