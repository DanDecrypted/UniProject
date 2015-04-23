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
            if (e.GetBytes().Length > 0)
            {
                if (e.ToString().Contains("Message="))
                {
                    string[] args = e.ToString().Split('=');
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i].Trim() == "CurrentUser")
                        {
                            ((ClientHandler)sender).CurrentUser = args[i + 1].Trim();
                            foreach (ctrlScreenViewer screenViewer in layoutPanel.Controls)
                            {
                                if (screenViewer.lblClientID.Text == ((ClientHandler)sender).Address.ToString())
                                    SafeUpdateLabel(screenViewer.lblCurrentUser, ((ClientHandler)sender).CurrentUser);
                            }
                        }
                    }
                    SafeUpdateLog(String.Format("Data Received: {0}", e.ToString()));
                }
                else
                {
                    MemoryStream ms = new MemoryStream(e.GetBytes());
                    Image imageFromStream = Image.FromStream(ms);
                    foreach (ctrlScreenViewer screenViewer in layoutPanel.Controls)
                    {
                        if (screenViewer.lblClientID.Text == ((ClientHandler)sender).Address.ToString())
                        {
                            if (screenViewer.OneToOneMode == false)
                            {
                                screenViewer.imgScreen.Image = imageFromStream;
                                screenViewer.imgScreen.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                screenViewer.OneToOneForm.ClientScreen.Image = imageFromStream;
                                screenViewer.OneToOneForm.ClientScreen.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event that fires when a client socket connection has been lost between the client and the server.
        /// </summary>
        /// <param name="sender">ClientHandler</param>
        /// <param name="e">IP of the client</param>
        void server_ClientDisconnected(object sender, CustomEventArgs e)
        {
            // check to see whether the form is closing (this.Disposing). if it's not, then carry on invoking the control 
            if (!this.Disposing)
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
        }

        /// <summary>
        /// Event that fires when a client socket connection has been established between the client and the server.
        /// </summary>
        /// <param name="sender">ClientHandler</param>
        /// <param name="e">IP of the client</param>
        void server_ClientConnected(object sender, CustomEventArgs e)
        {
            if (layoutPanel.InvokeRequired)
            {
                layoutPanel.Invoke(new Action<object, CustomEventArgs>(server_ClientConnected), sender, e);
            }
            else
            {
                ctrlScreenViewer screenViewer = new ctrlScreenViewer(e.ToString(), (ClientHandler)sender);
                layoutPanel.Controls.Add(screenViewer);
                SafeUpdateLog(String.Format("Client Connected: {0}", e.ToString()));
                ((ClientHandler)sender).Send("Info.CurrentUser");
            }
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

        /// <summary>
        /// Thread Safe call to update the log within the form. 
        /// </summary>
        /// <param name="text">Text to update the log with</param>
        /// <param name="clearLog">Do you want the log to be cleared?</param>
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

        /// <summary>
        /// Thread Safe call to update a label's text.
        /// </summary>
        /// <param name="label">label object to modify</param>
        /// <param name="text">text to give the label</param>
        private void SafeUpdateLabel(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action<Label, string>(SafeUpdateLabel), label, text);
            }
            else
            {
                label.Text = text;
            }
        }

        private void btnShareWithAll_Click(object sender, EventArgs e)
        {
            foreach (ClientHandler client in this.server.Clients)
            {
                client.Send("WinAPI.ShowTeacherScreen");    
            }
        }
    }
}
