using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class CustomEventArgs
    {
        public class BaseCustomEventArgs : EventArgs
        {
            private string m_Data;
            public string Data
            {
                get { return m_Data; }
                set { this.m_Data = value; }
            }
            public override string ToString()
            {
                return Data.ToString();
            }

            public BaseCustomEventArgs(string data = "")
            {
                this.m_Data = data;
            }
        }
        public class ClientErrorEventArgs : BaseCustomEventArgs
        {
            public ClientErrorEventArgs(string data) : base(data) { }
        }
        public class DataReceivedEventArgs : BaseCustomEventArgs
        {
            public DataReceivedEventArgs(string data) : base(data) { }
        }
        public class SocketConnectionEventArgs : EventArgs
        {
            private string m_Host;
            private int m_Port;
            public int Port
            {
                get { return m_Port; }
            }
            public string Host
            {
                get { return m_Host; }
            }
            public SocketConnectionEventArgs(string host, int port)
            {
                this.m_Host = host;
                this.m_Port = port;
            }

            public override string ToString()
            {
                return this.m_Host + ":" + this.m_Port;
            }
        }
    }
}
