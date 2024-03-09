using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

static class PDFPrintActualSize
{
    public const  string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const  string WIN2PDF_PRODUCT = "Win2PDF";
    public const  string PRINTER_NAME = "Actual Size Printer";

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
            string printername = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, "");

            if (args.Length == 0 || printername == "")
            {
                Interaction.MsgBox("Select Print Actual Size printer.", MsgBoxStyle.Information, WIN2PDF_PRODUCT);
                System.Windows.Forms.PrintDialog dlgPrint = new System.Windows.Forms.PrintDialog();
                try
                {
                    dlgPrint.AllowSelection = true;
                    dlgPrint.ShowNetwork = true;
                    if (printername.Length > 0)
                    {
                        dlgPrint.PrinterSettings.PrinterName = printername;
                    }
                    
                    if (dlgPrint.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        printername = dlgPrint.PrinterSettings.PrinterName;
                    }
                }
                catch (Exception ex)
                {
                    Interaction.MsgBox("Print Error: " + ex.Message, MsgBoxStyle.Exclamation, WIN2PDF_PRODUCT);
                }

                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, printername);
            }
            else if (args.Length == 1)
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
                    string arguments1 = string.Format("printpdfactualsize \"{0}\" \"{1}\"", args[0], printername);

                    ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                    {
                        var withBlock = startInfo;
                        withBlock.Arguments = arguments1;
                        withBlock.WindowStyle = ProcessWindowStyle.Hidden;
                    }

                    // execute the print direct command
                    newProc = System.Diagnostics.Process.Start(startInfo);
                    newProc.WaitForExit();
                    if (newProc.HasExited)
                    {
                        switch (newProc.ExitCode)
                        {
                            case ERROR_SUCCESS:
                                {
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
