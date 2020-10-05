using System;
using System.Diagnostics;
using System.IO;
using Outlook = NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;



static class PDFSendToOutlook
{
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1)
            {

                // Create an Outlook application.
                Outlook._Application oApp;
                oApp = new Outlook.Application();

                // Create a new MailItem.
                Outlook._MailItem oMsg;
                oMsg = (Outlook._MailItem)oApp.CreateItem((OlItemType)NetOffice.OutlookApi.Enums.OlItemType.olMailItem);

                // set the subject to the PDF file name without the path + attached
                oMsg.Subject = Path.GetFileName(args[0]) + " attached";

                // customize these fields to add "To" address or body
                // oMsg.To = 
                // oMsg.Body =

                // Add the PDF as an attachment
                string sSource = args[0];
                string sDisplayName = Path.GetFileName(args[0]);
                string sBodyLen = "0";

                Outlook.Attachments oAttachs = oMsg.Attachments;
                Outlook.Attachment oAttach;
                oAttach = oAttachs.Add(sSource, OlAttachmentType.olByValue, Convert.ToInt32(sBodyLen) + 1, sDisplayName);

                // Display
                oMsg.Display(true);

                // Send
                // oMsg.Send()

                // close OutlookApi And dispose
                oApp.Quit();
                oApp.Dispose();

                // Clean up
                oApp = null/* TODO Change to default(_) if this is not a reference type */;
                oMsg = null/* TODO Change to default(_) if this is not a reference type */;
                oAttach = null/* TODO Change to default(_) if this is not a reference type */;
                oAttachs = null/* TODO Change to default(_) if this is not a reference type */;
            }
        }
        catch (Exception ex)
        {
            var exception_description = string.Format("Win2PDF Send To Outlook plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite);
            System.Windows.Forms.MessageBox.Show(exception_description);
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Win2PDF";
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101);
            }
        }
    }
}
