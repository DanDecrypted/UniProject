using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniProject.FormServer
{
    public partial class ctrlScreenViewer : UserControl
    {
        public ctrlScreenViewer()
        {
            InitializeComponent();
        }

        public ctrlScreenViewer(string clientID)
        {
            InitializeComponent();
            this.lblClientID.Text = clientID;
        }
    }
}
