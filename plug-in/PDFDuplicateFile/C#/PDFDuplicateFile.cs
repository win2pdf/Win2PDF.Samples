using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;


static class PDFDuplicateFile
{
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string DUPFOLDER_SETTING = "Duplicate PDF Folder";
    public const string APPNAME = "Win2PDF Duplicate File Plug-in";


    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            // configure folder location
            if (args.Length == 0)
            {
                PDFChooseBackupFolder.ChooseBackupFolder bkfolder = new PDFChooseBackupFolder.ChooseBackupFolder();
                bkfolder.ShowDialog();
            }
            else if (args.Length == 1)
            {
                string dupfolder = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, DUPFOLDER_SETTING, Environment.SpecialFolder.MyDocuments + @"\backup" );

                if (File.Exists(args[0]))
                {
                    string destfile = Path.GetDirectoryName(dupfolder) + @"\" + Path.GetFileName(args[0]);
                    // overwrite the destination file if it exists
                    if (File.Exists(destfile))
                    {
                        File.Delete(destfile);
                    }
                    File.Copy(args[0], destfile);
                }
            }
            else
            { 
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
