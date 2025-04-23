using System;
using System.Diagnostics;



// Static class to log events related to PDF creation using Win2PDF
static class PDFPrintLogger
{
    // Constant for the event source name
    const string EVENTSOURCE = "Win2PDF";

    public static void Main(string[] args)
    {
        try
        {
            // Check if exactly one argument is passed
            if (args.Length == 1)
            {
                // Log the event to the Windows Application Event Log
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = EVENTSOURCE;

                    // Create a log entry indicating the file creation
                    string entry = string.Format("File {0} was created by Win2PDF", args[0]);
                    eventLog.WriteEntry(entry, EventLogEntryType.Information);
                }
            }
            else
            {
                // Show a message box if the number of parameters is invalid
                System.Windows.Forms.MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions and log the error details
            var exception_description = string.Format(
                "Win2PDF plug-in exception {0}, stack {1}, targetsite {2}",
                ex.Message, ex.StackTrace, ex.TargetSite
            );

            // Display the exception details in a message box
            System.Windows.Forms.MessageBox.Show(exception_description);

            // Log the exception details to the Windows Application Event Log
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
