using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UniProject.Core.ClientServer
{
    /// <summary>
    /// Synchronous Server. Taken from MSDN. Source http://msdn.microsoft.com/en-us/library/6y0e13d3%28v=vs.110%29.aspx
    /// This class was taken and edited by me, adding a class constructor, and custom event calls were added for the purpose of multi-treading
    /// </summary>
    public class Server
    {
        private int m_Port;
        private int m_MaxConnections;
        private string m_Host;
        private Thread m_ListenerThread; 
        private bool m_Listen = true;

        // Incoming data from the client.
        public Socket Socket;
        public Socket Listener;
        public List<ClientHandler> Clients;

        public delegate void ClientConnectedHandler(ClientHandler client);
        public delegate void ClientDisconnectedHandler(ClientHandler client);
        public delegate void DataReceivedHandler(ClientHandler client, CustomEventArgs.DataEventArgs e);
        public delegate void DataSentHandler(ClientHandler client, CustomEventArgs.DataEventArgs e);

        public event DataReceivedHandler DataReceivedEvent;
        public event DataSentHandler DataSentEvent;
        public event ClientConnectedHandler ClientConnectedEvent;
        public event ClientDisconnectedHandler ClientDisconnectedEvent;
        
        public Thread ListenerThread
        {
            get { return m_ListenerThread; }
        }

        public string Host
        {
            get
            {
                return m_Host;
            }
        }
        public int Port
        {
            get
            {
                return m_Port;
            }
        }

        public int MaxConnections
        {
            get
            {
                return m_MaxConnections;
            }
        }

        public bool IsAlive
        {
            get { return m_ListenerThread.IsAlive; }
        }


        public Server(string host = "127.0.0.1", int port = 100, int maxConnections = 100)
        {
            this.m_Port = port;
            this.m_Host = host;
            this.m_MaxConnections = maxConnections;
            this.Clients = new List<ClientHandler>();
            //m_ClientCheckThread = new Thread(CheckClients);
            m_ListenerThread = new Thread(StartListening);
            //m_ClientCheckThread.Start();
            m_ListenerThread.Start();
        }

        public void StartListening()
        {
            // Establish the local endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse(this.m_Host);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.m_Port);

            // Create a TCP/IP socket.67
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            Listener.Bind(localEndPoint);
            Listener.Listen(this.m_MaxConnections);

            // Start listening for connections.
            while (m_Listen)
            {
                Socket Socket = Listener.Accept();
                ClientHandler clientHandler = new ClientHandler(Socket, this.m_Host, this.m_Port, "Client");

                clientHandler.Start();
                clientHandler.DataReceivedEvent += clientHandler_DataReceivedEvent;
                clientHandler.DataSentEvent += clientHandler_DataSentEvent;
                clientHandler.ClientDisconnectedEvent += clientHandler_ClientDisconnectedEvent;
                
                this.Clients.Add(clientHandler);

                if (ClientConnectedEvent != null)
                    ClientConnectedEvent(clientHandler);
            }
        }

        private void clientHandler_DataSentEvent(Server.ClientHandler client, CustomEventArgs.DataEventArgs e)
        {
            if (this.DataSentEvent != null)
                this.DataSentEvent(client, e);
        }

        private void clientHandler_ClientDisconnectedEvent(Server.ClientHandler client)
        {

            this.Clients.Remove(client);
            if (this.ClientDisconnectedEvent != null)
                this.ClientDisconnectedEvent(client);
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopListening()
        {
            this.m_Listen = false;
            foreach (ClientHandler client in this.Clients)
            {
                client.Stop();
            }
            this.Listener.Shutdown(SocketShutdown.Both);
            this.Listener.Close();
        }

        private void clientHandler_DataReceivedEvent(ClientHandler client, CustomEventArgs.DataEventArgs e)
        {
            if (this.DataReceivedEvent != null)
                this.DataReceivedEvent(client, e);
        }


        public class ClientHandler
        {
            private Socket m_Socket;
            private Thread m_WorkerThread;
            private byte[] m_Data;
            private string m_Host;
            private string m_Name;
            private int m_Port;
            private volatile bool m_ShouldStop = false;

            public delegate void DataReceivedHandler(ClientHandler client, CustomEventArgs.DataEventArgs e);
            public delegate void DataSentHandler(ClientHandler client, CustomEventArgs.DataEventArgs e);
            public delegate void ClientDisconnectedHandler(ClientHandler client);

            public event DataReceivedHandler DataReceivedEvent;
            public event DataSentHandler DataSentEvent;
            public event ClientDisconnectedHandler ClientDisconnectedEvent;

            public Socket Socket
            {
                get { return m_Socket; }
            }

            public string Name
            {
                get { return m_Name;  }
                set { this.m_Name = value; }
            }

            public Thread WorkerThread
            {
                get { return m_WorkerThread; }
            }

            public ClientHandler(Socket socket, string host, int port, string name)
            {
                this.m_Host = host;
                this.m_Port = port;
                this.m_Socket = socket;
                this.m_Name = name;
            }

            public void Start()
            {
                m_WorkerThread = new Thread(Work);
                m_WorkerThread.Start();
            }

            public void Stop()
            {
                this.m_ShouldStop = true;
            }

            public void Send(byte[] data)
            {
                this.m_Data = data;
                int total = 0;
                int size = this.m_Data.Length;
                int dataleft = size;
                int sent;
                try
                {
                    byte[] datasize = new byte[4];
                    datasize = BitConverter.GetBytes(size);
                    int bytesSent = this.m_Socket.Send(datasize);

                    while (total < size)
                    {
                        sent = this.m_Socket.Send(this.m_Data, total, dataleft, SocketFlags.None);
                        total += sent;
                        dataleft -= sent;
                    }

                    if (DataSentEvent != null)
                        DataSentEvent(this, new CustomEventArgs.DataEventArgs(m_Data));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.SocketErrorCode.ToString());
                }
            }

            private void Work()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // An incoming connection needs to be processed.
                while (!m_ShouldStop)
                {
                    try
                    {
                        int total = 0;
                        int recv;
                        byte[] datasize = new byte[4];

                        recv = m_Socket.Receive(datasize, 0, 4, 0);
                        int size = BitConverter.ToInt32(datasize, 0);
                        int dataleft = size;
                        m_Data = new byte[size];

                        while (total < size)
                        {
                            recv = m_Socket.Receive(m_Data, total, dataleft, 0);
                            if (recv == 0)
                                break;
                            total += recv;
                            dataleft -= recv;
                        }

                        if (DataReceivedEvent != null)
                            DataReceivedEvent(this, new CustomEventArgs.DataEventArgs(m_Data));
                    }
                    catch (SocketException ex)
                    {
                        m_ShouldStop = true;
                        Console.WriteLine("Connection Dropped by " + this.Name + ex.ToString());
                    }
                }

                if (this.m_Socket != null && this.m_Socket.Connected)
                {
                    this.m_Socket.Shutdown(SocketShutdown.Both);
                    this.m_Socket.Close();
                }

                if (ClientDisconnectedEvent != null)
                    ClientDisconnectedEvent(this);
            }
        }
    }

    /// <summary>
    /// Synchronous Client. Taken from MSDN. http://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
    /// This class was taken and edited by me, adding a class constructor, and custom event calls were added for the purpose of multi-treading
    /// </summary>
    public class Client
    {
        private int m_Port;
        private string m_Host;
        private IPEndPoint m_RemoteEndP;
        private IPAddress m_IP;
        private Thread m_ReceiveWorker;
        private Socket m_Socket;
        private volatile bool m_ShouldStop;
        private byte[] m_Data;

        public Socket Socket
        {
            get { return m_Socket; }
        }

        public delegate void ConnectedHandler(CustomEventArgs.DataEventArgs e);
        public delegate void DataSentHandler(CustomEventArgs.DataEventArgs e);
        public delegate void DataReceivedHandler(CustomEventArgs.DataEventArgs e);

        public event ConnectedHandler ConnectedEvent;
        public event DataSentHandler DataSentEvent;
        public event DataReceivedHandler DataReceivedEvent;
        public Client(string host = "127.0.0.1", int port = 100)
        {
            this.m_Host = host;
            this.m_Port = port;
            this.InitializeSocket();
            this.m_ReceiveWorker = new Thread(Work);
            this.m_ReceiveWorker.Start();
        }

        public void InitializeSocket() {
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
                while (!m_Socket.Connected) ;

                // Flag out to tell the client that the socket has been created
                if (ConnectedEvent != null)
                    ConnectedEvent(new CustomEventArgs.DataEventArgs(ASCIIEncoding.ASCII.GetBytes(this.m_Host + ":" + this.m_Port.ToString())));
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.IsConnected)
                    Console.WriteLine("Connection already established");
                else
                    Console.WriteLine(ex.SocketErrorCode.ToString());
            }
        }

        public void AssignName(string name)
        {
            if (this.m_Socket != null && this.m_Socket.Connected)
            {
                Send(ASCIIEncoding.ASCII.GetBytes("Name=" + name));
            }
        }

        public void Send(byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;
            try
            {
                byte[] datasize = new byte[4];
                datasize = BitConverter.GetBytes(size);
                int bytesSent = this.m_Socket.Send(datasize);

                while (total < size)
                {
                    sent = this.m_Socket.Send(data, total, dataleft, SocketFlags.None);
                    total += sent;
                    dataleft -= sent;
                }

                if (DataSentEvent != null)
                    DataSentEvent(new CustomEventArgs.DataEventArgs(m_Data));
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.SocketErrorCode.ToString());
            }
        }

        private void Work()
        {
            byte[] bytes = new byte[1024];
            while(!m_ShouldStop)
            {
                try
                {
                    int total = 0;
                    int recv;
                    byte[] datasize = new byte[4];

                    recv = m_Socket.Receive(datasize, 0, 4, 0);
                    int size = BitConverter.ToInt32(datasize, 0);
                    int dataleft = size;
                    m_Data = new byte[size];

                    while (total < size)
                    {
                        recv = m_Socket.Receive(m_Data, total, dataleft, 0);
                        if (recv == 0)
                            break;
                        total += recv;
                        dataleft -= recv;
                    }

                    if (DataReceivedEvent != null)
                        DataReceivedEvent(new CustomEventArgs.DataEventArgs(m_Data));
                }
                catch (SocketException ex)
                {
                    m_ShouldStop = true;
                    Console.WriteLine("Connection Dropped by Server" + ex.ToString());
                }
            }
        }
    }
}
