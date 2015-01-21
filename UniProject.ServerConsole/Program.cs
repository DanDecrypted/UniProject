using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniProject.Core;

namespace UniProject.ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientServer.Server server = new ClientServer.Server("127.0.0.1", 100, 100);
            server.StartListening();
            Console.WriteLine("Listening {0}:{1}", server.Host, server.Port);
            Console.ReadLine();
        }
    }
}
