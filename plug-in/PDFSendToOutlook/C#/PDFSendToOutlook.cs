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
            // Check if exactly one argument (PDF file path) is provided
            if (args.Length == 1)
            {
                // Create an instance of the Outlook application
                Outlook._Application oApp;
                oApp = new Outlook.Application();

                // Create a new email (MailItem) in Outlook
                Outlook._MailItem oMsg;
                oMsg = (Outlook._MailItem)oApp.CreateItem((OlItemType)NetOffice.OutlookApi.Enums.OlItemType.olMailItem);

                // Set the email subject to the PDF file name (without path) followed by "attached"
                oMsg.Subject = Path.GetFileName(args[0]) + " attached";

                // Optional: Customize the "To" field and email body
                // oMsg.To = "recipient@example.com";
                // oMsg.Body = "Please find the attached PDF.";

                // Add the PDF file as an attachment to the email
                string sSource = args[0]; // Full path to the PDF file
                string sDisplayName = Path.GetFileName(args[0]); // File name to display in the email
                string sBodyLen = "0"; // Position of the attachment in the email body

                Outlook.Attachments oAttachs = oMsg.Attachments; // Get the attachments collection
                Outlook.Attachment oAttach;
                oAttach = oAttachs.Add(sSource, OlAttachmentType.olByValue, Convert.ToInt32(sBodyLen) + 1, sDisplayName);

                // Display the email to the user for review
                oMsg.Display(true);

                // Optional: Uncomment the following line to send the email automatically
                // oMsg.Send();

                // Close the Outlook application and release resources
                oApp.Quit();
                oApp.Dispose();

                // Clean up references to avoid memory leaks
                oApp = null;
                oMsg = null;
                oAttach = null;
                oAttachs = null;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions by showing a message box and logging the error to the Event Log
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
