using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PDFChooseBackupFolder
{
    public partial class ChooseBackupFolder : Form
    {
        // Constructor to initialize the form
        public ChooseBackupFolder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Centers the given form on the screen or relative to a parent form.
        /// </summary>
        /// <param name="frm">The form to center.</param>
        /// <param name="parent">The parent form to center relative to (optional).</param>
        private void CenterForm(Form frm, Form parent = null/* TODO Change to default(_) if this is not a reference type */)
        {
            System.Drawing.Rectangle r;

            // Determine the area to center the form within
            if (parent != null)
                r = parent.RectangleToScreen(parent.ClientRectangle);
            else
                r = Screen.FromPoint(frm.Location).WorkingArea;

            // Calculate the centered position
            var x = r.Left + (r.Width - frm.Width) / 2;
            var y = r.Top + (r.Height - frm.Height) / 2;

            // Set the form's location to the calculated position
            frm.Location = new System.Drawing.Point(x, y);
        }

        /// <summary>
        /// Opens a folder browser dialog to allow the user to select a backup folder.
        /// </summary>
        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

            // Show the folder browser dialog and set the selected path to the text box
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtBackupFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// Closes the form when the Cancel button is clicked.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Saves the selected backup folder path to the application settings and closes the form.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save the selected folder path to the application settings
            Interaction.SaveSetting(PDFArchiveFile.WIN2PDF_COMPANY, PDFArchiveFile.WIN2PDF_PRODUCT, PDFArchiveFile.ARCHIVE_FOLDER_SETTING, txtBackupFolder.Text);

            // Close the form
            this.Close();
        }

        /// <summary>
        /// Loads the form, initializes the backup folder path, and centers the form.
        /// </summary>
        private void ChooseBackupFolder_Load(object sender, EventArgs e)
        {
            // Retrieve the saved backup folder path or use a default path
            txtBackupFolder.Text = Interaction.GetSetting(PDFArchiveFile.WIN2PDF_COMPANY, PDFArchiveFile.WIN2PDF_PRODUCT, PDFArchiveFile.ARCHIVE_FOLDER_SETTING, Environment.SpecialFolder.MyDocuments + @"\backup");

            // Center the form on the screen
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

