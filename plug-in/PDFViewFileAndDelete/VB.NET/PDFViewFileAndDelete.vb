Imports System.IO
Imports System.Windows

Module PDFViewFileAndDelete
    ' Constant for the error code indicating a sharing violation
    Const ERROR_SHARING_VIOLATION As Integer = 32

    Sub Main(ByVal args() As String)
        Try
            ' Check if exactly one argument (the PDF file name) is provided
            If args.Length = 1 Then
                ' Verify if the specified file exists
                If File.Exists(args(0)) Then
                    ' Create a new process to open the PDF file
                    Dim process = New Process With {
                        .StartInfo = New ProcessStartInfo With {
                           .FileName = args(0) ' Set the file name to the provided argument
                        }
                    }
                    process.Start() ' Start the process to open the file

                    ' Check if the process has already exited
                    If process.HasExited Then
                        ' Poll until the file can be deleted (in case it's locked by another process)
                        Do While True
                            Try
                                File.Delete(args(0)) ' Attempt to delete the file
                                Exit Do ' Exit the loop if successful
                            Catch ex As System.IO.IOException
                                ' Check if the error is due to a sharing violation
                                If (ex.HResult And &HFFFF) = ERROR_SHARING_VIOLATION Then
                                    Threading.Thread.Sleep(1000) ' Wait for 1 second before retrying
                                Else
                                    Exit Do ' Exit the loop for any other IOException
                                End If
                            Catch ex As Exception
                                ' Exit the loop for any other type of exception
                                Exit Do
                            End Try
                        Loop
                    Else
                        ' Wait for the process to exit if it is still running
                        process.WaitForExit()
                        ' Delete the file after the viewer is closed
                        File.Delete(args(0))
                    End If
                End If
            Else
                ' Show a message if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle any unexpected exceptions
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description) ' Display the exception details in a message box

            ' Log the exception details to the Windows Event Log
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
