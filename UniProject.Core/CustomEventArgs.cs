using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniProject.Core
{
    public class CustomEventArgs
    {
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
