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
            Console.Title = "UniProject.Client";
            client.DataSentEvent += client_DataSentEvent;
            client.DataReceivedEvent += client_DataReceivedEvent;
            int count = 0;
            while (count < 10)
            {
                Console.ReadLine();
                client.Send(ASCIIEncoding.ASCII.GetBytes("Test " + count.ToString()));
                count += 1;
            }
            
        }

        static void client_DataReceivedEvent(Core.CustomEventArgs.DataEventArgs e)
        {
            string response = ASCIIEncoding.ASCII.GetString(e.Data).Replace("<EOF>", "");
            Console.WriteLine(response + " from server");
            string[] data = response.Split('=');
            if (data[0] == "NameChange")
                Console.Title = data[1];
        }

        static void client_DataSentEvent(Core.CustomEventArgs.DataEventArgs e)
        {
            Console.WriteLine(ASCIIEncoding.ASCII.GetString(e.Data).ToString() + " sent to server");
        }
    }
}
