Imports System.Windows

' Module to handle viewing PDF files
Module PDFViewFile

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Check if exactly one argument (PDF file name) is provided
            If args.Length = 1 Then
                ' Attempt to open the specified PDF file
                Process.Start(args(0))
            Else
                ' Show an error message if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle any exceptions that occur during execution
            ' Format the exception details into a descriptive message
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)

            ' Display the exception details in a message box
            MessageBox.Show(exception_description)

            ' Log the exception details to the Windows Event Log
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
