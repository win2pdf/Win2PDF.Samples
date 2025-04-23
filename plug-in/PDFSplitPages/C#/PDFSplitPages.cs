using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFSplitPages
{
    public static void Main(string[] args)
    {
        try
        {
            // Ensure exactly one argument is passed (the PDF file path)
            if (args.Length == 1)
            {
                // Check if the provided file has a .PDF extension (case-insensitive)
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

                    // Check if the Win2PDF executable exists at the determined path
                    if (File.Exists(win2pdfcmdline))
                    {
                        // Prepare the command-line arguments for splitting the PDF
                        // Syntax of the "splitpagesdelete" command:
                        // splitpagesdelete "sourcefile" "destfolder" pageincrement oddpages
                        // - "sourcefile": The full path to the PDF file to be split.
                        // - "destfolder": The folder where the split PDF files will be saved.
                        // - pageincrement: The number of pages per split file. For example, 1 means each file will contain 1 page.
                        // - oddpages: Set to 1 to handle odd pages differently (e.g., for duplex printing with a cover page). Set to 0 for normal splitting.
                        // The "splitpagesdelete" variation deletes the source file after splitting is complete.
                        string arguments = string.Format("splitpagesdelete \"{0}\" \"{1}\" 1 0", args[0], Path.GetDirectoryName(args[0]));

                        // Configure the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments; // Command-line arguments
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden; // Run the process in a hidden window
                        }

                        // Start the process to execute the Win2PDF command
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit(); // Wait for the process to complete

                        // Check the exit code of the process to determine success or failure
                        if (newProc.HasExited)
                        {
                            if (newProc.ExitCode != 0)
                            {
                                // Display an error message if the command failed
                                MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                            }
                        }
                    }
                    else
                    {
                        // Display an error message if the Win2PDF executable is not found
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                    }
                }
            }
            else
            {
                // Display an error message if the number of arguments is invalid
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during execution
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            MessageBox.Show(exception_description);

            // Log the exception details to the Windows Event Log
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
