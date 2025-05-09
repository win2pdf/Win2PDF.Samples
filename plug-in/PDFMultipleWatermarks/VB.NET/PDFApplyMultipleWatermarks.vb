﻿Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.Windows.Forms

Module PDFApplyMultipleWatermarks

    ' Constants for application settings
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const FIRST_PAGE_WATERMARK_SETTING As String = "First Page Watermark"
    Public Const REMAINING_PAGE_WATERMARK_SETTING As String = "Remaining Page Watermark"

    Sub Main(ByVal args() As String)
        Try
            ' Retrieve saved watermark file paths from application settings
            Dim first_page_watermark As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, "")
            Dim remaining_page_watermark As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, "")

            ' If no arguments are provided or watermark files are not configured, prompt the user to select them
            If args.Length = 0 OrElse first_page_watermark = "" OrElse remaining_page_watermark = "" _
                OrElse Not File.Exists(first_page_watermark) OrElse Not File.Exists(remaining_page_watermark) Then

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                ' Prompt user to select the first page watermark file
                openFileDialog.Title = "Select First Page Watermark File"
                If first_page_watermark.Length > 0 Then
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(first_page_watermark)
                Else
                    openFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                End If
                openFileDialog.Filter = "PDF files(*.pdf)|*.pdf"
                openFileDialog.RestoreDirectory = True

                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    ' Save the selected first page watermark file path
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, FIRST_PAGE_WATERMARK_SETTING, openFileDialog.FileName)

                    ' Prompt user to select the watermark file for remaining pages
                    openFileDialog.Title = "Select Watermark File For Remaining Pages"
                    If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                        ' Save the selected remaining page watermark file path
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, REMAINING_PAGE_WATERMARK_SETTING, openFileDialog.FileName)
                    End If
                End If
            End If

            ' If a single argument is provided, assume it is the PDF file to process
            If args.Length = 1 Then
                ' Ensure the file has a .PDF extension
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
                        ' watermark command line documented at: https://www.win2pdf.com/doc/command-line-watermark-pdf.html
                        ' Apply watermark to the first page
                        ' Command format: win2pdfd.exe watermark "sourcefile" "watermarkfile" "destfile" mode excludepre excludepost
                        ' Explanation of parameters:
                        ' - "sourcefile": The input PDF file to which the watermark will be applied.
                        ' - "watermarkfile": The PDF file used as the watermark.
                        ' - "destfile": The output PDF file with the watermark applied. If the same as "sourcefile", the file is modified in place.
                        ' - "mode": Specifies how the watermark is applied. "watermark" overlays the watermark on top of the content.
                        ' - "excludepre": Number of pages to skip from the beginning. Negative values include the watermark on skipped pages.
                        ' - "excludepost": Number of pages to skip from the end. Negative values include the watermark on skipped pages.
                        Debug.Assert(File.Exists(first_page_watermark))
                        Dim arguments1 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark -1 0", args(0), first_page_watermark, args(0))

                        ' Apply watermark to all pages except the first page
                        ' Here, "excludepre" is set to 1 to skip the first page, and "excludepost" is 0 to apply to all remaining pages.
                        Debug.Assert(File.Exists(remaining_page_watermark))
                        Dim arguments2 As String = String.Format("watermark ""{0}"" ""{1}"" ""{2}"" watermark 1 0", args(0), remaining_page_watermark, args(0))

                        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                        With startInfo
                            .Arguments = arguments1
                            .WindowStyle = ProcessWindowStyle.Hidden
                        End With

                        ' Execute the watermark command for the first page
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit()
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                ' Display error if the command fails
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode))
                            Else
                                ' Execute the watermark command for the remaining pages
                                startInfo.Arguments = arguments2
                                newProc = Diagnostics.Process.Start(startInfo)
                                newProc.WaitForExit()
                                If newProc.HasExited Then
                                    If newProc.ExitCode <> 0 Then
                                        ' Display error if the command fails
                                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0} {1}, error code {2}", win2pdfcmdline, arguments2, newProc.ExitCode))
                                    End If
                                End If
                            End If
                        End If
                    Else
                        ' Display error if Win2PDF is not installed
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle exceptions and log them to the Windows Event Log
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.Forms.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module