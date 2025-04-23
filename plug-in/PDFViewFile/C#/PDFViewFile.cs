using System;
using System.Diagnostics;
using System.Windows.Forms;


static class PDFViewFile
{
    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            // Check if exactly one argument is passed to the application
            if (args.Length == 1)
            {
                // Attempt to open the file specified in the argument using the default application
                Process.Start(args[0]);
            }
            else
            {
                // Show a message box if the number of arguments is invalid
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Format the exception details into a descriptive message
            var exception_description = string.Format(
                "Win2PDF plug-in exception {0}, stack {1}, targetsite {2}",
                ex.Message,
                ex.StackTrace,
                ex.TargetSite
            );

            // Display the exception details in a message box
            MessageBox.Show(exception_description);

            // Log the exception details to the Windows Event Log
            using (EventLog eventLog = new EventLog("Application"))
            {
                // Set the source of the event log entry
                eventLog.Source = "Win2PDF";

                // Write the exception details as an error entry
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
