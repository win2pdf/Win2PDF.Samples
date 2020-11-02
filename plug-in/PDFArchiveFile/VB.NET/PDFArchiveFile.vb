Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics



Module PDFArchiveFile
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const ARCHIVE_FOLDER_SETTING As String = "Archive PDF Folder"
    Public Const APPNAME As String = "Win2PDF Archive File Plug-in"

    Sub AppendPDF(appendfile As String, destfile As String)
        Dim newProc As Diagnostics.Process
        Dim win2pdfcmdline = Environment.SystemDirectory

        'get the path to the Win2PDF command line executable
        If Environment.Is64BitOperatingSystem Then
            win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
        Else
            win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
        End If

        If File.Exists(win2pdfcmdline) Then

            Dim destFileTemp = destfile + ".pdf"
            'enclose the file names in quotes in case they contain spaces
            'watermark command line documented at: https://www.win2pdf.com/doc/command-line-append-pdf.html
            'apply letterhead watermark to only first page
            Dim arguments1 As String = String.Format("append ""{0}"" ""{1}"" ""{2}""", appendfile, destfile, destFileTemp)

            Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
            With startInfo
                .Arguments = arguments1
                .WindowStyle = ProcessWindowStyle.Hidden
            End With

            'execute the append command line 
            newProc = Diagnostics.Process.Start(startInfo)
            newProc.WaitForExit()
            If newProc.HasExited Then
                If newProc.ExitCode <> 0 Then
                    MessageBox.Show(String.Format("Win2PDF archive append command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode))
                Else
                    If File.Exists(destFileTemp) Then
                        File.Delete(destfile)
                        File.Move(destFileTemp, destfile)
                    Else
                        MessageBox.Show(String.Format("Win2PDF archive append failed."))
                    End If
                End If
            End If
        Else
            MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
        End If
    End Sub

    Sub Main(ByVal args() As String)
        Try
            If args.Length = 0 Then 'configure the duplicate file folder
                Dim bkfolder As New ChooseBackupFolder
                bkfolder.ShowDialog()
            ElseIf args.Length = 1 Then 'the only parameter is the PDF file name
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then 'ignore if not PDF
                    Dim arcfolder As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, ARCHIVE_FOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup\")

                    If Not arcfolder.EndsWith("\") Then arcfolder += "\"
                    If Not Directory.Exists(arcfolder) Then Directory.CreateDirectory(arcfolder)

                    If File.Exists(args(0)) Then
                        Dim destfile As String = arcfolder + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf"
                        'overwrite the destination file if it exists
                        If File.Exists(destfile) Then
                            AppendPDF(args(0), destfile)
                        Else
                            File.Copy(args(0), destfile)
                        End If
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
