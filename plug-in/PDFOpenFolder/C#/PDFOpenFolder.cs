using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


static class PDFOpenFolder
{
    // The Main method serves as the entry point of the application.
    // It expects a single command-line argument representing a file path.
    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            // Check if exactly one argument is provided
            if (args.Length == 1)
            {
                // Extract the directory path from the provided file path
                // and open it in Windows Explorer.
                Process.Start(Path.GetDirectoryName(args[0]));
            }
            else
            {
                // Display an error message if the number of arguments is invalid.
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Format the exception details for logging and display.
            var exception_description = string.Format(
                "Win2PDF plug-in exception {0}, stack {1}, targetsite {2}",
                ex.Message,
                ex.StackTrace,
                ex.TargetSite
            );

            // Show the exception details in a message box to the user.
            MessageBox.Show(exception_description);

            // Log the exception details to the Windows Event Log.
            using (EventLog eventLog = new EventLog("Application"))
            {
                // Set the source of the event log entry.
                eventLog.Source = "Win2PDF";

                // Write the exception details as an error entry in the event log.
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
