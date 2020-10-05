Imports System.IO
Imports System.Windows
Imports Outlook = NetOffice.OutlookApi
Imports NetOffice.OutlookApi.Enums



Module PDFSendToOutlook

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 1 Then 'Win2PDF should only pass 1 parameter to the plug-in - the PDF file

                ' Create an Outlook application.
                Dim oApp As Outlook._Application
                oApp = New Outlook.Application()

                ' Create a new MailItem.
                Dim oMsg As Outlook._MailItem
                oMsg = CType(oApp.CreateItem(CType(NetOffice.OutlookApi.Enums.OlItemType.olMailItem, OlItemType)), Outlook._MailItem)

                'set the subject to the PDF file name without the path + attached
                oMsg.Subject = Path.GetFileName(args(0)) + " attached"

                'customize these fields to add "To" address or body
                'oMsg.To = 
                'oMsg.Body =

                ' Add the PDF as an attachment
                Dim sSource As String = args(0)
                Dim sDisplayName As String = Path.GetFileName(args(0))
                Dim sBodyLen As String = "0"

                Dim oAttachs As Outlook.Attachments = oMsg.Attachments
                Dim oAttach As Outlook.Attachment
                oAttach = oAttachs.Add(sSource, OlAttachmentType.olByValue, Convert.ToInt32(sBodyLen) + 1, sDisplayName)

                ' Display
                oMsg.Display(True)

                ' Send
                'oMsg.Send()

                'close OutlookApi And dispose
                oApp.Quit()
                oApp.Dispose()

                ' Clean up
                oApp = Nothing
                oMsg = Nothing
                oAttach = Nothing
                oAttachs = Nothing

            End If

        Catch ex As System.Exception
            Dim exception_description = String.Format("Win2PDF Send To Outlook plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
