Imports System.IO
Imports System.Windows


Module PDFViewFileAndDelete
    Const ERROR_SHARING_VIOLATION As Integer = 32

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 1 Then 'the only parameter is the PDF file name

                If File.Exists(args(0)) Then
                    Dim process = New Process With {
                        .StartInfo = New ProcessStartInfo With {
                           .FileName = args(0)
                        }
                    }
                    process.Start()
                    If process.HasExited Then 'an existing process was already started, so poll until the file can be deleted

                        Do While True
                            Try
                                File.Delete(args(0))
                                Exit Do
                            Catch ex As System.IO.IOException
                                If (ex.HResult And &HFFFF) = ERROR_SHARING_VIOLATION Then 'keep polling while the file is open
                                    Threading.Thread.Sleep(1000)
                                Else
                                    Exit Do 'exit for an unknown error
                                End If
                            Catch ex As Exception
                                Exit Do
                            End Try
                        Loop
                    Else
                        process.WaitForExit()
                        'delete file after the viewer is closed
                        File.Delete(args(0))
                    End If
                End If
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
