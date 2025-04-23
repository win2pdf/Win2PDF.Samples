Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFSplitPages

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
                        ' Prepare the command line arguments for splitting the PDF into individual pages
                        ' The "splitpagesdelete" command splits the PDF into segments based on the page increment
                        ' and deletes the source file after successful splitting.
                        ' Parameters:
                        ' - "sourcefile": The input PDF file to split
                        ' - "destfolder": The folder where the split files will be saved
                        ' - "pageincrement": Number of pages per split segment (set to 1 here for single-page splits)
                        ' - "oddpages": Set to 0 (not used in this case)
                        Dim arguments As String = String.Format("splitpagesdelete ""{0}"" ""{1}"" 1 0", args(0), Path.GetDirectoryName(args(0)))

                        ' Configure the process start information
                        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                        With startInfo
                            .Arguments = arguments
                            .WindowStyle = ProcessWindowStyle.Hidden ' Run the process in a hidden window
                        End With

                        ' Start the process to execute the splitpagesdelete command
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit() ' Wait for the process to complete

                        ' Check the exit code to determine if the process succeeded
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                ' Display an error message if the command line execution failed
                                MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode))
                            End If
                        End If
                    Else
                        ' Display an error message if the Win2PDF executable is not found
                        MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            Else
                ' Display an error message if the number of arguments is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle any exceptions that occur during execution
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)

            ' Log the exception details to the Windows Event Log
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
