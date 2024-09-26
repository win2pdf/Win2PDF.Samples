using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;


static class PDFOpenFolder
{
    [STAThreadAttribute]
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {
                //open the folder using Windows Explorer
                Process.Start(Path.GetDirectoryName(args[0]));
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
