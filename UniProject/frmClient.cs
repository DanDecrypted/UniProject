using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            client = new ClientServer.Client();
            clientThread = new Thread(client.StartClient);
            clientThread.Start();
            
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            SendMessage("Lock");
        }

        private void SendMessage(string message)
        {
            textBox1.Text += message + " Sent to Server" + Environment.NewLine;
            byte[] bytes = new byte[1024];
            byte[] encodedMessage = Encoding.ASCII.GetBytes(message.ToString() + "<EOF>");
            int bytesSent = client.Socket.Send(encodedMessage);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client.Socket.Connected)
            {
                client.Socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            } 
            client.Socket.Close();
            Console.WriteLine("Client gracefully closed.");
        }
    }
}
