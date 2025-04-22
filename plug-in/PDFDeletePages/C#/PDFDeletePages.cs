using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFDeletePages
{
    public static void Main(string[] args)
    {
        try
        {
            // Check if exactly one argument is passed
            if (args.Length == 1)
            {
                // Ensure the file has a .PDF extension (case-insensitive)
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF")
                {
                    System.Diagnostics.Process newProc;
                    var win2pdfcmdline = Environment.SystemDirectory;

                    // Determine the path to the Win2PDF command line executable based on system architecture
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
                        string truncated_file = args[0] + ".pdf";

                        // Prepare the command line arguments to delete pages 2 through 9999
                        // Syntax: win2pdfd.exe deletepages "sourcefile" startpage endpage "destfile"
                        // - "sourcefile": The path to the original PDF file (can be a local file or a URL).
                        // - startpage: The first page to delete (inclusive).
                        // - endpage: The last page to delete (inclusive).
                        // - "destfile": The path to save the modified PDF file. If the same as "sourcefile", the file is modified in place.
                        // Note: File names with spaces must be enclosed in quotes.
                        string arguments = string.Format("deletepages \"{0}\" 2 9999 \"{1}\"", args[0], truncated_file);

                        // Configure the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments; // Set the command line arguments
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden; // Run the process in hidden mode
                        }

                        // Execute the deletepages command
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit(); // Wait for the process to complete

                        if (newProc.HasExited)
                        {
                            // If the process exits successfully, replace the original file with the truncated file
                            if (newProc.ExitCode == 0)
                            {
                                if (File.Exists(args[0]))
                                    File.Delete(args[0]); // Delete the original file
                                File.Move(truncated_file, args[0]); // Rename the truncated file to the original file name
                            }
                            // If the process fails, it might be due to no pages being deleted (e.g., a 1-page PDF)
                            // The error is ignored in this case
                        }
                    }
                    else
                    {
                        // Display an error message if Win2PDF is not installed
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
            // Handle any exceptions and log the error details
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            MessageBox.Show(exception_description);

            // Log the exception to the Windows Event Log
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
