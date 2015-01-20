using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.Server
{
    public partial class frmMain : Form
    {
        private ClientServer.Server server;
        private Thread serverThread;
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            server = new ClientServer.Server();
            serverThread = new Thread(server.StartListening);
            serverThread.Start();
            Console.WriteLine("Initialising Server Thread...");
            while (!serverThread.IsAlive) ;

            textBox1.Text += "Server Started on: " + server.Host.ToString() + ":" + server.Port.ToString() + " in a worker thread." + Environment.NewLine;
            server.ClientConnected += server_ClientConnected;
            server.DataReceived += server_DataReceived;
        }

        void server_DataReceived(System.Net.Sockets.Socket client, CustomEventArgs.DataReceivedEventArgs e)
        {
            this.textBox1.Text += "Data Received: " + e.Data.ToString() + Environment.NewLine;
            if (e.Data.ToString() == "Lock")
            {
                WinAPI.LockWorkStation();
            }
        }

        void server_ClientConnected(System.Net.Sockets.Socket client, CustomEventArgs.SocketConnectedEventArgs e)
        {
            this.textBox1.Text += "Client Connected:" + e.ToString() +Environment.NewLine;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            while (serverThread.IsAlive)
            {
                serverThread.Interrupt();
                serverThread.Join();
            }
            Console.WriteLine("Server Thread terminated");
            Application.Exit();
        }
    }
}
