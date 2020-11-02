Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.Windows.Forms

Module PDFApplyMultipleWatermarks

    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const FIRST_PAGE_WATERMARK_SETTING As String = "First Page Watermark"
    Public Const REMAINING_PAGE_WATERMARK_SETTING As String = "Remaining Page Watermark"

    Sub Main(ByVal args() As String)
        Try
            Dim first_page_watermark As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, "")
            Dim remaining_page_watermark As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, "")

            If args.Length = 0 OrElse first_page_watermark = "" OrElse remaining_page_watermark = "" _
                OrElse Not File.Exists(first_page_watermark) OrElse Not File.Exists(remaining_page_watermark) Then 'configure watermark files
                Dim filePath As String = String.Empty

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                openFileDialog.Title = "Select First Page Watermark File"
                If first_page_watermark.Length > 0 Then
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(first_page_watermark)
                Else
                    openFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                End If

                openFileDialog.Filter = "PDF files(*.pdf)|*.pdf"
                openFileDialog.RestoreDirectory = True

                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, openFileDialog.FileName)
                    openFileDialog.Title = "Select Watermark File For Remaining Pages"
                    If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, openFileDialog.FileName)
                    End If
                End If
            End If

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
                        'watermark command line documented at: https://www.win2pdf.com/doc/command-line-watermark-pdf.html
                        'apply watermark to only first page
                        Debug.Assert(File.Exists(first_page_watermark))
                        Dim arguments1 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark -1 0", args(0), first_page_watermark, args(0))

                        'apply watermark to all pages except the first page
                        Debug.Assert(File.Exists(remaining_page_watermark))
                        Dim arguments2 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark 1 0", args(0), remaining_page_watermark, args(0))

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
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode))
                            Else
                                startInfo.Arguments = arguments2
                                'execute the watermark command line for the remaining pages
                                newProc = Diagnostics.Process.Start(startInfo)
                                newProc.WaitForExit()
                                If newProc.HasExited Then
                                    If newProc.ExitCode <> 0 Then
                                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode))
                                    End If
                                End If
                            End If
                        End If
                    Else
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            End If
        Catch ex As Exception
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.Forms.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
