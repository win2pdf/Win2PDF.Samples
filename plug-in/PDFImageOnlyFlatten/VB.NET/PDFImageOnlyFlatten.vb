Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics



Module PDFImageOnlyFlatten

    Public Const WIN2PDF_FLATTEN_TEMPFILE As String = "Win2PDFFlattenTemp.pdf"

    Sub Main(ByVal args() As String)
        Try

            If args.Length = 1 Then 'the only parameter is the PDF file name
                Dim newfile As String = Path.GetDirectoryName(args(0)) + "\" + WIN2PDF_FLATTEN_TEMPFILE
                If (Not args(0).ToString.Equals(newfile)) Then 'skip recursive call from printpdf command line
                    Dim newProc As Diagnostics.Process
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    'get the path to the Win2PDF command line executable
                    If Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    'copy original PDF file to temporary name, and use original PDF file as destination
                    If File.Exists(newfile) Then
                        File.Delete(newfile)
                    End If
                    File.Move(args(0), newfile)

                    'enclose the file names in quotes in case they contain spaces, 
                    'use ".pdfc" as destination to force "PDF Image Only (color)"
                    'change destination to ".pdfi" to force "PDF Image Only (monochrome)"
                    'use "Win2Image" printer so the plug-in isn't called recursively
                    Dim arguments As String = String.Format("printpdf ""{0}"" ""{1}"" ""{2}""", newfile, "Win2Image", args(0) + "c")

                    Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                    With startInfo
                        .Arguments = arguments
                        .WindowStyle = ProcessWindowStyle.Hidden
                    End With

                    'execute the printpdf command line to create an Image Only PDF
                    newProc = Diagnostics.Process.Start(startInfo)
                    newProc.WaitForExit()
                    If newProc.HasExited Then
                        'delete temp file
                        If File.Exists(newfile) Then
                            File.Delete(newfile)
                        End If

                        If newProc.ExitCode <> 0 Then
                            MessageBox.Show(String.Format("Win2PDF command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode))
                        End If
                    End If
                End If
            Else
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite), EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
