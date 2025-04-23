using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

static class PDFImageOnlyFlatten
{
    // Constant for the temporary file name used during processing
    public const string WIN2PDF_FLATTEN_TEMPFILE = "Win2PDFFlattenTemp.pdf";

    public static void Main(string[] args)
    {
        try
        {
            // Ensure exactly one argument is passed (the PDF file path)
            if (args.Length == 1)
            {
                // Check if the provided file has a .PDF extension
                if (Path.GetExtension(args[0]).ToUpper() == ".PDF") // Ignore if not a PDF
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
                        // Use the Win2PDF command line "imagepdf" command: https://www.win2pdf.com/doc/command-line-pdf-image-only.html
                        //  win2pdfd.exe imagepdf "sourcefile" "destfile" colormode
                        // Prepare the command line arguments for the "imagepdf" command
                        // This command converts the input PDF to an image-only PDF
                        // "color" is used as the color mode, but it can be changed to "mono" or "grayscale"
                        // Enclose the file names in quotes in case they contain spaces.
                        string arguments = string.Format("imagepdf \"{0}\" \"{1}\" color", args[0], args[0]);

                        // Configure the process start information
                        ProcessStartInfo startInfo = new ProcessStartInfo(win2pdfcmdline);
                        {
                            var withBlock = startInfo;
                            withBlock.Arguments = arguments; // Command line arguments
                            withBlock.WindowStyle = ProcessWindowStyle.Hidden; // Run the process in a hidden window
                        }

                        // Start the process to create an image-only PDF
                        newProc = System.Diagnostics.Process.Start(startInfo);
                        newProc.WaitForExit(); // Wait for the process to complete

                        // Check the exit code to determine if the process succeeded
                        if (newProc.HasExited)
                        {
                            if (newProc.ExitCode != 0)
                            {
                                // Display an error message if the process failed
                                MessageBox.Show(string.Format("Win2PDF command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode));
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
                // Display a message if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log the error details
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
