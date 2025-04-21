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
            // Retrieve the previously saved append file path from the application settings
            string append_file = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, APPEND_FILE_SETTING, "");

            // If no arguments are provided, or the append file is not set or doesn't exist, prompt the user to select a file
            if (args.Length == 0 || append_file == "" || !File.Exists(append_file))
            {
                string filePath = string.Empty;

                // Open a file dialog to allow the user to select a PDF file to append
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Title = "Select PDF to Append",
                    InitialDirectory = append_file.Length > 0 ? Path.GetDirectoryName(append_file) : Environment.SpecialFolder.MyDocuments.ToString(),
                    Filter = "PDF files(*.pdf)|*.pdf",
                    RestoreDirectory = true
                };

                // Save the selected file path to the application settings
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, APPEND_FILE_SETTING, openFileDialog.FileName);
                }
            }

            // If a single argument is provided, process the file
            if (args.Length == 1)
            {
                // Ensure the provided file is a PDF
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF")
                {
                    System.Diagnostics.Process newProc;
                    var win2pdfcmdline = Environment.SystemDirectory;

                    // Determine the path to the Win2PDF command line executable based on the system architecture
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

                    // Check if the Win2PDF executable exists
                    if (File.Exists(win2pdfcmdline))
                    {
                        // Construct the command line arguments for appending the PDF
                        // The format is: append "file1" "file2" "file1"
                        // This appends "file2" (append_file) to "file1" (args[0]) and saves the result back to "file1"
                        string arguments = string.Format("append \"{0}\" \"{1}\" \"{0}\"", args[0], append_file);

                        // Configure the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline)
                        {
                            Arguments = arguments,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };

                        // Execute the append command
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit();

                        // Check the exit code to determine if the operation was successful
                        if (newProc.HasExited && newProc.ExitCode != 0)
                        {
                            MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                        }
                    }
                    else
                    {
                        // Display an error message if Win2PDF is not installed
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                    }
                }
                else
                {
                    // Display an error message if the provided file is not a PDF
                    MessageBox.Show(string.Format("Incorrect append file type {0}", args[0]));
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
