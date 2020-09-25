Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics



Module PDFApplyMultipleWatermarks

    Private Const FIRST_PAGE_WATERMARK As String = "..\\..\\..\\letterhead-watermark.pdf"
    Private Const REMAINING_PAGES_WATERMARK As String = "..\\..\\..\\confidential-watermark.pdf"

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 1 Then 'the only parameter is the PDF file name
                Dim newProc As Diagnostics.Process
                Dim win2pdfcmdline = Environment.SystemDirectory

                'get the path to the Win2PDF command line executable
                If Environment.Is64BitOperatingSystem Then
                    win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                Else
                    win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                End If

                If File.Exists(win2pdfcmdline) Then

                    'enclose the file names in quotes in case they contain spaces
                    'watermark command line documented at: https://www.win2pdf.com/doc/command-line-watermark-pdf.html
                    'apply letterhead watermark to only first page
                    Debug.Assert(File.Exists(FIRST_PAGE_WATERMARK))
                    Dim arguments1 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark -1 0", args(0), FIRST_PAGE_WATERMARK, args(0))

                    'apply confidential watermark to all pages except the first page
                    Debug.Assert(File.Exists(REMAINING_PAGES_WATERMARK))
                    Dim arguments2 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark 1 0", args(0), REMAINING_PAGES_WATERMARK, args(0))

                    Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                    With startInfo
                        .Arguments = arguments1
                        .WindowStyle = ProcessWindowStyle.Hidden
                    End With

                    'execute the watermark command line for the first page
                    newProc = Diagnostics.Process.Start(startInfo)
                    newProc.WaitForExit()
                    If newProc.HasExited Then
                        If newProc.ExitCode <> 0 Then
                            MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode))
                        Else
                            startInfo.Arguments = arguments2
                            'execute the watermark command line for the remaining pages
                            newProc = Diagnostics.Process.Start(startInfo)
                            newProc.WaitForExit()
                            If newProc.HasExited Then
                                If newProc.ExitCode <> 0 Then
                                    MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode))
                                End If
                            End If
                        End If
                    End If
                Else
                    MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
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
