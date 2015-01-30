using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniProject.ServerForm
{
    public partial class ucClientViewer : UserControl
    {
        public Image Image
        {
            get { return this.imgClientScreen.Image; }
            set { this.imgClientScreen.Image = value; }
        }

        public string ClientID
        {
            get { return this.lblClientID.Text; }
            set { this.lblClientID.Text = value; }
        }
        public ucClientViewer()
        {
            InitializeComponent();
        }

        public ucClientViewer(string ClientID)
        {
            InitializeComponent();
            this.lblClientID.Text = ClientID;
        }
    }
}
