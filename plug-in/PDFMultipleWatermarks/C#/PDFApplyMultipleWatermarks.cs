using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFApplyMultipleWatermarks
{
    private const string FIRST_PAGE_WATERMARK = @"..\\..\\..\\letterhead-watermark.pdf";
    private const string REMAINING_PAGES_WATERMARK = @"..\\..\\..\\confidential-watermark.pdf";

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

                    // enclose the file names in quotes in case they contain spaces
                    // watermark command line documented at: https://www.win2pdf.com/doc/command-line-watermark-pdf.html
                    // apply letterhead watermark to only first page
                    Debug.Assert(File.Exists(FIRST_PAGE_WATERMARK));
                    string arguments1 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark -1 0", args[0], FIRST_PAGE_WATERMARK, args[0]);

                    // apply confidential watermark to all pages except the first page
                    Debug.Assert(File.Exists(REMAINING_PAGES_WATERMARK));
                    string arguments2 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark 1 0", args[0], REMAINING_PAGES_WATERMARK, args[0]);

                    ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                    {
                        var withBlock = startInfo;
                        withBlock.Arguments = arguments1;
                        withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    // execute the watermark command line for the first page
                    newProc = System.Diagnostics.Process.Start(startInfo);
                    newProc.WaitForExit();
                    if (newProc.HasExited)
                    {
                        if (newProc.ExitCode != 0)
                            MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode));
                        else
                        {
                            startInfo.Arguments = arguments2;
                            // execute the watermark command line for the remaining pages
                            newProc = System.Diagnostics.Process.Start(startInfo);
                            newProc.WaitForExit();
                            if (newProc.HasExited)
                            {
                                if (newProc.ExitCode != 0)
                                    MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode));
                            }
                        }
                    }
                }
                else
                    MessageBox.Show(string.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
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
