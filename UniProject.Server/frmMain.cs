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
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverThread.IsAlive)
            {
                serverThread.Interrupt();
                Console.WriteLine("Server Thread terminated");
            }
        }
    }
}
