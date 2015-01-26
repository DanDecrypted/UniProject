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
        private Thread m_ClientCheckThread;
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

            // Create a TCP/IP socket.
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

                foreach (ClientHandler client in this.Clients)
                {
                    if (client.Name != "Client" + this.Clients.IndexOf(client).ToString())
                    {
                        client.Name = "Client" + this.Clients.IndexOf(client).ToString();
                        while (!client.Socket.Connected) ;
                        client.Send("NameChange=" + Clients.IndexOf(client));
                    }
                }

                if (ClientConnectedEvent != null)
                    ClientConnectedEvent(clientHandler);
            }
        }

        private void clientHandler_DataSentEvent(Server.ClientHandler client, CustomEventArgs.DataEventArgs e)
        {
            this.DataSentEvent(client, e);
        }

        private void clientHandler_ClientDisconnectedEvent(Server.ClientHandler client)
        {

            this.Clients.Remove(client);

            try
            {
                foreach (ClientHandler handler in this.Clients)
                {
                    if (handler.Name != "Client" + this.Clients.IndexOf(handler).ToString())
                    {
                        handler.Send("NameChange=Client" + this.Clients.IndexOf(handler).ToString());
                        handler.Name = "Client" + this.Clients.IndexOf(handler).ToString();
                    }
                }
            }
            catch
            {

            }

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
            private string m_Data;
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

            public void Send(string message)
            {
                try
                {
                    byte[] encodedMessage = Encoding.ASCII.GetBytes(message.ToString() + "<EOF>");
                    int bytesSent = this.m_Socket.Send(encodedMessage);
                    if (DataSentEvent != null)
                        DataSentEvent(this, new CustomEventArgs.DataEventArgs(message));
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
                        bytes = new Byte[1024];
                        int bytesRec = this.m_Socket.Receive(bytes);
                        m_Data = "";
                        m_Data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (m_Data.IndexOf("<EOF>") > -1)
                        {
                            if (DataReceivedEvent != null)
                                DataReceivedEvent(this, new CustomEventArgs.DataEventArgs(m_Data));
                        }
                    }
                    catch (SocketException ex)
                    {
                        m_ShouldStop = true;
                        Console.WriteLine("Connection Dropped by " + this.Name);
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
        private string m_Data;

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
                    ConnectedEvent(new CustomEventArgs.DataEventArgs(this.m_Host + ":" + this.m_Port.ToString()));
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.IsConnected)
                    Console.WriteLine("Connection already established");
                else
                    Console.WriteLine(ex.SocketErrorCode.ToString());
            }
        }

        public int Send(string message)
        {
            try
            {
                byte[] encodedMessage = Encoding.ASCII.GetBytes(message.ToString() + "<EOF>");
                int bytesSent = this.Socket.Send(encodedMessage);
                if (DataSentEvent != null)
                    DataSentEvent(new CustomEventArgs.DataEventArgs(message));
                    
                if (message == "Shutdown")
                {
                    this.Socket.Shutdown(SocketShutdown.Both);
                    this.Socket.Close();
                }
                return bytesSent;
            }
            catch (SocketException ex)

            {
                Console.WriteLine(ex.SocketErrorCode.ToString());
                return 0;
            }
        }

        private void Work()
        {
            byte[] bytes = new byte[1024];
            while(!m_ShouldStop)
            {
                try
                {
                    bytes = new byte[1024];
                    int bytesRec = this.m_Socket.Receive(bytes);
                    m_Data = "";
                    m_Data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (m_Data.IndexOf("<EOF>") > -1)
                    {
                        if (DataReceivedEvent != null)
                            DataReceivedEvent(new CustomEventArgs.DataEventArgs(m_Data));
                    }
                }
                catch (SocketException ex)
                {
                    m_ShouldStop = true;
                    Console.WriteLine("Connection Dropped by Server");
                }
            }
        }
    }
}
