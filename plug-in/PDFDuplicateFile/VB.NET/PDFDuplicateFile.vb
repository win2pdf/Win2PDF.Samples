Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics

Module PDFDuplicateFile
    ' Constants for application settings and metadata
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const DUPFOLDER_SETTING As String = "Duplicate PDF Folder"
    Public Const APPNAME As String = "Win2PDF Duplicate File Plug-in"

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Check if no arguments are passed (configure the duplicate file folder)
            If args.Length = 0 Then
                Dim bkfolder As New ChooseBackupFolder
                bkfolder.ShowDialog() ' Open a dialog to choose the backup folder

                ' Check if exactly one argument is passed (PDF file name)
            ElseIf args.Length = 1 Then
                ' Retrieve the duplicate folder path from application settings or use a default path
                Dim dupfolder As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, DUPFOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup")

                ' Ensure the folder path ends with a backslash
                If Not dupfolder.EndsWith("\") Then dupfolder += "\"

                ' Create the folder if it does not exist
                If Not Directory.Exists(dupfolder) Then Directory.CreateDirectory(dupfolder)

                ' Check if the specified PDF file exists
                If File.Exists(args(0)) Then
                    ' Construct the destination file path
                    Dim destfile As String = dupfolder + Path.GetFileName(args(0))

                    ' Overwrite the destination file if it already exists
                    If File.Exists(destfile) Then
                        File.Delete(destfile)
                    End If

                    ' Copy the source file to the destination folder
                    File.Copy(args(0), destfile)
                End If
            Else
                ' Show an error message if the number of parameters is invalid
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle exceptions and log the error details
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)

            ' Write the exception details to the Windows Event Log
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
