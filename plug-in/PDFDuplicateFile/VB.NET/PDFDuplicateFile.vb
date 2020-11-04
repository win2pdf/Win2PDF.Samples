Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics



Module PDFDuplicateFile
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const DUPFOLDER_SETTING As String = "Duplicate PDF Folder"
    Public Const APPNAME As String = "Win2PDF Duplicate File Plug-in"


    Sub Main(ByVal args() As String)
        Try
            If args.Length = 0 Then 'configure the duplicate file folder
                Dim bkfolder As New ChooseBackupFolder
                bkfolder.ShowDialog()
            ElseIf args.Length = 1 Then 'the only parameter is the PDF file name
                Dim dupfolder As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, DUPFOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup")
                If Not dupfolder.EndsWith("\") Then dupfolder += "\"
                If Not Directory.Exists(dupfolder) Then Directory.CreateDirectory(dupfolder)

                If File.Exists(args(0)) Then
                    Dim destfile As String = dupfolder + Path.GetFileName(args(0))
                    'overwrite the destination file if it exists
                    If File.Exists(destfile) Then
                        File.Delete(destfile)
                    End If
                    File.Copy(args(0), destfile)
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
