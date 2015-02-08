using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniProject.Core;
using System.Net;
using System.Net.Sockets;
using System.Data;

namespace UniProject.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Any, 101);
            server.Start();
            server.ClientConnected += server_ClientConnected;
            server.ClientDisconnected += server_ClientDisconnected;
            server.DataReceived += server_DataReceived;
            Console.WriteLine("Sevrer Started {0}:{1}", ((IPEndPoint)server.Socket.LocalEndpoint).Address.ToString(), ((IPEndPoint)server.Socket.LocalEndpoint).Port.ToString());
            string command;
            while (true)
            {
                command = Console.ReadLine();
                foreach (ClientHandler client in server.Clients)
                {
                    client.Send(command);
                }
            }
        }

        static void server_DataReceived(object sender, CustomEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        static void server_ClientDisconnected(object sender, CustomEventArgs e)
        {
            Console.WriteLine("Client Disconnected {0}", e.ToString());
        }

        static void server_ClientConnected(object sender, CustomEventArgs e)
        {
            Console.WriteLine("Client Connected {0}", e.ToString());
        }
    }
}
