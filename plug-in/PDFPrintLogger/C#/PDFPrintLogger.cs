using System;
using System.Diagnostics;



static class PDFPrintLogger
{
    const string EVENTSOURCE = "Win2PDF";

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = EVENTSOURCE;
                    string entry = string.Format("File {0} was created by Win2PDF", args[0]);
                    eventLog.WriteEntry(entry, EventLogEntryType.Information);
                }
            }
            else
                System.Windows.Forms.MessageBox.Show("Invalid number of parameters");
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            System.Windows.Forms.MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
