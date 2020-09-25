using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFImageOnlyFlatten
{
    public const string WIN2PDF_FLATTEN_TEMPFILE = "Win2PDFFlattenTemp.pdf";

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {
                string newfile = Path.GetDirectoryName(args[0]) + @"\" + WIN2PDF_FLATTEN_TEMPFILE;
                if ((!args[0].ToString().Equals(newfile)))
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
                        // copy original PDF file to temporary name, and use original PDF file as destination
                        if (File.Exists(newfile))
                            File.Delete(newfile);
                        File.Move(args[0], newfile);

                        // enclose the file names in quotes in case they contain spaces, 
                        // use ".pdfc" as destination to force "PDF Image Only (color)"
                        // change destination to ".pdfi" to force "PDF Image Only (monochrome)"
                        // use "Win2Image" printer so the plug-in isn't called recursively
                        string arguments = string.Format("printpdf \"{0}\" \"{1}\" \"{2}\"", newfile, "Win2Image", args[0] + "c");

                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments;
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                        }

                        // execute the printpdf command line to create an Image Only PDF
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit();
                        if (newProc.HasExited)
                        {
                            // delete temp file
                            if (File.Exists(newfile))
                                File.Delete(newfile);

                            if (newProc.ExitCode != 0)
                                MessageBox.Show(string.Format("Win2PDF command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                        }
                    }
                    else
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                }
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
