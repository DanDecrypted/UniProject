﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UniProject.Core
{
    public class WinAPI
    {
        [DllImport("user32")]
        public static extern void LockWorkStation();

        [DllImport("User32.Dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);

        public static bool IsProcessRunning(string processName)
        {
            bool temp = false;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == processName)
                {
                    temp = true;
                    break;
                }
            }
            return temp;
        }

        public static void BrowseURL(string url)
        {
            StartProcess(url);
        }

        public static void Shutdown()
        {
            StartProcess("shutdown", "/s /t 0");
        }

        public static void StartProcess(string process, string arguments = "")
        {
            if (arguments == "")
                Process.Start(process);
            else 
                Process.Start(process, arguments);
        }
    }
}