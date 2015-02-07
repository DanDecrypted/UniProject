using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class CustomEventArgs : EventArgs
    {
        private byte[] m_Data;
        
        public CustomEventArgs(byte[] data)
        {
            this.m_Data = data;
        }

        public CustomEventArgs(string data)
        {
            this.m_Data = ASCIIEncoding.ASCII.GetBytes(data);
        }

        public override string ToString()
        {
            return ASCIIEncoding.ASCII.GetString(m_Data);
        }

        public byte[] GetBytes()
        {
            return m_Data;
        }
    }
}
