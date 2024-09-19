using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

static class PDF2JPG
{
    public const  string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const  string WIN2PDF_PRODUCT = "Win2PDF";
    public const  string JPG_RESOLUTION = "JPGResolution";

    public const  int ERROR_LOCKED = 212;
    public const  int ERROR_INVALID_FUNCTION = 2;
    public const  int ERROR_FILE_NOT_FOUND = 2;
    public const  int ERROR_ACCESS_DENIED = 5;
    public const  int ERROR_BAD_FORMAT = 11;
    public const  int ERROR_INVALID_PARAMETER = 87;
    public const  int ERROR_SUCCESS = 0;

    public static void Main(string[] args)
    {
        try
        {
            string resolution = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, JPG_RESOLUTION, "");

            if (args.Length == 0 || resolution == "")
            {
                resolution = Interaction.InputBox("Enter JPG Resolution:", WIN2PDF_PRODUCT, "100");
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, JPG_RESOLUTION, resolution);
            }
            else if (args.Length == 1)
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
                    string jpgname = System.IO.Path.Combine(Path.GetDirectoryName(args[0]),Path.GetFileNameWithoutExtension(args[0])) + ".jpg";
                    // command line format is: pdffile jpgfile pagenumber xresolution yresolution
                    string arguments1 = string.Format("pdf2jpg \"{0}\" \"{1}\" 0 {2} {3}", args[0], jpgname, resolution, resolution);

                    ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                    {
                        var withBlock = startInfo;
                        withBlock.Arguments = arguments1;
                        withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    // execute the command
                    newProc = System.Diagnostics.Process.Start(startInfo);
                    newProc.WaitForExit();
                    if (newProc.HasExited)
                    {
                        switch (newProc.ExitCode)
                        {
                            case ERROR_SUCCESS:
                                {
                                    //delete the PDF on success
                                    File.Delete(args[0]);
                                    break;
                                }

                            case ERROR_FILE_NOT_FOUND:
                                {
                                    System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF could not find file: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }

                            case ERROR_ACCESS_DENIED:
                                {
                                    System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }

                            default:
                                {
                                    MessageBox.Show(string.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                        }
                    }
                }
                else
                    System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
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
