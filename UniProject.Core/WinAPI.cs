using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace UniProject.Core
{
    public class WinAPI
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();
    }
}
