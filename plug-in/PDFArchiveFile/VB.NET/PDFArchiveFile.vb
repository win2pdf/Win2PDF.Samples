Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFArchiveFile
    ' Constants for application settings
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const ARCHIVE_FOLDER_SETTING As String = "Archive PDF Folder"
    Public Const APPNAME As String = "Win2PDF Archive File Plug-in"

    ' Method to append a PDF file to another PDF file
    ' Parameters:
    ' - appendfile: The file to append
    ' - destfile: The destination file to which the appendfile will be added
    Sub AppendPDF(appendfile As String, destfile As String)
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
            Dim destFileTemp = destfile + ".pdf"

            ' Construct the command line arguments for appending PDFs
            ' Syntax: win2pdfd.exe append "file1" "file2" "mergedfile"
            ' - file1: The destination file
            ' - file2: The file to append
            ' - mergedfile: The resulting merged file
            Dim arguments1 As String = String.Format("append ""{0}"" ""{1}"" ""{2}""", destfile, appendfile, destFileTemp)

            ' Configure the process to execute the command line
            Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
            With startInfo
                .Arguments = arguments1
                .WindowStyle = ProcessWindowStyle.Hidden
            End With

            ' Execute the append command
            newProc = Diagnostics.Process.Start(startInfo)
            newProc.WaitForExit()

            ' Check the process exit code to determine success or failure
            If newProc.HasExited Then
                If newProc.ExitCode <> 0 Then
                    ' Display an error message if the append operation fails
                    MessageBox.Show(String.Format("Win2PDF archive append command line failed: {0} {1}, error code {2}", win2pdfcmdline, arguments1, newProc.ExitCode))
                Else
                    ' Replace the original destination file with the merged file
                    If File.Exists(destFileTemp) Then
                        File.Delete(destfile)
                        File.Move(destFileTemp, destfile)
                    Else
                        MessageBox.Show(String.Format("Win2PDF archive append failed."))
                    End If
                End If
            End If
        Else
            ' Display a message if Win2PDF is not installed
            MessageBox.Show(String.Format("Win2PDF is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
        End If
    End Sub

    ' Main entry point of the application
    ' Handles different scenarios based on the number of arguments passed
    Sub Main(ByVal args() As String)
        Try
            If args.Length = 0 Then
                ' No arguments: Open the folder selection dialog to configure the backup folder
                Dim bkfolder As New ChooseBackupFolder
                bkfolder.ShowDialog()
            ElseIf args.Length = 1 Then
                ' One argument: Process the provided PDF file
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then
                    ' Retrieve the archive folder path from application settings
                    Dim arcfolder As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, ARCHIVE_FOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup\")

                    ' Ensure the archive folder path ends with a backslash
                    If Not arcfolder.EndsWith("\") Then arcfolder += "\"
                    ' Create the archive folder if it does not exist
                    If Not Directory.Exists(arcfolder) Then Directory.CreateDirectory(arcfolder)

                    ' Check if the provided PDF file exists
                    If File.Exists(args(0)) Then
                        ' Generate the destination file name based on the current date
                        Dim destfile As String = arcfolder + DateTime.Now.ToString("yyyy-MM-dd") + ".pdf"

                        ' If the destination file exists, append the new file to it
                        If File.Exists(destfile) Then
                            AppendPDF(args(0), destfile)
                        Else
                            ' Otherwise, copy the file to the destination
                            File.Copy(args(0), destfile)
                        End If
                    End If
                End If
            Else
                ' Invalid number of arguments
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle exceptions and log them to the Windows Event Log
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
