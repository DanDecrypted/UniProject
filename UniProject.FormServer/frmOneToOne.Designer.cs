namespace UniProject.FormServer
{
    partial class frmOneToOne
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ClientScreen = new System.Windows.Forms.PictureBox();
            this.btnExitFullscreen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ClientScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // ClientScreen
            // 
            this.ClientScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClientScreen.Location = new System.Drawing.Point(0, 0);
            this.ClientScreen.Name = "ClientScreen";
            this.ClientScreen.Size = new System.Drawing.Size(1119, 566);
            this.ClientScreen.TabIndex = 0;
            this.ClientScreen.TabStop = false;
            // 
            // btnExitFullscreen
            // 
            this.btnExitFullscreen.Location = new System.Drawing.Point(455, 12);
            this.btnExitFullscreen.Name = "btnExitFullscreen";
            this.btnExitFullscreen.Size = new System.Drawing.Size(127, 23);
            this.btnExitFullscreen.TabIndex = 1;
            this.btnExitFullscreen.Text = "Exit Fullscreen";
            this.btnExitFullscreen.UseVisualStyleBackColor = true;
            this.btnExitFullscreen.Click += new System.EventHandler(this.btnExitFullscreen_Click);
            // 
            // frmOneToOne
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 566);
            this.Controls.Add(this.btnExitFullscreen);
            this.Controls.Add(this.ClientScreen);
            this.Name = "frmOneToOne";
            this.Text = "frmFullScreenClient";
            this.Resize += new System.EventHandler(this.frmOneToOne_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.ClientScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox ClientScreen;
        private System.Windows.Forms.Button btnExitFullscreen;

    }
}