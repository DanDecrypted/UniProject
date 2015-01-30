using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.ScreenshotTest
{
    public partial class Form1 : Form
    {
        Thread imageThread;
        //Create a new bitmap.
        static Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
        UniProject.Core.ClientServer.Client client;
        public Form1()
        {
            InitializeComponent();
            client = new Core.ClientServer.Client();
            client.DataSentEvent += client_DataSentEvent;
            client.ConnectedEvent += client_ConnectedEvent;
        }

        void client_ConnectedEvent(Core.CustomEventArgs.DataEventArgs e)
        {
            client.AssignName(Environment.MachineName);
        }

        void client_DataSentEvent(Core.CustomEventArgs.DataEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            imageThread = new Thread(LiveFeed);
            imageThread.Start();
            while (!imageThread.IsAlive) ;

        }

        private void UpdateImage()
        {

        }

        private void LiveFeed()
        {
            MemoryStream ms = new MemoryStream();
            while(true)
            {
                // The screenshot will be stored in this bitmap.
                bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using (Graphics g = Graphics.FromImage(bmpScreenshot))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                }
                bmpScreenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                client.Send(ms.ToArray());
                Thread.Sleep(100);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            imageThread.Abort();
            imageThread.Join();
            
        }
    }
}
