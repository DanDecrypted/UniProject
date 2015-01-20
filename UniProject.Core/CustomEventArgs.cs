using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class CustomEventArgs
    {
        public class SocketConnectedEventArgs : EventArgs
        {
            private string m_Host;
            private int m_Port;
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
            public SocketConnectedEventArgs(string host, int port)
            {
                this.m_Host = host;
                this.m_Port = port;
            }

            public override string ToString()
            {
                return this.m_Host + ":" + this.m_Port;
            }
        }
        public class DataReceivedEventArgs : EventArgs
        {
            private string m_Data;
            public string Data
            {
                get
                {
                    return m_Data;
                }
                set
                {
                    this.m_Data = value;
                }
            }
            public override string ToString()
            {
                return Data.ToString();
            }

            public DataReceivedEventArgs(string data="")
            {
                if (data.IndexOf("<EOF>") > -1)
                {
                    data = data.Replace("<EOF>", "");
                }
                this.m_Data = data;
            }
        }
    }
}
