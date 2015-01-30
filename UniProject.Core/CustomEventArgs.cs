using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniProject.Core;

namespace UniProject.Core.CustomEventArgs
{
    public class DataEventArgs : EventArgs
    {
        private byte[] m_Data;
        public byte[] Data
        {
            get { return m_Data; }
            set { this.m_Data = value; }
        }
        public override string ToString()
        {
            return Data.ToString();
        }

        public DataEventArgs(byte[] data)
        {
            this.m_Data = data;
        }
    }
}
