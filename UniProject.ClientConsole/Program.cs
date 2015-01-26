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
            UniProject.Core.ClientServer.Client client = new UniProject.Core.ClientServer.Client();
            client.DataSentEvent += client_DataSentEvent;
            client.DataReceivedEvent += client_DataReceivedEvent;
            int count = 0;
            while (count < 10)
            {
                client.Send("Test" + count.ToString());
                count += 1;
                Console.ReadLine();
            }
            client.Send("Shutdown");
            Console.ReadLine();
        }

        static void client_DataReceivedEvent(Core.CustomEventArgs.DataEventArgs e)
        {
            string response = e.Data.ToString().Replace("<EOF>", "");
            Console.WriteLine(response + " from server");
        }

        static void client_DataSentEvent(Core.CustomEventArgs.DataEventArgs e)
        {
            Console.WriteLine(e.Data.ToString() + " sent to server");
        }
    }
}
