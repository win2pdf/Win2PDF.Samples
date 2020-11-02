Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics



Module PDFSplitPages

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 1 Then 'the only parameter is the PDF file name
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then 'ignore if not PDF
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
                        'splitpagesdelete command line documented at: https://www.win2pdf.com/doc/command-line-split-pages-pdf.html
                        'change the 3rd parameter to specify a different page increment for the split
                        Dim arguments As String = String.Format("splitpagesdelete ""{0}"" ""{1}"" 1 0", args(0), Path.GetDirectoryName(args(0)))

                        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                        With startInfo
                            .Arguments = arguments
                            .WindowStyle = ProcessWindowStyle.Hidden
                        End With

                        'execute the splitpages command line to split the PDF into individual files
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit()
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments, newProc.ExitCode))
                            End If
                        End If
                    Else
                        MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
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
