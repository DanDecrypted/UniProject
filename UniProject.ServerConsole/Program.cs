using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniProject.Core;

namespace UniProject.ServerConsole
{
    class Program
    {
        static UniProject.Core.ClientServer.Server server;
        static void Main(string[] args)
        {
            server = new UniProject.Core.ClientServer.Server("127.0.0.1", 100, 100);
            Console.Title = "Clients connected 0/" + server.MaxConnections;
            server.ClientConnectedEvent += server_ClientConnected;
            server.ClientDisconnectedEvent += server_ClientDisconnected;
            server.DataReceivedEvent += server_DataReceived;
            server.DataSentEvent += server_DataSentEvent;
            Console.WriteLine("Server Started {0}:{1}", server.Host, server.Port);
            while (server.IsAlive) ; //keep going till the server shuts down

            Console.WriteLine("Server Gracefully shutdown");
            Console.ReadLine();
        }

        static void server_DataSentEvent(Core.ClientServer.Server.ClientHandler client, Core.CustomEventArgs.DataEventArgs e)
        {
            Console.WriteLine(ASCIIEncoding.ASCII.GetString(e.Data).ToString() + " Sent to " + client.Name);
        }

        static void server_ClientDisconnected(UniProject.Core.ClientServer.Server.ClientHandler client)
        {
            Console.Title = String.Format("Clients connected {0}/{1}", server.Clients.Count(), server.MaxConnections);
            Console.WriteLine("Client Disconnected {0}. Clients connected {1}/{2}", client.Name, server.Clients.Count(), server.MaxConnections);
        }

        static void server_ClientConnected(UniProject.Core.ClientServer.Server.ClientHandler client)
        {
            Console.Title = String.Format("Clients connected {0}/{1}", server.Clients.Count(), server.MaxConnections);
            Console.WriteLine("Client Connected {0}. Clients connected {1}/{2}", client.Name, server.Clients.Count(), server.MaxConnections);    
        }

        static void server_DataReceived(UniProject.Core.ClientServer.Server.ClientHandler client, Core.CustomEventArgs.DataEventArgs e)
        {
            Console.WriteLine(client.Name + ": " + e.Data.ToString());
            if (ASCIIEncoding.ASCII.GetString(e.Data).ToString() == "Shutdown")
            {
                server.StopListening();
            }
        }
    }
}
