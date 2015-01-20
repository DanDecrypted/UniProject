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
        /// State object for reading client data asynchronously.
        /// </summary>
        public class StateObject
        {
            // Socket for processing requests.
            public Socket WorkSocket = null;
            public const int BufferSize = 1024;
            public byte[] Buffer = new byte[BufferSize];
            
            // Received data string
            public StringBuilder Data = new StringBuilder();
        }


        /// <summary>
        /// Asynchronous client class. http://msdn.microsoft.com/en-us/library/bew39x2a(v=vs.110).aspx was the tutorial/documentation used in the creation of this class
        /// </summary>
        public class Client
        {
            public int Port;
            public string Host;
            public Socket Socket;
            
            // ManualResetEvent instaces signal completion
            public ManualResetEvent connectDone = new ManualResetEvent(false);
            public ManualResetEvent SendDone = new ManualResetEvent(false);
            public ManualResetEvent ReceiveDone = new ManualResetEvent(false);

            // Response from server
            public String Response = String.Empty;

            public Client(int port = 100, string host = "127.0.0.1")
            {
                this.Port = port;
                this.Host = host;
            }

            public void StartClient()
            {
                // Connect to a remote device.
                try
                {
                    // Establish the remote endpoint the socket.
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(this.Host);
                    IPAddress ipAddress = ipHostInfo.AddressList[3];
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, this.Port);

                    // Create a TCP/IP socket
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the remote endpoint. 
                    Socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), Socket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            public void ConnectCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete the connection.
                    client.EndConnect(ar);

                    Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                    // Signal that the connection has been made.
                    connectDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void Send(Socket client, String data)
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }

            public void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = client.EndSend(ar);

                    // Signal that all bytes have been sent.
                    SendDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void Receive(Socket client)
            {
                try
                {
                    // Create the state object.
                    StateObject state = new StateObject();
                    state.WorkSocket = client;

                    // Begin receiving the data from the remote device.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the state object and the client socket from the asynchronous state object.
                    StateObject state = (StateObject)ar.AsyncState;
                    Socket client = state.WorkSocket;

                    // Read data from the remote device.
                    int bytesRead = client.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        // There might be more data, so store the data received so far.
                        state.Data.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                        // Get the rest of the data.
                        client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // All the data has arrived; put it in response.
                        if (state.Data.Length > 1)
                        {
                            Response = state.Data.ToString();
                        }
                        // Signal that all bytes have been received.
                        ReceiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Asyncronus Server class. http://msdn.microsoft.com/en-us/library/5w7b7x5f(v=vs.110).aspx was the tutorial/documentation used in the creation of this class
        /// </summary>
        public class Server
        {
            // Thread signal.
            public ManualResetEvent AllDone = new ManualResetEvent(false);
            public int Port;
            public int MaxConnections;
            public string Host;
            private volatile bool m_ShouldStop;

            public Server(int port = 100, string host = "127.0.0.1", int maxConnections = 100)
            {
                this.Port = port;
                this.Host = host;
                this.MaxConnections = maxConnections;
            }

            public void StartListening()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Local endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(this.Host);
                IPAddress ipAddress = ipHostInfo.AddressList[3];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.Port);

                // Create a TCP/IP socket.
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                // Bind the socket to the local endpoint and listen for incoming connections
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(this.MaxConnections);

                    Console.WriteLine("Waiting for a connection...");
                    while (!m_ShouldStop)
                    {
                        // Set the event to a nonsignaled state.
                        AllDone.Reset();

                        // Start an asynchronous socket to listen for connections.
                        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                        AllDone.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

            }

            public void RequestStop()
            {
               
                m_ShouldStop = true;
            }

            public void AcceptCallback(IAsyncResult ar)
            {
                // Signal the main thread to continue.
                AllDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                Console.WriteLine("Client accepted");

                //Create the state object.
                StateObject state = new StateObject();
                state.WorkSocket = handler;
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }

            public void ReadCallback(IAsyncResult ar)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket from the async object
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.WorkSocket;

                // Read data from the client socket
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, beyond the buffer. So we need to store what we have collected so far 
                    state.Data.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead)); 

                    // Check for the end-of-file tag. if it's not there, read more data.
                    content = state.Data.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        // All the data has been read from the client. Display it to console.
                        Console.WriteLine("Read {0} bytes from socket. \n Data: {1}", content.Length, content);
                        // Echo the data back to the client.
                    }
                }
            }

            public void Send(Socket handler, String data)
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device. 
                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }

            public void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = handler.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
