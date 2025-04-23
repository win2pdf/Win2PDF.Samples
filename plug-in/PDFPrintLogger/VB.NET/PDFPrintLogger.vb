Imports System.Windows

' Module to log PDF creation events using the Windows Event Log
Module PDFPrintLogger

    ' Constant for the event source name
    Const EVENTSOURCE = "Win2PDF"

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Check if exactly one argument is passed (the PDF file name)
            If args.Length = 1 Then
                ' Log the event in the Windows Event Log
                Using eventLog As EventLog = New EventLog("Application")
                    eventLog.Source = EVENTSOURCE
                    ' Create a log entry indicating the file was created
                    Dim entry As String = String.Format("File {0} was created by Win2PDF", args(0))
                    eventLog.WriteEntry(entry, EventLogEntryType.Information)
                End Using
            Else
                ' Show a message box if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle any exceptions that occur
            ' Create a detailed exception description
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            ' Display the exception details in a message box
            MessageBox.Show(exception_description)
            ' Log the exception details in the Windows Event Log
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
