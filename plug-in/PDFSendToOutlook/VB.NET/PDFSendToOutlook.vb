Imports System.IO
Imports System.Windows
Imports Outlook = NetOffice.OutlookApi
Imports NetOffice.OutlookApi.Enums

' This module handles sending a PDF file as an email attachment using Outlook.
Module PDFSendToOutlook

    ' Entry point of the application.
    ' Expects a single argument: the path to the PDF file to be sent.
    Sub Main(ByVal args() As String)
        Try
            ' Ensure only one argument (the PDF file path) is passed.
            If args.Length = 1 Then ' Win2PDF should only pass 1 parameter to the plug-in - the PDF file

                ' Create an instance of the Outlook application.
                Dim oApp As Outlook._Application
                oApp = New Outlook.Application()

                ' Create a new email (MailItem) in Outlook.
                Dim oMsg As Outlook._MailItem
                oMsg = CType(oApp.CreateItem(CType(NetOffice.OutlookApi.Enums.OlItemType.olMailItem, OlItemType)), Outlook._MailItem)

                ' Set the email subject to the PDF file name (without the path) and append "attached".
                oMsg.Subject = Path.GetFileName(args(0)) + " attached"

                ' Optional: Customize the "To" address and email body.
                ' Uncomment and set these fields as needed.
                ' oMsg.To = 
                ' oMsg.Body =

                ' Add the PDF file as an attachment to the email.
                Dim sSource As String = args(0) ' Path to the PDF file.
                Dim sDisplayName As String = Path.GetFileName(args(0)) ' File name for display in the email.
                Dim sBodyLen As String = "0" ' Placeholder for body length (not used here).

                Dim oAttachs As Outlook.Attachments = oMsg.Attachments
                Dim oAttach As Outlook.Attachment
                oAttach = oAttachs.Add(sSource, OlAttachmentType.olByValue, Convert.ToInt32(sBodyLen) + 1, sDisplayName)

                ' Display the email to the user for review.
                oMsg.Display(True)

                ' Optional: Send the email automatically (commented out for safety).
                ' oMsg.Send()

                ' Close the Outlook application and release resources.
                oApp.Quit()
                oApp.Dispose()

                ' Clean up object references to free memory.
                oApp = Nothing
                oMsg = Nothing
                oAttach = Nothing
                oAttachs = Nothing

            End If

        Catch ex As System.Exception
            ' Handle exceptions by logging the error and showing a message box.
            Dim exception_description = String.Format("Win2PDF Send To Outlook plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
