using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;


static class PDFViewFile
{

    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    Process.Start(args[0]);
                }
            }
            else
            { 
                MessageBox.Show("Invalid number of parameters");
            }
        }
        catch (Exception ex)
        {
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
