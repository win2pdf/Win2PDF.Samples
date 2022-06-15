namespace PDFAWSConfig
{
    partial class ConfigureS3
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
            this.txtAccessKey = new System.Windows.Forms.TextBox();
            this.lblAccessKeyID = new System.Windows.Forms.Label();
            this.lblSecretAccessID = new System.Windows.Forms.Label();
            this.txtSecretAccess = new System.Windows.Forms.TextBox();
            this.lblBucketName = new System.Windows.Forms.Label();
            this.txtBucketName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolderName = new System.Windows.Forms.TextBox();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cboRegion = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rdoCopy = new System.Windows.Forms.RadioButton();
            this.rdoMove = new System.Windows.Forms.RadioButton();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAccessKey
            // 
            this.txtAccessKey.Location = new System.Drawing.Point(142, 62);
            this.txtAccessKey.Name = "txtAccessKey";
            this.txtAccessKey.Size = new System.Drawing.Size(214, 20);
            this.txtAccessKey.TabIndex = 0;
            // 
            // lblAccessKeyID
            // 
            this.lblAccessKeyID.AutoSize = true;
            this.lblAccessKeyID.Location = new System.Drawing.Point(21, 65);
            this.lblAccessKeyID.Name = "lblAccessKeyID";
            this.lblAccessKeyID.Size = new System.Drawing.Size(77, 13);
            this.lblAccessKeyID.TabIndex = 1;
            this.lblAccessKeyID.Text = "Access Key ID";
            // 
            // lblSecretAccessID
            // 
            this.lblSecretAccessID.AutoSize = true;
            this.lblSecretAccessID.Location = new System.Drawing.Point(21, 91);
            this.lblSecretAccessID.Name = "lblSecretAccessID";
            this.lblSecretAccessID.Size = new System.Drawing.Size(90, 13);
            this.lblSecretAccessID.TabIndex = 3;
            this.lblSecretAccessID.Text = "Secret Access ID";
            // 
            // txtSecretAccess
            // 
            this.txtSecretAccess.Location = new System.Drawing.Point(142, 88);
            this.txtSecretAccess.Name = "txtSecretAccess";
            this.txtSecretAccess.Size = new System.Drawing.Size(214, 20);
            this.txtSecretAccess.TabIndex = 2;
            // 
            // lblBucketName
            // 
            this.lblBucketName.AutoSize = true;
            this.lblBucketName.Location = new System.Drawing.Point(21, 117);
            this.lblBucketName.Name = "lblBucketName";
            this.lblBucketName.Size = new System.Drawing.Size(72, 13);
            this.lblBucketName.TabIndex = 5;
            this.lblBucketName.Text = "Bucket Name";
            // 
            // txtBucketName
            // 
            this.txtBucketName.Location = new System.Drawing.Point(142, 114);
            this.txtBucketName.Name = "txtBucketName";
            this.txtBucketName.Size = new System.Drawing.Size(214, 20);
            this.txtBucketName.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Folder Name (Optional)";
            // 
            // txtFolderName
            // 
            this.txtFolderName.Location = new System.Drawing.Point(142, 140);
            this.txtFolderName.Name = "txtFolderName";
            this.txtFolderName.Size = new System.Drawing.Size(214, 20);
            this.txtFolderName.TabIndex = 6;
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel1.Controls.Add(this.btnCancel, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.btnOK, 0, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(178, 197);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(181, 30);
            this.TableLayoutPanel1.TabIndex = 8;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnCancel.Location = new System.Drawing.Point(95, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.AutoSize = true;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(8, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cboRegion
            // 
            this.cboRegion.FormattingEnabled = true;
            this.cboRegion.Items.AddRange(new object[] {
            "US East (Ohio)",
            "US East (N. Virginia)",
            "US West (N. California)",
            "US West (Oregon)",
            "Africa (Cape Town)",
            "Asia Pacific (Hong Kong)",
            "Asia Pacific (Jakarta)",
            "Asia Pacific (Mumbai)",
            "Asia Pacific (Osaka)",
            "Asia Pacific (Seoul)",
            "Asia Pacific (Singapore)",
            "Asia Pacific (Sydney)",
            "Asia Pacific (Tokyo)",
            "Canada (Central)",
            "China (Beijing)",
            "China (Ningxia)",
            "Europe (Frankfurt)",
            "Europe (Ireland)",
            "Europe (London)",
            "Europe (Milan)",
            "Europe (Paris)",
            "Europe (Stockholm)",
            "Middle East (Bahrain)",
            "South America (São Paulo)"});
            this.cboRegion.Location = new System.Drawing.Point(142, 167);
            this.cboRegion.Name = "cboRegion";
            this.cboRegion.Size = new System.Drawing.Size(214, 21);
            this.cboRegion.TabIndex = 9;
            this.cboRegion.Text = "US East (N. Virginia)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "AWS Region";
            // 
            // rdoCopy
            // 
            this.rdoCopy.AutoSize = true;
            this.rdoCopy.Location = new System.Drawing.Point(24, 13);
            this.rdoCopy.Name = "rdoCopy";
            this.rdoCopy.Size = new System.Drawing.Size(187, 17);
            this.rdoCopy.TabIndex = 11;
            this.rdoCopy.TabStop = true;
            this.rdoCopy.Text = "Copy PDF to S3 (leave local copy)";
            this.rdoCopy.UseVisualStyleBackColor = true;
            // 
            // rdoMove
            // 
            this.rdoMove.AutoSize = true;
            this.rdoMove.Location = new System.Drawing.Point(24, 36);
            this.rdoMove.Name = "rdoMove";
            this.rdoMove.Size = new System.Drawing.Size(193, 17);
            this.rdoMove.TabIndex = 12;
            this.rdoMove.TabStop = true;
            this.rdoMove.Text = "Move PDF to S3 (delete local copy)";
            this.rdoMove.UseVisualStyleBackColor = true;
            // 
            // ConfigureS3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 238);
            this.Controls.Add(this.rdoMove);
            this.Controls.Add(this.rdoCopy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboRegion);
            this.Controls.Add(this.TableLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolderName);
            this.Controls.Add(this.lblBucketName);
            this.Controls.Add(this.txtBucketName);
            this.Controls.Add(this.lblSecretAccessID);
            this.Controls.Add(this.txtSecretAccess);
            this.Controls.Add(this.lblAccessKeyID);
            this.Controls.Add(this.txtAccessKey);
            this.Name = "ConfigureS3";
            this.Text = "Configure Amazon S3";
            this.Load += new System.EventHandler(this.ConfigureS3_Load);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAccessKey;
        private System.Windows.Forms.Label lblAccessKeyID;
        private System.Windows.Forms.Label lblSecretAccessID;
        private System.Windows.Forms.TextBox txtSecretAccess;
        private System.Windows.Forms.Label lblBucketName;
        private System.Windows.Forms.TextBox txtBucketName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolderName;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cboRegion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdoCopy;
        private System.Windows.Forms.RadioButton rdoMove;
    }
}