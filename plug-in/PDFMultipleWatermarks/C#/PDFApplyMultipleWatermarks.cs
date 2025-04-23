using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;

static class PDFApplyMultipleWatermarks
{
    // Constants for application settings
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string FIRST_PAGE_WATERMARK_SETTING = "First Page Watermark";
    public const string REMAINING_PAGE_WATERMARK_SETTING = "Remaining Page Watermark";

    public static void Main(string[] args)
    {
        try
        {
            // Retrieve saved watermark file paths from application settings
            string first_page_watermark = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, "");
            string remaining_page_watermark = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, "");

            // If no arguments are provided or watermark files are missing, prompt the user to select them
            if (args.Length == 0 || first_page_watermark == "" || remaining_page_watermark == "" || !File.Exists(first_page_watermark) || !File.Exists(remaining_page_watermark))
            {
                string filePath = string.Empty;

                // Open file dialog to select the first page watermark file
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select First Page Watermark File",
                    InitialDirectory = first_page_watermark.Length > 0 ? Path.GetDirectoryName(first_page_watermark) : Environment.SpecialFolder.MyDocuments.ToString(),
                    Filter = "PDF files(*.pdf)|*.pdf",
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Save the selected first page watermark file path
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, openFileDialog.FileName);

                    // Prompt the user to select the watermark file for remaining pages
                    openFileDialog.Title = "Select Watermark File For Remaining Pages";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save the selected remaining page watermark file path
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, openFileDialog.FileName);
                    }
                }
            }

            // If a single argument is provided, process the PDF file
            if (args.Length == 1)
            {
                // Ensure the provided file is a PDF
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF")
                {
                    Process newProc;
                    var win2pdfcmdline = Environment.SystemDirectory;

                    // Determine the path to the Win2PDF command line executable based on system architecture
                    if (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) == "ARM64")
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

                    // Check if the Win2PDF command line executable exists
                    if (File.Exists(win2pdfcmdline))
                    {
                        // Construct command line arguments for applying watermarks
                        // Syntax: win2pdfd.exe watermark "sourcefile" "watermarkfile" "destfile" mode excludepre excludepost
                        // - "sourcefile": The input PDF file to which the watermark will be applied.
                        // - "watermarkfile": The PDF file used as the watermark.
                        // - "destfile": The output PDF file (can be the same as "sourcefile" for in-place modification).
                        // - "mode": Specifies how the watermark is applied:
                        //     - "watermark": Overlay the watermark on top of existing graphics.
                        //     - "background": Place the watermark underneath existing graphics.
                        //     - "watermarklink" or "backgroundlink": Same as above but retains clickable links in the watermark file.
                        // - "excludepre": Number of pages to skip from the beginning of the document.
                        //     - Set to 0 to apply the watermark to all pages, including the first.
                        //     - Set to a negative value to include the watermark on the specified number of pages from the start.
                        // - "excludepost": Number of pages to skip from the end of the document.
                        //     - Set to 0 to apply the watermark to all pages, including the last.
                        //     - Set to a negative value to include the watermark on the specified number of pages from the end.

                        // Apply watermark to only the first page
                        Debug.Assert(File.Exists(first_page_watermark));
                        string arguments1 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark -1 0", args[0], first_page_watermark, args[0]);

                        // Apply watermark to all pages except the first page
                        Debug.Assert(File.Exists(remaining_page_watermark));
                        string arguments2 = string.Format("watermark \"{0}\" \"{1}\" \"{2}\" watermark 1 0", args[0], remaining_page_watermark, args[0]);

                        // Configure process start info for the first page watermark
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline)
                        {
                            Arguments = arguments1,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };

                        // Execute the watermark command for the first page
                        newProc = Process.Start(startInfo);
                        newProc.WaitForExit();
                        if (newProc.HasExited)
                        {
                            if (newProc.ExitCode != 0)
                            {
                                // Display error message if the command fails
                                MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode));
                            }
                            else
                            {
                                // Configure and execute the watermark command for the remaining pages
                                startInfo.Arguments = arguments2;
                                newProc = Process.Start(startInfo);
                                newProc.WaitForExit();
                                if (newProc.HasExited)
                                {
                                    if (newProc.ExitCode != 0)
                                    {
                                        // Display error message if the command fails
                                        MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Display error message if Win2PDF is not installed
                        MessageBox.Show("Win2PDF Pro is not installed. Download Win2PDF at https://www.win2pdf.com/download/");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log them to the Windows Event Log
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
