using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniProject.Core;

namespace UniProject.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("10.248.0.10", 101);
            client.ClientConnected += client_ClientConnected;
            client.DataReceived += client_DataReceived;
            client.Start();
            Console.ReadLine();
            client.Send("Test test test");
        }

        static void client_DataReceived(CustomEventArgs e)
        {
            string[] args = e.ToString().Split('.');
            Console.WriteLine(e.ToString());
            if (args[0] == "WinAPI")
            {
                if (args[1] == "Lock")
                {
                    WinAPI.LockWorkStation();
                    Console.WriteLine("Locking Work Station...");
                }
            }
        }

        static void client_ClientConnected(CustomEventArgs e)
        {
            Console.WriteLine("Client Connected to server " + e.ToString());
        }
    }
}
