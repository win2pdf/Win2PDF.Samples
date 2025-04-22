Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFDeletePages

    ' Entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Check if exactly one argument is passed (the PDF file name)
            If args.Length = 1 Then
                ' Ensure the file has a .PDF extension (case-insensitive)
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then

                    Dim newProc As Diagnostics.Process
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    ' Determine the path to the Win2PDF command line executable based on system architecture
                    If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
                        win2pdfcmdline += "\spool\drivers\arm64\3\win2pdfd.exe"
                    ElseIf Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    ' Check if the Win2PDF executable exists
                    If File.Exists(win2pdfcmdline) Then
                        Dim truncated_file As String = args(0) + ".pdf"

                        ' Prepare the command line arguments to delete pages 2 to 9999 from the PDF
                        ' Command format: win2pdfd.exe deletepages "sourcefile" startpage endpage "destfile"
                        ' - "sourcefile": The path to the original PDF file to modify.
                        ' - "startpage": The first page to delete (inclusive).
                        ' - "endpage": The last page to delete (inclusive). If the value exceeds the total number of pages, it deletes up to the last page.
                        ' - "destfile": The path to save the modified PDF. If "sourcefile" and "destfile" are the same, the file is modified in place.
                        ' Enclose file names in quotes to handle spaces in file paths.
                        Dim arguments As String = String.Format("deletepages ""{0}"" 2 9999 ""{1}""", args(0), truncated_file)

                        ' Configure the process start information
                        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                        With startInfo
                            .Arguments = arguments
                            .WindowStyle = ProcessWindowStyle.Hidden ' Run the process in hidden mode
                        End With

                        ' Execute the deletepages command
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit() ' Wait for the process to complete

                        ' Check the exit code of the process
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                ' If the command fails (e.g., no pages to delete), ignore the error
                                ' This could happen if the PDF has only one page
                                ' MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode))
                            Else
                                ' If the command succeeds, replace the original file with the truncated file
                                If File.Exists(args(0)) Then
                                    File.Delete(args(0)) ' Delete the original file
                                End If
                                File.Move(truncated_file, args(0)) ' Rename the truncated file to the original file name
                            End If
                        End If
                    Else
                        ' Display an error message if Win2PDF is not installed
                        MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            Else
                ' Display an error message if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle any exceptions and log the error details
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
