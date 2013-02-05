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
    public partial class ListeningForm : Form
    {
        private static int WM_QUERYENDSESSION = 0x0011;
        private static bool systemShutdown = false;
        private static bool letItThroughNextTime = false;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {

            if (m.Msg == WM_QUERYENDSESSION)
            {
                systemShutdown = true;
                if (!letItThroughNextTime) m.Result = (IntPtr)0;
            }
            base.WndProc(ref m);
        }

        private void ListenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (systemShutdown)
            {
                systemShutdown = false;
                bool hasUSB = false;

                foreach (DriveInfo Drive in DriveInfo.GetDrives())
                {
                    if (Drive.DriveType == DriveType.Removable && !(Drive.Name == "A:\\" || Drive.Name == "B:\\"))
                    {
                        hasUSB = true;
                    }
                }

                if (hasUSB&&(!letItThroughNextTime))
                {
                    e.Cancel = true;
                    letItThroughNextTime = true;
                    label2.Text = "Caught WM_QUERYENDSESSION, Cancelled with: " + NativeMethods.CancelShutdown().ToString(CultureInfo.InvariantCulture);
                    ShutterDowner SD = new ShutterDowner(20, "Logging Off in {0} Seconds");
                    SD.Show();
                }
                else
                {
                    e.Cancel = false;
                }
            }
        }

        public ListeningForm()
        {
            InitializeComponent();
            this.Shown += ListenForm_Shown;
            this.FormClosing += new FormClosingEventHandler(ListenForm_FormClosing);
        }

        void ListenForm_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
