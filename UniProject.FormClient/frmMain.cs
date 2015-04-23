using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.FormClient
{
    public partial class frmMain : Form
    {
        private Client m_Client;
        private Thread m_ScreenWorkerThread;
        private volatile bool m_ShouldWork;
        private bool pauseSending = false;
        //Create a new bitmap.
        static Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
        public frmMain()
        {
            InitializeComponent();
            m_Client = new Client("127.0.0.1", 101);
            m_ShouldWork = true;
            m_ScreenWorkerThread = new Thread(ScreenFeed);
            m_Client.ClientConnected += client_ClientConnected;
            m_Client.DataReceived += client_DataReceived;
            m_Client.ServerConnectionDropped += m_Client_ServerConnectionDropped;
        }

        void m_Client_ServerConnectionDropped(CustomEventArgs e)
        {
            notifyIcon.ShowBalloonTip(1000, "Server Connection Dropped", e.ToString(), ToolTipIcon.Error);
            pauseSending = true;
        }

        void client_ClientConnected(CustomEventArgs e)
        {
            notifyIcon.ShowBalloonTip(1000, "Client Connection Established", e.ToString(), ToolTipIcon.Info);
            pauseSending = false;
        }

        void client_DataReceived(CustomEventArgs e)
        {
            string message = e.ToString();
            string[] args = message.Split('.');
            if (args[0] == "WinAPI")
            {
                if (args[1] == "Lock")
                {
                    frmFullScreen imageForm = new frmFullScreen();

                    WinAPI.LockWorkStation();
                }
            }
            else if (args[0] == "Info")
            {
                if (args[1] == "CurrentUser")
                    this.m_Client.Send("Message=CurrentUser=" + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Hide();
            m_Client.Start();
            m_ScreenWorkerThread.Start();
            notifyIcon.ShowBalloonTip(1000, "Client Started...", String.Format("Server: {0}", m_Client.ServerAddress), ToolTipIcon.Info);
        }

        private void ScreenFeed()
        {
            MemoryStream ms = new MemoryStream();
            while (m_ShouldWork)
            {
                if (!pauseSending)
                {
                    // Screen Feed Code
                    bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    using (Graphics g = Graphics.FromImage(bmpScreenshot))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                    }
                    bmpScreenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    m_Client.Send(ms.ToArray());
                    Thread.Sleep(1000);
                }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_ScreenWorkerThread.IsAlive)
            {
                m_ShouldWork = false;
                Thread.Sleep(1000);
                m_Client.Socket.Close();
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            notifyIconContextMenu.Show(Control.MousePosition);
        }

        private void mnuExitClient_Click(object sender, EventArgs e)
        {
            if (m_ScreenWorkerThread.IsAlive)
            {
                m_ShouldWork = false;
                m_ScreenWorkerThread.Join();
                m_Client.Stop();
                this.Close();
                Application.Exit();

            }
        }
    }
}
