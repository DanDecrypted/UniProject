using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.FormServer
{
    public partial class ctrlScreenViewer : UserControl
    {
        private ClientHandler client;
        public frmOneToOne OneToOneForm;
        public bool OneToOneMode = false;
        public ctrlScreenViewer()
        {
            InitializeComponent();
        }

        public ctrlScreenViewer(string clientID, ClientHandler client)
        {
            InitializeComponent();
            this.client = client;
            this.lblClientID.Text = clientID;
            this.lblCurrentUser.Text = client.CurrentUser;
        }

        private void OneToOneMode_Activate(object sender, EventArgs e)
        {
            if (!OneToOneMode)
            {
                OneToOneForm = new frmOneToOne("One to One session with: " + this.lblClientID.Text, this.client);
                OneToOneForm.Show();
                OneToOneForm.FormClosed += oneToOneForm_FormClosed;
                OneToOneForm.Bounds = Screen.PrimaryScreen.Bounds;
                OneToOneForm.FormBorderStyle = FormBorderStyle.None;
                OneToOneForm.WindowState = FormWindowState.Maximized;
                OneToOneMode = true;
            }
        }

        private void ClientArea_Click(object sender, EventArgs e)
        {
            this.clientCommandsMenuStrip.Show(Cursor.Position);
        }

        void oneToOneForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            OneToOneMode = false;
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.client.Send("WinAPI.Lock");
        }

        private void softLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.client.Send("SoftAPI.Lock");
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.client.Send("WinAPI.Shutdown");
        }
    }
}
