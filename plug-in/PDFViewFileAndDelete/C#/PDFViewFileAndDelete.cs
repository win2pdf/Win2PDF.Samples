using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;


static class PDFViewFileAndDelete
{
    // Error code for file sharing violation
    const int ERROR_SHARING_VIOLATION = 32;

    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            // Ensure exactly one argument is passed (the file path)
            if (args.Length == 1)
            {
                // Check if the specified file exists
                if (File.Exists(args[0]))
                {
                    // Create a new process to open the file
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = args[0] // File to be opened
                        }
                    };

                    // Start the process
                    process.Start();

                    // Check if the process has already exited
                    if (process.HasExited)
                    {
                        // Poll until the file is no longer in use, then delete it
                        while (true)
                        {
                            try
                            {
                                File.Delete(args[0]); // Attempt to delete the file
                                break; // Exit the loop if successful
                            }
                            catch (IOException ex)
                            {
                                // Handle file sharing violation by waiting and retrying
                                if ((ex.HResult & 0xFFFF) == ERROR_SHARING_VIOLATION)
                                    System.Threading.Thread.Sleep(1000); // Wait 1 second
                                else
                                    break; // Exit for any other IO exception
                            }
                            catch
                            {
                                break; // Exit for any other exception
                            }
                        }
                    }
                    else
                    {
                        // Wait for the process to exit
                        process.WaitForExit();

                        // Delete the file after the viewer is closed
                        File.Delete(args[0]);
                    }
                }
            }
            else
            {
                // Show an error message if the number of arguments is invalid
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle unexpected exceptions
            var exception_description = string.Format(
                "Win2PDF plug-in exception {0}, stack {1}, targetsite {2}",
                ex.Message, ex.StackTrace, ex.TargetSite
            );

            // Display the exception details in a message box
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
