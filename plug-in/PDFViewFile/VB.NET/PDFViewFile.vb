Imports System.Windows


Module PDFViewFile

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 1 Then 'the only parameter is the PDF file name
                Process.Start(args(0))
            Else
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
