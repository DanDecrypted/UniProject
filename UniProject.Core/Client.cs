using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class Client
    {
        private int m_Port;
        private string m_Host;
        private IPEndPoint m_RemoteEndP;
        private IPAddress m_IP;
        private Thread m_ReceiveWorker;
        private Socket m_Socket;
        private volatile bool m_ShouldWork;

        public Socket Socket
        {
            get { return m_Socket; }
        }

        public string LocalIP
        {
            get
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(); //http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp
            }
        }

        public string ServerAddress
        {
            get { return m_Host + ":" + m_Port.ToString(); }
        }

        public delegate void ConnectedHandler(CustomEventArgs e);
        public delegate void DataSentHandler(CustomEventArgs e);
        public delegate void DataReceivedHandler(CustomEventArgs e);

        public event ConnectedHandler ClientConnected;
        public event DataSentHandler DataSent;
        public event DataReceivedHandler DataReceived;
        public Client(string addr, int port = 100)
        {
            this.m_Host = addr;
            this.m_Port = port;
            this.m_ShouldWork = true;
            this.m_ReceiveWorker = new Thread(Main);
        }

        public void InitializeSocket()
        {
            try
            {
                // Establish the remote endpoint for the socket.
                m_IP = IPAddress.Parse(this.m_Host);
                m_RemoteEndP = new IPEndPoint(m_IP, this.m_Port);

                // Create a TCP/IP  socket if it hasn't already been created
                if (m_Socket == null)
                    m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Attempt to make a connection
                m_Socket.Connect(m_RemoteEndP);

                // Give the socket time to initialize 
                Thread.Sleep(10);

                // Start Receiving on socket connection
                if (!this.m_ReceiveWorker.IsAlive)
                    this.m_ReceiveWorker.Start();

                // Flag out to tell the client that the socket has been created
                if (ClientConnected != null)
                    ClientConnected(new CustomEventArgs(this.ServerAddress));
            }
            catch
            {
                Console.WriteLine("A Connection to the remote host could not be established... Retrying in 10 seconds.");
                Thread.Sleep(10000);
                InitializeSocket();
            }
        }

        public void Start()
        {
            this.InitializeSocket();
        }

        public void Stop()
        {
            this.m_ShouldWork = false;
            this.m_Socket.Close();
        }

        public void Send(string data)
        {
            Send(ASCIIEncoding.ASCII.GetBytes(data));
        }

        public void Send(byte[] data)
        {
            int dataTotal = 0;
            int dataSize = data.Length;
            int dataLeft = dataSize;
            int dataSent;
            byte[] dataBuffer = new byte[4];
            dataBuffer = BitConverter.GetBytes(dataSize);
            int bytesSent = this.m_Socket.Send(dataBuffer);

            while (dataTotal < dataSize)
            {
                dataSent = this.m_Socket.Send(data, dataTotal, dataLeft, SocketFlags.None);
                dataTotal += dataSent;
                dataLeft -= dataSent;
            }

            if (DataSent != null)
                DataSent(new CustomEventArgs(data));
        }

        private void Main()
        {
            while (m_ShouldWork)
            {
                try
                {
                    int dataTotal = 0;
                    int dataReceived;
                    byte[] packetSize = new byte[4];
                    dataReceived = m_Socket.Receive(packetSize, 0, 4, 0);
                    int dataBuffer = BitConverter.ToInt32(packetSize, 0);
                    int dataLeft = dataBuffer;
                    byte[] data = new byte[dataBuffer];
                    while (dataTotal < dataBuffer)
                    {
                        dataReceived = m_Socket.Receive(data, dataTotal, dataLeft, 0);
                        if (dataReceived == 0)
                            break;
                        dataTotal += dataReceived;
                        dataLeft -= dataReceived;
                    }

                    if (DataReceived != null)
                        DataReceived(new CustomEventArgs(data));
                }
                catch
                {
                    // Connection Dropped
                    lock (this.m_Socket)
                    {
                        this.m_Socket.Close();
                        this.m_Socket = null;
                    }
                    InitializeSocket();
                }
            }
        }
    }
}
