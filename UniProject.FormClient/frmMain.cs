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
        private bool Initialised = false;

        //Create a new bitmap.
        static Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
        public frmMain()
        {
            if (WinAPI.IsProcessRunning("UniProject.FormClient"))
            {
                Application.Exit();
            }
            else
            {
                InitializeComponent();
                m_Client = new Client("10.248.0.27", 101);
                m_ShouldWork = true;
                m_ScreenWorkerThread = new Thread(ScreenFeed);
                m_Client.ClientConnected += client_ClientConnected;
                m_Client.DataReceived += client_DataReceived;
                m_Client.ServerConnectionDropped += m_Client_ServerConnectionDropped;
                Initialised = true;
            }
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
                    WinAPI.LockWorkStation();
                }
                else if (args[1] == "Shutdown")
                {
                    WinAPI.Shutdown();
                }
                else if (args[1] == "StartProcess")
                {
                    string temp = message.Replace(args[0], "").Replace(args[1], "").Replace("..", "");
                    string[] urlArgs = temp.Split('|');
                    WinAPI.StartProcess(urlArgs[0], urlArgs[1]);
                }
            }
            else if (args[0] == "SoftAPI")
            {
                if (args[1] == "Lock")
                {
                    // Hacked in because creating a new form on this same thread locked the application ?
                    Application.Run(new frmFullScreen(Properties.Resources.LockedScreen));
                    // TODO lock keyboard and mouse input
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
            while (!Initialised) ;
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
                        try
                        {
                            g.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                        }
                        catch (Exception ex)
                        {
                            m_Client.Send("Error when sending picture from " + m_Client.LocalIP + ": " + ex.Message.ToString());
                        }
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

        private void safeShowForm(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action<Form>(safeShowForm), form);
            }
            else
            {
                form.Show();
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
