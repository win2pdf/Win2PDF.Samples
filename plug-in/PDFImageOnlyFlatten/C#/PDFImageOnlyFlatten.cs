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
                        //Use the Win2PDF command line "imagepdf" command: https://www.win2pdf.com/doc/command-line-pdf-image-only.html
                        //  win2pdfd.exe imagepdf "sourcefile" "destfile" colormode
                        //This example uses "color" for the colormode, but you can change to "mono" or "grayscale"
                        //Enclose the file names in quotes in case they contain spaces.
                        string arguments = string.Format("imagepdf \"{0}\" \"{1}\" color", args[0], args[0]);

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
                            if (newProc.ExitCode != 0)
                            {
                                MessageBox.Show(string.Format("Win2PDF command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
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
