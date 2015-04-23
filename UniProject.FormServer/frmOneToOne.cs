using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniProject.Core;

namespace UniProject.FormServer
{
    public partial class frmOneToOne : Form
    {
        ClientHandler client;
        public frmOneToOne()
        {
            InitializeComponent();
        }

        public frmOneToOne(string title, ClientHandler client)
        {
            InitializeComponent();
            this.Text = title;
            this.client = client;
        }

        private void btnExitFullscreen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmOneToOne_Resize(object sender, EventArgs e)
        {
            this.btnExitFullscreen.Location = new Point((this.Width / 2) - (this.btnExitFullscreen.Width / 2), this.btnExitFullscreen.Location.Y);
        }
    }
}
