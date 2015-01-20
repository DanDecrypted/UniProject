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
        /// </summary>
        public class Server
        {
            private int m_Port;
            private int m_MaxConnections;
            private string m_Host;

            // Incoming data from the client.
            public string data = null;
            public Socket Socket;
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

            public Server(int port = 100, string host = "127.0.0.1", int maxConnections = 100)
            {
                this.m_Port = port;
                this.m_Host = host;
                this.m_MaxConnections = maxConnections;
            }

            public void StartListening()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.s
                IPHostEntry ipHostInfo = Dns.GetHostEntry(this.m_Host);
                IPAddress ipAddress = ipHostInfo.AddressList[3];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.m_Port);

                // Create a TCP/IP socket.
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Bind the socket to the local endpoint and 
                // listen for incoming connections.
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(this.m_MaxConnections);

                    // Start listening for connections.
                    while (true)
                    {
                        // Program is suspended while waiting for an incoming connection.
                        Socket = listener.Accept();
                        data = null;

                        // An incoming connection needs to be processed.
                        while (true)
                        {
                            bytes = new byte[1024];
                            int bytesRec = Socket.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data.IndexOf("<EOF>") > -1)
                            {
                                break;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Synchronous Client. Taken from MSDN. http://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
        /// </summary>
        public class Client
        {
            private int m_Port;
            private int m_MaxConnections;
            private string m_Host;
            public Socket Socket;
            public Client(int port = 100, string host = "127.0.0.1")
            {
                this.m_Port = port;
                this.m_Host = host;
            }
             public void StartClient() {
                // Data buffer for incoming data.
                byte[] bytes = new byte[1024];

                // Connect to a remote device.
                try {
                    // Establish the remote endpoint for the socket.
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(this.m_Host);
                    IPAddress ipAddress = ipHostInfo.AddressList[3];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, this.m_Port);

                    // Create a TCP/IP  socket.
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

                    // Connect the socket to the remote endpoint. Catch any errors.
                    try {
                        Socket.Connect(remoteEP);

                        Console.WriteLine("Socket connected to {0}", Socket.RemoteEndPoint.ToString());
                
                    } catch (ArgumentNullException ane) {
                        Console.WriteLine("ArgumentNullException : {0}",ane.ToString());
                    } catch (SocketException se) {
                        Console.WriteLine("SocketException : {0}",se.ToString());
                    } catch (Exception e) {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    }

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
