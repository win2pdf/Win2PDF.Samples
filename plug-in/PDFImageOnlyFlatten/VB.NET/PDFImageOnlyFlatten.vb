Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFImageOnlyFlatten

    ' Constant for the temporary file name used during processing
    Public Const WIN2PDF_FLATTEN_TEMPFILE As String = "Win2PDFFlattenTemp.pdf"

    ' Main entry point of the application
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
                        ' Use the Win2PDF command line "imagepdf" command: https://www.win2pdf.com/doc/command-line-pdf-image-only.html
                        '  win2pdfd.exe imagepdf "sourcefile" "destfile" colormode
                        ' Prepare the command line arguments for the "imagepdf" command
                        ' The "imagepdf" command converts the PDF to an image-only PDF
                        ' Arguments: source file, destination file (same as source here), and color mode
                        ' This example uses "color" for the colormode, but you can change to "mono" or "grayscale"
                        ' Enclose the file names in quotes in case they contain spaces.
                        Dim arguments As String = String.Format("imagepdf ""{0}"" ""{1}"" color", args(0), args(0))

                        ' Configure the process start information
                        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                        With startInfo
                            .Arguments = arguments
                            .WindowStyle = ProcessWindowStyle.Hidden ' Run the process in a hidden window
                        End With

                        ' Start the process and wait for it to complete
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit()

                        ' Check the exit code to determine if the process succeeded
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                ' Display an error message if the process failed
                                MessageBox.Show(String.Format("Win2PDF command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode))
                            End If
                        End If
                    Else
                        ' Display a message if Win2PDF is not installed
                        MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            Else
                ' Display a message if the number of parameters is invalid
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
