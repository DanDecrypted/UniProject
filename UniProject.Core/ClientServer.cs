using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UniProject.Core
{
    public class ClientServer
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

            // Incoming data from the client.
            public string data = "";
            public Socket Socket;
            public Socket Listener;

            public delegate void ClientConnectedHandler(Socket client, CustomEventArgs.SocketConnectionEventArgs e);
            public delegate void ClientDisconnectedHandler(Socket client, CustomEventArgs.SocketConnectionEventArgs e);
            public delegate void DataReceivedHandler(Socket client, CustomEventArgs.DataReceivedEventArgs e);

            public event ClientConnectedHandler ClientConnected;
            public event ClientDisconnectedHandler ClientDisconnected;
            public event DataReceivedHandler DataReceived;
            public int Port
            {
                get
                {
                    return m_Port;
                }
            }

            public string Host
            {
                get
                {
                    return m_Host;
                }
            }

            public int MaxConnections
            {
                get
                {
                    return m_MaxConnections;
                }
            }

            public Server(string host = "127.0.0.1", int port = 100, int maxConnections = 100)
            {
                this.m_Port = port;
                this.m_Host = host;
                this.m_MaxConnections = maxConnections;
            }

            public void StartListening()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse(this.m_Host);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.m_Port);

                // Create a TCP/IP socket.
                Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                Listener.Bind(localEndPoint);
                Listener.Listen(this.m_MaxConnections);

                // Start listening for connections.
                while (!data.Contains("DestroyServer<EOF>"))
                {
                    // Program is suspended while waiting for an incoming connection.
                    Socket = Listener.Accept();
                    CustomEventArgs.SocketConnectionEventArgs sc = new CustomEventArgs.SocketConnectionEventArgs(this.m_Host, this.m_Port);
                    if (ClientConnected != null)
                        ClientConnected(Socket, sc);
                    
                    // Reset data field ready for the next client connection
                    data = "";
                    
                    // An incoming connection needs to be processed.
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = Socket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            CustomEventArgs.DataReceivedEventArgs dr = new CustomEventArgs.DataReceivedEventArgs(data);
                            if (DataReceived != null)
                                DataReceived(Socket, dr);
                            break;
                        }
                        data = "";
                    }

                    if (ClientDisconnected != null)
                        ClientDisconnected(Socket, sc);

                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
            }

            /// <summary>
            /// Creates an instance of Client() to give the server the message to shut down as it's synchronous, this seems to be the safest way to exit out of the thread.
            /// </summary>
            public void StopListening()
            {
                Client client = new Client(this.m_Host, this.m_Port);
                client.InitializeSocket();
                client.Send("DestroyServer");
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
            private IPEndPoint remoteEP;
            private IPAddress ipAddress;

            public Socket Socket;

            public delegate void ClientConnectedHandler(Socket socket, CustomEventArgs.SocketConnectionEventArgs e);
            public delegate void ClientErrorHandler(CustomEventArgs.BaseCustomEventArgs e);
            public delegate void ClientDataSentHandler(CustomEventArgs.BaseCustomEventArgs e);

            public event ClientConnectedHandler ConnectedEvent;
            public event ClientErrorHandler ErrorEvent;
            public event ClientDataSentHandler DataSentEvent;
            public Client(string host = "127.0.0.1", int port = 100)
            {
                this.m_Host = host;
                this.m_Port = port;
            }
            public void InitializeSocket() {
                try
                {
                    // Establish the remote endpoint for the socket.
                    ipAddress = IPAddress.Parse(this.m_Host);
                    remoteEP = new IPEndPoint(ipAddress, this.m_Port);

                }
                catch (Exception ex)
                {
                    ErrorEvent(new CustomEventArgs.ClientErrorEventArgs(ex.ToString()));
                }
            }

            public int Send(string message)
            {
                try
                {
                    // Create a TCP/IP  socket if it hasn't already been created
                    if (Socket == null)
                        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Attempt to make a connection
                    Socket.Connect(remoteEP);

                    // Give the socket time to initialize 
                    while (!Socket.Connected) ;

                    // Flag out to tell the client that the socket has been created
                    CustomEventArgs.SocketConnectionEventArgs sce = new CustomEventArgs.SocketConnectionEventArgs(this.m_Host, this.m_Port);
                    if (ConnectedEvent != null)
                        ConnectedEvent(this.Socket, sce);

                    byte[] encodedMessage = Encoding.ASCII.GetBytes(message.ToString() + "<EOF>");
                    int bytesSent = this.Socket.Send(encodedMessage);
                    if (DataSentEvent != null)
                        DataSentEvent(new CustomEventArgs.BaseCustomEventArgs(message));
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                    Socket = null;
                    return bytesSent;
                }
                catch (Exception ex)
                {
                    if (Socket.Connected)
                    {
                        Socket.Shutdown(SocketShutdown.Both);
                        Socket.Close();
                    }
                    ErrorEvent(new CustomEventArgs.BaseCustomEventArgs(ex.ToString()));
                    return 0;
                }
            }
        }
    }
}
