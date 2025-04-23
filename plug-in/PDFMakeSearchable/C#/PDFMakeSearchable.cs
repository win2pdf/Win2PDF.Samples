using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFMakeSearchable
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
                        // Prepare the command line arguments to make the PDF searchable
                        // Syntax: win2pdfd.exe makesearchable "sourcefile" "destfile"
                        // - "sourcefile": The path to the input PDF file. Can be a local file path or a URL.
                        // - "destfile": The path to the output PDF file. If the same as "sourcefile", the file is modified in place.
                        // - The command uses Optical Character Recognition (OCR) to add an invisible text layer to the PDF.
                        // - Returns 0 on success, or a Windows system error code on failure.
                        string arguments = string.Format("makesearchable \"{0}\" \"{1}\"", args[0], args[0]);

                        // Configure the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments; // Set the command line arguments
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden; // Run the process in a hidden window
                        }

                        // Start the process and wait for it to complete
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit();

                        // Check the exit code of the process
                        if (newProc.HasExited)
                        {
                            if (newProc.ExitCode != 0)
                            {
                                // Display an error message if the process failed
                                MessageBox.Show(string.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
                            }
                        }
                    }
                    else
                    {
                        // Display a message if Win2PDF is not installed
                        MessageBox.Show(string.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"));
                    }
                }
            }
            else
            {
                // Display a message if the number of arguments is invalid
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
