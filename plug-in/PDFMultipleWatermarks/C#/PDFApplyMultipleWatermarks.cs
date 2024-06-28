using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;

static class PDFApplyMultipleWatermarks
{
    public const  string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const  string WIN2PDF_PRODUCT = "Win2PDF";
    public const  string FIRST_PAGE_WATERMARK_SETTING = "First Page Watermark";
    public const  string REMAINING_PAGE_WATERMARK_SETTING = "Remaining Page Watermark";

    public static void Main(string[] args)
    {
        try
        {
            string first_page_watermark = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, "");
            string remaining_page_watermark = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, "");

            if (args.Length == 0 || first_page_watermark == "" || remaining_page_watermark == "" || !File.Exists(first_page_watermark) || !File.Exists(remaining_page_watermark))
            {
                string filePath = string.Empty;

                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

                openFileDialog.Title = "Select First Page Watermark File";
                if (first_page_watermark.Length > 0)
                { 
                openFileDialog.InitialDirectory = Path.GetDirectoryName(first_page_watermark);
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
                }

                openFileDialog.Filter = "PDF files(*.pdf)|*.pdf";
                openFileDialog.RestoreDirectory = true;

                if ((openFileDialog.ShowDialog() == DialogResult.OK))
                {
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, openFileDialog.FileName);
                    openFileDialog.Title = "Select Watermark File For Remaining Pages";
                    if ((openFileDialog.ShowDialog() == DialogResult.OK))
                    {
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, openFileDialog.FileName);
                    }
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
                        // watermark command line documented at: https://www.win2pdf.com/doc/command-line-watermark-pdf.html
                        // apply watermark to only first page
                        Debug.Assert(File.Exists(first_page_watermark));
                        string arguments1 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark -1 0", args[0], first_page_watermark, args[0]);

                        // apply watermark to all pages except the first page
                        Debug.Assert(File.Exists(remaining_page_watermark));
                        string arguments2 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark 1 0", args[0], remaining_page_watermark, args[0]);

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
                            {
                                System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode));
                            }
                            else
                            {
                                startInfo.Arguments = arguments2;
                                // execute the watermark command line for the remaining pages
                                newProc = System.Diagnostics.Process.Start(startInfo);
                                newProc.WaitForExit();
                                if (newProc.HasExited)
                                {
                                    if (newProc.ExitCode != 0)
                                    {
                                        System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            System.Windows.Forms.MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
