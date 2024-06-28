using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFAppendFile
{
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string APPEND_FILE_SETTING = "Plug-In Append File";

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            string append_file = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, APPEND_FILE_SETTING, "");
            if (args.Length == 0 || append_file == "" ||  !File.Exists(append_file) ) // configure append file
            {
                string filePath = string.Empty;

                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

                openFileDialog.Title = "Select PDF to Append";
                if (append_file.Length > 0)
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(append_file);
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
                }

                openFileDialog.Filter = "PDF files(*.pdf)|*.pdf";
                openFileDialog.RestoreDirectory = true;

                if ((openFileDialog.ShowDialog() == DialogResult.OK))
                {
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, APPEND_FILE_SETTING, openFileDialog.FileName);
                }
            }

            if (args.Length == 1)
            {
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF") //ignore if not PDF
                {
                    System.Diagnostics.Process newProc;
                    var win2pdfcmdline = Environment.SystemDirectory;

                    // get the path to the Win2PDF command line executable
                    if (System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) == "ARM64")
                    {
                        win2pdfcmdline += @"\spool\drivers\arm64\3\win2pdfd.exe";
                    }
                    else if (Environment.Is64BitOperatingSystem)
                    {
                        win2pdfcmdline += @"\spool\drivers\x64\3\win2pdfd.exe";
                    }
                    else
                    {
                        win2pdfcmdline += @"\spool\drivers\w32x86\3\win2pdfd.exe";
                    }

                    if (File.Exists(win2pdfcmdline))
                    {

                        // enclose the file names in quotes in case they contain spaces
                        // command line documented at: https://www.win2pdf.com/doc/command-line-append-pdf.html
                        string arguments = string.Format("append \"{0}\" \"{1}\" \"{0}\"", args[0], append_file);

                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments;
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                        }

                        // execute the append command
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit();
                        if (newProc.HasExited)
                        {
                            if (newProc.ExitCode != 0)
                            {
                                MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Incorrect append file type {0}"), args[0]);
                }
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
