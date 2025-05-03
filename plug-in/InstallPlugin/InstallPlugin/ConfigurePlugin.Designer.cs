using System;

namespace InstallPlugin
{
    partial class ConfigurePlugin
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
            this.lblPlugIn = new System.Windows.Forms.Label();
            this.chkPrinter = new System.Windows.Forms.CheckBox();
            this.chkBatch = new System.Windows.Forms.CheckBox();
            this.chkWatch = new System.Windows.Forms.CheckBox();
            this.rdoCurrentUser = new System.Windows.Forms.RadioButton();
            this.rdoAllUsers = new System.Windows.Forms.RadioButton();
            this.lblApplyTo = new System.Windows.Forms.Label();
            this.txtPlugIn = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPlugIn
            // 
            this.lblPlugIn.AutoSize = true;
            this.lblPlugIn.Location = new System.Drawing.Point(18, 32);
            this.lblPlugIn.Name = "lblPlugIn";
            this.lblPlugIn.Size = new System.Drawing.Size(74, 13);
            this.lblPlugIn.TabIndex = 0;
            this.lblPlugIn.Text = "Plug In Name:";
            // 
            // chkPrinter
            // 
            this.chkPrinter.AutoSize = true;
            this.chkPrinter.Location = new System.Drawing.Point(21, 63);
            this.chkPrinter.Name = "chkPrinter";
            this.chkPrinter.Size = new System.Drawing.Size(122, 17);
            this.chkPrinter.TabIndex = 1;
            this.chkPrinter.Text = "Install Printer Plug In";
            this.chkPrinter.UseVisualStyleBackColor = true;
            // 
            // chkBatch
            // 
            this.chkBatch.AutoSize = true;
            this.chkBatch.Location = new System.Drawing.Point(21, 86);
            this.chkBatch.Name = "chkBatch";
            this.chkBatch.Size = new System.Drawing.Size(160, 17);
            this.chkBatch.TabIndex = 2;
            this.chkBatch.Text = "Install Batch Convert Plug In";
            this.chkBatch.UseVisualStyleBackColor = true;
            // 
            // chkWatch
            // 
            this.chkWatch.AutoSize = true;
            this.chkWatch.Location = new System.Drawing.Point(21, 109);
            this.chkWatch.Name = "chkWatch";
            this.chkWatch.Size = new System.Drawing.Size(164, 17);
            this.chkWatch.TabIndex = 3;
            this.chkWatch.Text = "Install Watch Convert Plug In";
            this.chkWatch.UseVisualStyleBackColor = true;
            // 
            // rdoCurrentUser
            // 
            this.rdoCurrentUser.AutoSize = true;
            this.rdoCurrentUser.Location = new System.Drawing.Point(21, 162);
            this.rdoCurrentUser.Name = "rdoCurrentUser";
            this.rdoCurrentUser.Size = new System.Drawing.Size(84, 17);
            this.rdoCurrentUser.TabIndex = 4;
            this.rdoCurrentUser.TabStop = true;
            this.rdoCurrentUser.Text = "Current User";
            this.rdoCurrentUser.UseVisualStyleBackColor = true;
            // 
            // rdoAllUsers
            // 
            this.rdoAllUsers.AutoSize = true;
            this.rdoAllUsers.Location = new System.Drawing.Point(21, 185);
            this.rdoAllUsers.Name = "rdoAllUsers";
            this.rdoAllUsers.Size = new System.Drawing.Size(66, 17);
            this.rdoAllUsers.TabIndex = 5;
            this.rdoAllUsers.TabStop = true;
            this.rdoAllUsers.Text = "All Users";
            this.rdoAllUsers.UseVisualStyleBackColor = true;
            // 
            // lblApplyTo
            // 
            this.lblApplyTo.AutoSize = true;
            this.lblApplyTo.Location = new System.Drawing.Point(21, 140);
            this.lblApplyTo.Name = "lblApplyTo";
            this.lblApplyTo.Size = new System.Drawing.Size(52, 13);
            this.lblApplyTo.TabIndex = 6;
            this.lblApplyTo.Text = "Apply To:";
            // 
            // txtPlugIn
            // 
            this.txtPlugIn.Enabled = false;
            this.txtPlugIn.Location = new System.Drawing.Point(95, 29);
            this.txtPlugIn.Name = "txtPlugIn";
            this.txtPlugIn.Size = new System.Drawing.Size(285, 20);
            this.txtPlugIn.TabIndex = 7;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(188, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 30);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(287, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ConfigurePlugin
            // 
            this.ClientSize = new System.Drawing.Size(402, 274);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPlugIn);
            this.Controls.Add(this.lblApplyTo);
            this.Controls.Add(this.rdoAllUsers);
            this.Controls.Add(this.rdoCurrentUser);
            this.Controls.Add(this.chkWatch);
            this.Controls.Add(this.chkBatch);
            this.Controls.Add(this.chkPrinter);
            this.Controls.Add(this.lblPlugIn);
            this.Name = "ConfigurePlugin";
            this.Text = "Install Win2PDF Plug In";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPlugIn;
        private System.Windows.Forms.CheckBox chkPrinter;
        private System.Windows.Forms.CheckBox chkBatch;
        private System.Windows.Forms.CheckBox chkWatch;
        private System.Windows.Forms.RadioButton rdoCurrentUser;
        private System.Windows.Forms.RadioButton rdoAllUsers;
        private System.Windows.Forms.Label lblApplyTo;
        private System.Windows.Forms.TextBox txtPlugIn;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

        public EventHandler chkBatch_CheckedChanged { get; private set; }
    }
}