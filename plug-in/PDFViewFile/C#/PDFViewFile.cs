using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;


static class PDFViewFile
{

    [STAThreadAttribute]
    public void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = args[0]
                        }
                    };
                    process.Start();
                    if (process.HasExited) //if process was already started, poll for file to close
                    {
                        while (true)
                        {
                            try
                            {
                                File.Delete(args[0]);
                                break;
                            }
                            catch (IOException ex)
                            {
                                if ((ex.HResult & 0xFFFF) == ERROR_SHARING_VIOLATION)
                                    System.Threading.Thread.Sleep(1000);
                                else
                                    break; // exit for an unknown error
                            }
                            catch (Exception ex)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        process.WaitForExit();
                        // delete file after the viewer is closed
                        File.Delete(args[0]);
                    }
                }
            }
            else
                MessageBox.Show("Invalid number of parameters");
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
