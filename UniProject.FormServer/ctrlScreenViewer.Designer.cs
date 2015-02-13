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
            this.lblClientID = new System.Windows.Forms.Label();
            this.imgScreen = new System.Windows.Forms.PictureBox();
            this.lblCurrentUser = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).BeginInit();
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
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentUser.AutoSize = true;
            this.lblCurrentUser.Location = new System.Drawing.Point(210, 5);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblCurrentUser.Size = new System.Drawing.Size(73, 13);
            this.lblCurrentUser.TabIndex = 2;
            this.lblCurrentUser.Text = "lblCurrentUser";
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
            ((System.ComponentModel.ISupportInitialize)(this.imgScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblClientID;
        public System.Windows.Forms.PictureBox imgScreen;
        public System.Windows.Forms.Label lblCurrentUser;

    }
}
