using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;
using System.Threading;

namespace UniProject.Client
{
    public partial class frmClient : Form
    {
        private ClientServer.Client client;
        private Thread clientThread;
        public frmClient()
        {
            InitializeComponent();
            client = new ClientServer.Client("169.254.50.104", 80);
            client.InitializeSocket();
            client.ErrorEvent += client_ClientError;
            client.ConnectedEvent += client_ClientConnected;
            client.DataSentEvent += client_DataSentEvent;
        }

        void client_DataSentEvent(CustomEventArgs.BaseCustomEventArgs e)
        {
            SafeTextboxUpdate("Data Sent: " + e.Data.ToString());
        }

        void client_ClientError(CustomEventArgs.BaseCustomEventArgs e)
        {
            MessageBox.Show(e.Data.ToString(), "Error");
        }

        void client_ClientConnected(System.Net.Sockets.Socket socket, CustomEventArgs.SocketConnectionEventArgs e)
        {
            SafeTextboxUpdate("Client connected to: " + e.ToString());
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            SendMessage("Lock");
        }

        private void SendMessage(string message)
        {
            client.Send(message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void SafeTextboxUpdate(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action<string>(SafeTextboxUpdate), text);
                return;
            }

            textBox1.Text += text + Environment.NewLine;
        }
    }
}
