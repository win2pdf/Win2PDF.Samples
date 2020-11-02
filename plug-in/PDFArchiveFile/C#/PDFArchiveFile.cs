using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using PDFChooseBackupFolder;

static class PDFArchiveFile
{
    public const  string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const  string WIN2PDF_PRODUCT = "Win2PDF";
    public const  string ARCHIVE_FOLDER_SETTING = "Archive PDF Folder";
    public const  string APPNAME = "Win2PDF Archive File Plug-in";

    public static void AppendPDF(string appendfile, string destfile)
    {
        System.Diagnostics.Process newProc;
        var win2pdfcmdline = Environment.SystemDirectory;

        // get the path to the Win2PDF command line executable
        if (Environment.Is64BitOperatingSystem)
            win2pdfcmdline += @"\spool\drivers\x64\3\win2pdfd.exe";
        else
            win2pdfcmdline += @"\spool\drivers\w32x86\3\win2pdfd.exe";

        if (File.Exists(win2pdfcmdline))
        {
            var destFileTemp = destfile + ".pdf";
            // enclose the file names in quotes in case they contain spaces
            // watermark command line documented at: https://www.win2pdf.com/doc/command-line-append-pdf.html
            // apply letterhead watermark to only first page
            string arguments1 = string.Format("append \"{0}\" \"{1}\" \"{2}\"", appendfile, destfile, destFileTemp);

            ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
            {
                var withBlock = startInfo;
                withBlock.Arguments = arguments1;
                withBlock.WindowStyle = ProcessWindowStyle.Hidden;
            }

            // execute the append command line 
            newProc = System.Diagnostics.Process.Start(startInfo);
            newProc.WaitForExit();
            if (newProc.HasExited)
            {
                if (newProc.ExitCode != 0)
                {
                    MessageBox.Show(string.Format("Win2PDF archive append command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode));
                }
                else if (File.Exists(destFileTemp))
                {
                    File.Delete(destfile);
                    File.Move(destFileTemp, destfile);
                }
                else
                {
                    MessageBox.Show(string.Format("Win2PDF archive append failed."));
                }
            }
        }
        else
        {
            MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
        }
    }

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                ChooseBackupFolder bkfolder = new ChooseBackupFolder();
                bkfolder.ShowDialog();
            }
            else if (args.Length == 1)
            {
                string arcfolder = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, ARCHIVE_FOLDER_SETTING, Environment.SpecialFolder.MyDocuments + @"\backup\");

                if (!arcfolder.EndsWith(@"\"))
                {
                    arcfolder += @"\";
                }
                if (!Directory.Exists(arcfolder))
                {
                    Directory.CreateDirectory(arcfolder);
                }

                if (File.Exists(args[0]))
                {
                    string destfile = arcfolder + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";
                    // overwrite the destination file if it exists
                    if (File.Exists(destfile))
                    {
                        AppendPDF(args[0], destfile);
                    }
                    else
                    {
                        File.Copy(args[0], destfile);
                    }
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
