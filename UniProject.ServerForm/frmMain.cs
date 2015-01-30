using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.ServerForm
{
    public partial class frmMain : Form
    {
        UniProject.Core.ClientServer.Server server;
        delegate void UpdateImageCallback(Core.ClientServer.Server.ClientHandler client, Core.CustomEventArgs.DataEventArgs e);
        delegate void UpdateLogTextCallback(string text);
        MemoryStream ms;
        Image imageFromStream;
        public frmMain()
        {
            InitializeComponent();
            server = new Core.ClientServer.Server();
            server.ClientConnectedEvent += server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += server_ClientDisconnectedEvent;
            server.DataReceivedEvent += server_DataReceivedEvent;
        }

        void server_DataReceivedEvent(Core.ClientServer.Server.ClientHandler client, Core.CustomEventArgs.DataEventArgs e)
        {
            try
            {
                ms = new MemoryStream(e.Data);
                imageFromStream = Image.FromStream(ms);
                foreach (ucClientViewer clientViewer in tableLayoutPanel1.Controls)
                {
                    if (clientViewer.ClientID == client.Name)
                    {
                        clientViewer.Image = imageFromStream;
                    }
                }
                e.Data = new byte[] { };
            }
            catch
            {
                try
                {
                    string dataString = ASCIIEncoding.ASCII.GetString(e.Data);
                    string[] args = dataString.Split('=');
                    if (args[0] == "Name")
                    {
                        client.Name = args[1];
                    }
                }
                catch (Exception ex)
                {
                    UpdateLog("Error: " + ex.ToString());
                }
            }
        }

        void server_ClientDisconnectedEvent(Core.ClientServer.Server.ClientHandler client)
        {
            UpdateLog("Client Disconnected. " + String.Format("Clients connected {0} / {1}", server.Clients.Count, server.MaxConnections));
        }

        void server_ClientConnectedEvent(Core.ClientServer.Server.ClientHandler client)
        {
            ucClientViewer clientViewer = new ucClientViewer(client.Name);
            if (tableLayoutPanel1.InvokeRequired)
            {
                tableLayoutPanel1.Invoke(new Action<Core.ClientServer.Server.ClientHandler>(server_ClientConnectedEvent), client);
            }
            else
            {
                tableLayoutPanel1.Controls.Add(clientViewer);
                UpdateLog("Client Connected. " + String.Format("Clients connected {0} / {1}", server.Clients.Count, server.MaxConnections));
            }
        }

        void UpdateLog(string text)
        {
            if (this.txtServerLog.InvokeRequired)
            {
                this.txtServerLog.Invoke(new UpdateLogTextCallback(UpdateLog), new object[] { text });
            }
            else
            {
                this.txtServerLog.Text += text + Environment.NewLine;
            }
        }
    }
}
