using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using PDFChooseBackupFolder;

// This sample Win2PDF plug-in automatically archives newly created PDF files created by Win2PDF in a designated archive file folder.
// The files are appended to an archive PDF named based on the current date.  The archive file folder can reside on a shared network
// location, or in a cloud backed location such as OneDrive, DropBox, or Google Drive.  Run the "Configure Win2PDF Archive" shortcut
// from the Start menu to configure the archive file folder location.

static class PDFArchiveFile
{
    // Constants for application settings
    public const string WIN2PDF_COMPANY = "Dane Prairie Systems";
    public const string WIN2PDF_PRODUCT = "Win2PDF";
    public const string ARCHIVE_FOLDER_SETTING = "Archive PDF Folder";
    public const string APPNAME = "Win2PDF Archive File Plug-in";

    /// <summary>
    /// Appends the content of one PDF file to another using the Win2PDF command-line tool.
    /// </summary>
    /// <param name="appendfile">The PDF file to append.</param>
    /// <param name="destfile">The destination PDF file.</param>
    public static void AppendPDF(string appendfile, string destfile)
    {
        System.Diagnostics.Process newProc;
        var win2pdfcmdline = Environment.SystemDirectory;

        // Determine the path to the Win2PDF command-line executable based on the system architecture
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
            var destFileTemp = destfile + ".pdf";

            // Construct the command line arguments for appending the PDF
            // The format is: append "file1" "file2" "file1"
            // This appends "file2" (append_file) to "file1" (args[0]) and saves the result back to "file1"
            string arguments1 = string.Format("append \"{0}\" \"{1}\" \"{2}\"", destfile, appendfile, destFileTemp);

            // Configure the process start information
            ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
            {
                var withBlock = startInfo;
                withBlock.Arguments = arguments1;
                withBlock.WindowStyle = ProcessWindowStyle.Hidden;
            }

            // Execute the append command
            newProc = System.Diagnostics.Process.Start(startInfo);
            newProc.WaitForExit();

            // Handle the process result
            if (newProc.HasExited)
            {
                if (newProc.ExitCode != 0)
                {
                    MessageBox.Show(string.Format("Win2PDF archive append command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode));
                }
                else if (File.Exists(destFileTemp))
                {
                    File.Delete(destfile);
                    File.Move(destFileTemp, destfile);
                }
                else
                {
                    MessageBox.Show(string.Format("Win2PDF archive append failed."));
                }
            }
        }
        else
        {
            // Notify the user if Win2PDF is not installed
            MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
        }
    }

    /// <summary>
    /// Main entry point for the application. Handles different command-line arguments.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    [STAThread] // Ensures the main thread is in STA mode
    public static void Main(string[] args)
    {
        try
        {
            // If no arguments are provided, show the ChooseBackupFolder dialog
            if (args.Length == 0)
            {
                ChooseBackupFolder bkfolder = new ChooseBackupFolder();
                bkfolder.ShowDialog();
            }
            // If one argument is provided, process the PDF file
            else if (args.Length == 1)
            {
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF") // Ignore non-PDF files
                {
                    // Retrieve the archive folder path from settings or use a default path
                    string arcfolder = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, ARCHIVE_FOLDER_SETTING, Environment.SpecialFolder.MyDocuments + @"\backup\");

                    // Ensure the folder path ends with a backslash
                    if (!arcfolder.EndsWith(@"\"))
                    {
                        arcfolder += @"\";
                    }

                    // Create the archive folder if it doesn't exist
                    if (!Directory.Exists(arcfolder))
                    {
                        Directory.CreateDirectory(arcfolder);
                    }

                    // Process the PDF file
                    if (File.Exists(args[0]))
                    {
                        string destfile = arcfolder + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf";

                        // If the destination file exists, append the new file to it
                        if (File.Exists(destfile))
                        {
                            AppendPDF(args[0], destfile);
                        }
                        else
                        {
                            // Otherwise, copy the file to the destination
                            File.Copy(args[0], destfile);
                        }
                    }
                }
            }
            else
            {
                // Notify the user if an invalid number of parameters are provided
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log them to the Event Log
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
