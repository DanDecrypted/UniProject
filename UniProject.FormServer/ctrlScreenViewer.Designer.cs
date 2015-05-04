namespace UniProject.FormServer
{
    partial class ctrlScreenViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblClientID = new System.Windows.Forms.Label();
            this.imgScreen = new System.Windows.Forms.PictureBox();
            this.lblCurrentUser = new System.Windows.Forms.Label();
            this.clientCommandsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneToOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).BeginInit();
            this.clientCommandsMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblClientID
            // 
            this.lblClientID.AutoSize = true;
            this.lblClientID.Location = new System.Drawing.Point(4, 4);
            this.lblClientID.Name = "lblClientID";
            this.lblClientID.Size = new System.Drawing.Size(54, 13);
            this.lblClientID.TabIndex = 0;
            this.lblClientID.Text = "lblClientID";
            this.lblClientID.Click += new System.EventHandler(this.ClientArea_Click);
            this.lblClientID.DoubleClick += new System.EventHandler(this.OneToOneMode_Activate);
            // 
            // imgScreen
            // 
            this.imgScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgScreen.Location = new System.Drawing.Point(7, 21);
            this.imgScreen.Name = "imgScreen";
            this.imgScreen.Size = new System.Drawing.Size(273, 157);
            this.imgScreen.TabIndex = 1;
            this.imgScreen.TabStop = false;
            this.imgScreen.Click += new System.EventHandler(this.ClientArea_Click);
            this.imgScreen.DoubleClick += new System.EventHandler(this.OneToOneMode_Activate);
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentUser.Location = new System.Drawing.Point(109, 2);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(171, 16);
            this.lblCurrentUser.TabIndex = 2;
            this.lblCurrentUser.Text = "lblCurrentUser";
            this.lblCurrentUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCurrentUser.Click += new System.EventHandler(this.ClientArea_Click);
            this.lblCurrentUser.DoubleClick += new System.EventHandler(this.OneToOneMode_Activate);
            // 
            // clientCommandsMenuStrip
            // 
            this.clientCommandsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lockToolStripMenuItem,
            this.shutdownToolStripMenuItem,
            this.oneToOneToolStripMenuItem});
            this.clientCommandsMenuStrip.Name = "clientCommandsMenuStrip";
            this.clientCommandsMenuStrip.Size = new System.Drawing.Size(142, 70);
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            this.lockToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.lockToolStripMenuItem.Text = "Lock";
            this.lockToolStripMenuItem.Click += new System.EventHandler(this.lockToolStripMenuItem_Click);
            // 
            // shutdownToolStripMenuItem
            // 
            this.shutdownToolStripMenuItem.Name = "shutdownToolStripMenuItem";
            this.shutdownToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.shutdownToolStripMenuItem.Text = "Shutdown";
            // 
            // oneToOneToolStripMenuItem
            // 
            this.oneToOneToolStripMenuItem.Name = "oneToOneToolStripMenuItem";
            this.oneToOneToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.oneToOneToolStripMenuItem.Text = "One To One ";
            this.oneToOneToolStripMenuItem.Click += new System.EventHandler(this.OneToOneMode_Activate);
            // 
            // ctrlScreenViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCurrentUser);
            this.Controls.Add(this.imgScreen);
            this.Controls.Add(this.lblClientID);
            this.Name = "ctrlScreenViewer";
            this.Size = new System.Drawing.Size(292, 190);
            this.Click += new System.EventHandler(this.ClientArea_Click);
            this.DoubleClick += new System.EventHandler(this.OneToOneMode_Activate);
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).EndInit();
            this.clientCommandsMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblClientID;
        public System.Windows.Forms.PictureBox imgScreen;
        public System.Windows.Forms.Label lblCurrentUser;
        private System.Windows.Forms.ContextMenuStrip clientCommandsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem lockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shutdownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oneToOneToolStripMenuItem;

    }
}
