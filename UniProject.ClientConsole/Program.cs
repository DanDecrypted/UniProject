using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniProject.Core;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UniProject.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientServer.Client client = new ClientServer.Client();
            client.StartClient();
            Console.WriteLine("Sending Test data: Test<EOF>...");
            client.Socket.Shutdown(SocketShutdown.Both);
            client.Socket.Close();
            Console.ReadLine();
        }
    }
}
