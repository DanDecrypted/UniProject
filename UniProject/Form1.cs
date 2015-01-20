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
    public partial class Form1 : Form
    {
        private ClientServer.Client client;
        private Thread clientThread;
        public Form1()
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
            byte[] bytes = new byte[1024];
            byte[] encodedMessage = Encoding.ASCII.GetBytes(message.ToString() + "<EOF>");
            int bytesSent = client.Socket.Send(encodedMessage);
            int bytesRec = client.Socket.Receive(bytes);
            textBox1.Text += String.Format("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
            client.Socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            client.Socket.Close();
        }
    }
}
