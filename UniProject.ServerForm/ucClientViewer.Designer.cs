namespace UniProject.ServerForm
{
    partial class ucClientViewer
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
            this.imgClientScreen = new System.Windows.Forms.PictureBox();
            this.lblClientID = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgClientScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // imgClientScreen
            // 
            this.imgClientScreen.Location = new System.Drawing.Point(4, 26);
            this.imgClientScreen.Name = "imgClientScreen";
            this.imgClientScreen.Size = new System.Drawing.Size(210, 210);
            this.imgClientScreen.TabIndex = 0;
            this.imgClientScreen.TabStop = false;
            // 
            // lblClientID
            // 
            this.lblClientID.AutoSize = true;
            this.lblClientID.Location = new System.Drawing.Point(4, 0);
            this.lblClientID.Name = "lblClientID";
            this.lblClientID.Size = new System.Drawing.Size(35, 13);
            this.lblClientID.TabIndex = 1;
            this.lblClientID.Text = "label1";
            // 
            // ucClientViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblClientID);
            this.Controls.Add(this.imgClientScreen);
            this.Name = "ucClientViewer";
            this.Size = new System.Drawing.Size(220, 240);
            ((System.ComponentModel.ISupportInitialize)(this.imgClientScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgClientScreen;
        private System.Windows.Forms.Label lblClientID;
    }
}
