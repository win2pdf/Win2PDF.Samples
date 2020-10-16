using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFMakeSearchable
{
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
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
                    string truncated_file = args[0] + ".pdf";

                    // enclose the file names in quotes in case they contain spaces
                    // Make the PDF searchable in place. command line documented at: https://www.win2pdf.com/doc/command-line-make-searcheable-ocr-pdf.html
                    string arguments = string.Format("makesearchable \"{0}\" \"{1}\"", args[0], args[0]);

                    ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                    {
                        var withBlock = startInfo;
                        withBlock.Arguments = arguments;
                        withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    // execute the deletepages command
                    newProc = System.Diagnostics.Process.Start(startInfo);
                    newProc.WaitForExit();
                    if (newProc.HasExited)
                    {
                        if (newProc.ExitCode != 0)
                            MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                    }
                }
                else
                    MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
            }
            else
                MessageBox.Show("Invalid number of parameters");
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