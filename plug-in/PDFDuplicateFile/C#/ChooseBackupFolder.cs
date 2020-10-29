using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PDFChooseBackupFolder
{
    public partial class ChooseBackupFolder : Form
    {
        public ChooseBackupFolder()
        {
            InitializeComponent();
        }

        private void CenterForm(Form frm, Form parent = null/* TODO Change to default(_) if this is not a reference type */)
        {
            System.Drawing.Rectangle r;
            if (parent != null)
                r = parent.RectangleToScreen(parent.ClientRectangle);
            else
                r = Screen.FromPoint(frm.Location).WorkingArea;

            var x = r.Left + (r.Width - frm.Width) / 2;
            var y = r.Top + (r.Height - frm.Height) / 2;
            frm.Location = new System.Drawing.Point(x, y);
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtBackupFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Interaction.SaveSetting(PDFDuplicateFile.WIN2PDF_COMPANY, PDFDuplicateFile.WIN2PDF_PRODUCT, PDFDuplicateFile.DUPFOLDER_SETTING, txtBackupFolder.Text);
            this.Close();
        }

        private void ChooseBackupFolder_Load(object sender, EventArgs e)
        {
            txtBackupFolder.Text = Interaction.GetSetting(PDFDuplicateFile.WIN2PDF_COMPANY, PDFDuplicateFile.WIN2PDF_PRODUCT, PDFDuplicateFile.DUPFOLDER_SETTING, Environment.SpecialFolder.MyDocuments + @"\backup");
            CenterForm(this);
        }
    }
}


//namespace PDFDuplicateFile
//{
//    public partial class ChooseBackupFolder : Form
//    {
//        public ChooseBackupFolder()
//        {
//            InitializeComponent();
//        }
//    }
//}

