namespace Hms.AwsConsole
{
    partial class FormMain
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtMonitor = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.hMSInfrastructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servicesSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(39, 562);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtMonitor
            // 
            this.txtMonitor.Location = new System.Drawing.Point(12, 138);
            this.txtMonitor.Name = "txtMonitor";
            this.txtMonitor.Size = new System.Drawing.Size(1210, 506);
            this.txtMonitor.TabIndex = 1;
            this.txtMonitor.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hMSInfrastructureToolStripMenuItem,
            this.servicesSystemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1255, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // hMSInfrastructureToolStripMenuItem
            // 
            this.hMSInfrastructureToolStripMenuItem.Name = "hMSInfrastructureToolStripMenuItem";
            this.hMSInfrastructureToolStripMenuItem.Size = new System.Drawing.Size(119, 20);
            this.hMSInfrastructureToolStripMenuItem.Text = "HMS Infrastructure";
            // 
            // servicesSystemToolStripMenuItem
            // 
            this.servicesSystemToolStripMenuItem.Name = "servicesSystemToolStripMenuItem";
            this.servicesSystemToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.servicesSystemToolStripMenuItem.Text = "Services System";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1255, 731);
            this.Controls.Add(this.txtMonitor);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "HMS AWS Console";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox txtMonitor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem hMSInfrastructureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servicesSystemToolStripMenuItem;
    }
}

