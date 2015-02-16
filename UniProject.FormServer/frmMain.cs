using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.FormServer
{
    public partial class frmMain : Form
    {
        Server server;
        public frmMain()
        {
            InitializeComponent();
            server = new Server(IPAddress.Any, 101);
            server.ClientConnected += server_ClientConnected;
            server.ClientDisconnected += server_ClientDisconnected;
            server.DataReceived += server_DataReceived;
            server.DataSent += server_DataSent;
        }

        void server_DataSent(object sender, CustomEventArgs e)
        {
            SafeUpdateLog(String.Format("Data Sent: {0}", e.ToString()));
        }

        void server_DataReceived(object sender, CustomEventArgs e)
        {
            try
            {
                MemoryStream ms = new MemoryStream(e.GetBytes());
                Image imageFromStream = Image.FromStream(ms);
                foreach (ctrlScreenViewer screenViewer in layoutPanel.Controls)
                {
                    if (screenViewer.lblClientID.Text == ((ClientHandler)sender).Address.ToString())
                    {
                        screenViewer.imgScreen.Image = imageFromStream;
                        screenViewer.imgScreen.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
            }
            catch
            {
                SafeUpdateLog(String.Format("Data Received: {0}", e.ToString()));
            }
        }

        void server_ClientDisconnected(object sender, CustomEventArgs e)
        {
            if (layoutPanel.InvokeRequired)
            {
                layoutPanel.Invoke(new Action<object, CustomEventArgs>(server_ClientDisconnected), sender, e);
            }
            else
            {
                foreach (ctrlScreenViewer screenViewer in layoutPanel.Controls)
                {
                    if (screenViewer.lblClientID.Text == e.ToString())
                    {
                        layoutPanel.Controls.Remove(screenViewer);
                    }
                }
                SafeUpdateLog(String.Format("Client Disconnected: {0}", e.ToString()));
            }
        }

        void server_ClientConnected(object sender, CustomEventArgs e)
        {
            if (layoutPanel.InvokeRequired)
            {
                layoutPanel.Invoke(new Action<object, CustomEventArgs>(server_ClientConnected), sender, e);
            }
            else
            {
                ctrlScreenViewer screenViewer = new ctrlScreenViewer(e.ToString());
                screenViewer.Click += screenViewer_Click;
                layoutPanel.Controls.Add(screenViewer);
                SafeUpdateLog(String.Format("Client Connected: {0}", e.ToString()));
            }
        }

        void screenViewer_Click(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            server.Start();
            this.layoutPanel.Controls.Clear();
        }

        private void btnLockAll_Click(object sender, EventArgs e)
        {
            foreach(ClientHandler client in server.Clients)
            {
                client.Send("WinAPI.Lock");
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Stop();
        }

        private void SafeUpdateLog(string text, bool clearLog = false)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string, bool>(SafeUpdateLog), text, clearLog);
            }
            else
            {
                if (clearLog)
                    txtLog.Text = "";
                else
                    txtLog.Text += text + Environment.NewLine;
            }
        }
    }
}
