using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class Server
    {
        public delegate void ClientConnectedHandler(object sender, CustomEventArgs e);
        public delegate void ClientDisconnectedHandler(object sender, CustomEventArgs e);
        public delegate void DataReceivedHandler(object sender, CustomEventArgs e);
        public delegate void DataSentHandler(object sender, CustomEventArgs e);
        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;
        public event DataReceivedHandler DataReceived;
        public event DataSentHandler DataSent;

        private TcpListener m_Socket;
        private Thread m_ListenerThread;
        private List<ClientHandler> m_Clients;
        private volatile bool m_ShouldListen;

        public TcpListener Socket
        {
            get { return m_Socket; }
        }

        public List<ClientHandler> Clients
        {
            get { return m_Clients; }
        }
        public Server(IPAddress addr, ushort port)
        {
            m_ShouldListen = true;
            m_Socket = new TcpListener(addr, port);
            m_ListenerThread = new Thread(AcceptClients);
            m_Clients = new List<ClientHandler>();
        }
        private void AcceptClients()
        {
            while (m_ShouldListen)
            {
                ClientHandler newClient = new ClientHandler(this, m_Socket.AcceptTcpClient());
                newClient.DataReceived += ClientHandler_DataReceived;
                newClient.DataSent += ClientHandler_DataSent;
                newClient.ClientDisconnected += ClientHandler_ClientDisconnected;
                AddClient(newClient);
                newClient.Start();
                
                if (ClientConnected != null)
                    ClientConnected(newClient, new CustomEventArgs(newClient.Address.ToString()));

                newClient = null;
            }
        }

        void ClientHandler_ClientDisconnected(object sender, CustomEventArgs e)
        {
            RemoveClient((ClientHandler)sender);
            if (ClientDisconnected != null)
                ClientDisconnected(sender, e);
        }

        void ClientHandler_DataSent(object sender, CustomEventArgs e)
        {
            if (DataSent != null)
                DataSent(sender, e);
        }

        void ClientHandler_DataReceived(object sender, CustomEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(sender, e);
        }

        public void AddClient(ClientHandler client)
        {
            lock (m_Clients)
            {
                m_Clients.Add(client);
                Console.WriteLine("Currently Connected Clients {0}", m_Clients.Count);
            }
        }

        public void RemoveClient(ClientHandler client)
        {
            lock (m_Clients)
            {
                m_Clients.Remove(client);
                Console.WriteLine("Currently Connected Clients {0}", m_Clients.Count);
            }
        }

        public void Start()
        {
            m_Socket.Start();
            if (!m_ListenerThread.IsAlive)
                m_ListenerThread.Start();
        }

        public void Stop()
        {
            m_Socket.Stop();
            m_ShouldListen = false;
        }
    }
}
