namespace SLAM
{
    partial class MainForm
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
            this.streamBox = new System.Windows.Forms.PictureBox();
            this.sceneBox = new System.Windows.Forms.PictureBox();
            this.logBox = new System.Windows.Forms.ListBox();
            this.cameraInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.streamBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sceneBox)).BeginInit();
            this.SuspendLayout();
            // 
            // streamBox
            // 
            this.streamBox.Location = new System.Drawing.Point(984, 12);
            this.streamBox.Name = "streamBox";
            this.streamBox.Size = new System.Drawing.Size(270, 480);
            this.streamBox.TabIndex = 0;
            this.streamBox.TabStop = false;
            // 
            // sceneBox
            // 
            this.sceneBox.BackColor = System.Drawing.Color.White;
            this.sceneBox.Location = new System.Drawing.Point(12, 12);
            this.sceneBox.Name = "sceneBox";
            this.sceneBox.Size = new System.Drawing.Size(966, 611);
            this.sceneBox.TabIndex = 1;
            this.sceneBox.TabStop = false;
            // 
            // logBox
            // 
            this.logBox.FormattingEnabled = true;
            this.logBox.Location = new System.Drawing.Point(984, 502);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(270, 121);
            this.logBox.TabIndex = 2;
            // 
            // cameraInfo
            // 
            this.cameraInfo.AutoSize = true;
            this.cameraInfo.Location = new System.Drawing.Point(984, 12);
            this.cameraInfo.Name = "cameraInfo";
            this.cameraInfo.Size = new System.Drawing.Size(35, 13);
            this.cameraInfo.TabIndex = 3;
            this.cameraInfo.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1266, 635);
            this.Controls.Add(this.cameraInfo);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.sceneBox);
            this.Controls.Add(this.streamBox);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.streamBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sceneBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox streamBox;
        private System.Windows.Forms.PictureBox sceneBox;
        private System.Windows.Forms.ListBox logBox;
        private System.Windows.Forms.Label cameraInfo;

    }
}

