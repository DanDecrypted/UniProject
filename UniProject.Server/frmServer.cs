using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.Server
{
    public partial class frmServer : Form
    {
        private ClientServer.Server server;
        private Thread serverThread;
        private bool initialized = false;
        public frmServer()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            server = new ClientServer.Server("127.0.0.1", 80, 100);
            serverThread = new Thread(server.StartListening);
            serverThread.Start();
            Console.WriteLine("Initialising Server Thread...");
            while (!serverThread.IsAlive) ;

            textBox1.Text += "Server Started on: " + server.Host.ToString() + ":" + server.Port.ToString() + " in a worker thread." + Environment.NewLine;
            server.ClientConnected += server_ClientConnected;
            server.ClientDisconnected += server_ClientDisconnected;
            server.DataReceived += server_DataReceived;
            initialized = true;
        }

        void server_ClientDisconnected(Socket client, CustomEventArgs.SocketConnectionEventArgs e)
        {
            SafeTextboxUpdate("Client Disconnected: " + e.ToString()); 
        }

        void server_DataReceived(Socket client, CustomEventArgs.DataReceivedEventArgs e)
        {
            SafeTextboxUpdate("Data Received: " + e.Data.ToString());
        }

        void server_ClientConnected(Socket client, CustomEventArgs.SocketConnectionEventArgs e)
        {
            SafeTextboxUpdate("Client Connected: " + client.RemoteEndPoint.ToString());
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (initialized)
            {
                server.StopListening();
                serverThread.Abort();
                Console.WriteLine("Server Thread terminated");
            }
            else
            {
                this.BeginInvoke(new MethodInvoker(Close));
            }
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
