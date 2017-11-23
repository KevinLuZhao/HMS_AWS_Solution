﻿namespace Hms.AwsConsole
{
    partial class FormHmsSystem
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ddlWebServerAMI = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboxLevel2 = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboxLevel1 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCreateRDS = new System.Windows.Forms.Button();
            this.txtMonitor = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsComboEnv = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tsComboRegion = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.tsComboColor = new System.Windows.Forms.ToolStripComboBox();
            this.btnCreateVpcConnection = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(7, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1117, 251);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ddlWebServerAMI);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.cboxLevel2);
            this.tabPage1.Controls.Add(this.btnCreate);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cboxLevel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1109, 225);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Application Infrastructure";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ddlWebServerAMI
            // 
            this.ddlWebServerAMI.FormattingEnabled = true;
            this.ddlWebServerAMI.Location = new System.Drawing.Point(138, 119);
            this.ddlWebServerAMI.Name = "ddlWebServerAMI";
            this.ddlWebServerAMI.Size = new System.Drawing.Size(121, 21);
            this.ddlWebServerAMI.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Web Server AMI";
            // 
            // cboxLevel2
            // 
            this.cboxLevel2.AutoSize = true;
            this.cboxLevel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxLevel2.Location = new System.Drawing.Point(36, 95);
            this.cboxLevel2.Name = "cboxLevel2";
            this.cboxLevel2.Size = new System.Drawing.Size(68, 17);
            this.cboxLevel2.TabIndex = 3;
            this.cboxLevel2.Text = "Level 2";
            this.cboxLevel2.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(29, 202);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Create VPC, Public Subnet, Private Subnet, Gateways and Route Tables";
            // 
            // cboxLevel1
            // 
            this.cboxLevel1.AutoSize = true;
            this.cboxLevel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboxLevel1.Location = new System.Drawing.Point(36, 23);
            this.cboxLevel1.Name = "cboxLevel1";
            this.cboxLevel1.Size = new System.Drawing.Size(68, 17);
            this.cboxLevel1.TabIndex = 0;
            this.cboxLevel1.Text = "Level 1";
            this.cboxLevel1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.btnCreateRDS);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1109, 225);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Database Infrastructure";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "label3";
            // 
            // btnCreateRDS
            // 
            this.btnCreateRDS.Location = new System.Drawing.Point(19, 184);
            this.btnCreateRDS.Name = "btnCreateRDS";
            this.btnCreateRDS.Size = new System.Drawing.Size(119, 23);
            this.btnCreateRDS.TabIndex = 0;
            this.btnCreateRDS.Text = "Create Database Infra";
            this.btnCreateRDS.UseVisualStyleBackColor = true;
            this.btnCreateRDS.Click += new System.EventHandler(this.btnCreateRDS_Click);
            // 
            // txtMonitor
            // 
            this.txtMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMonitor.Location = new System.Drawing.Point(7, 314);
            this.txtMonitor.Name = "txtMonitor";
            this.txtMonitor.Size = new System.Drawing.Size(1113, 311);
            this.txtMonitor.TabIndex = 1;
            this.txtMonitor.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsComboEnv,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.tsComboRegion,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.tsComboColor});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1127, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabel1.Text = "Environment: ";
            // 
            // tsComboEnv
            // 
            this.tsComboEnv.Name = "tsComboEnv";
            this.tsComboEnv.Size = new System.Drawing.Size(110, 25);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(50, 22);
            this.toolStripLabel2.Text = "Region: ";
            // 
            // tsComboRegion
            // 
            this.tsComboRegion.Name = "tsComboRegion";
            this.tsComboRegion.Size = new System.Drawing.Size(150, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(42, 22);
            this.toolStripLabel3.Text = "Color: ";
            // 
            // tsComboColor
            // 
            this.tsComboColor.Name = "tsComboColor";
            this.tsComboColor.Size = new System.Drawing.Size(75, 25);
            // 
            // btnCreateVpcConnection
            // 
            this.btnCreateVpcConnection.Location = new System.Drawing.Point(12, 285);
            this.btnCreateVpcConnection.Name = "btnCreateVpcConnection";
            this.btnCreateVpcConnection.Size = new System.Drawing.Size(148, 23);
            this.btnCreateVpcConnection.TabIndex = 6;
            this.btnCreateVpcConnection.Text = "Create Peering Connection";
            this.btnCreateVpcConnection.UseVisualStyleBackColor = true;
            this.btnCreateVpcConnection.Click += new System.EventHandler(this.btnCreateVpcConnection_Click);
            // 
            // FormHmsSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 631);
            this.Controls.Add(this.btnCreateVpcConnection);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.txtMonitor);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormHmsSystem";
            this.Text = "HMS System Builder";
            this.Load += new System.EventHandler(this.FormHmsSystem_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox txtMonitor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cboxLevel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tsComboEnv;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox tsComboRegion;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox tsComboColor;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.CheckBox cboxLevel2;
        private System.Windows.Forms.ComboBox ddlWebServerAMI;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCreateRDS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCreateVpcConnection;
    }
}