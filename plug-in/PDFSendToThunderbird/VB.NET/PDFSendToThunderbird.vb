Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Windows.Forms

Module PDFSendToThunderbird

    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const THUNDERBIRD_PATH As String = "Thunderbird Path"

    Sub Main(ByVal args() As String)
        Try
            Dim thunderbird As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, "C:\Program Files (x86)\Mozilla Thunderbird\Thunderbird.exe")

            'look in other standard locations if default location doesn't exist
            If Not File.Exists(thunderbird) Then
                Dim alt_locations() As String = {"C:\Program Files\Mozilla Thunderbird\Thunderbird.exe",
                    "C:\Program Files (x86)\Thunderbird\Thunderbird.exe",
                    "C:\Program Files\Thunderbird\Thunderbird.exe"}
                For Each tbird As String In alt_locations
                    If File.Exists(tbird) Then
                        thunderbird = tbird
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, tbird)
                        Exit For
                    End If
                Next
            End If

            If args.Length = 0 OrElse Not File.Exists(thunderbird) Then 'configure Thunderbird location
                Dim filePath As String = String.Empty

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                openFileDialog.Title = "Choose Thunderbird Program"
                openFileDialog.InitialDirectory = Path.GetDirectoryName(thunderbird)

                openFileDialog.Filter = "Application (*.exe)|*.exe"
                openFileDialog.RestoreDirectory = True

                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, openFileDialog.FileName)
                End If
            End If

            If args.Length = 1 Then 'the only parameter is the PDF file name
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then 'ignore if not PDF
                    Dim newProc As Diagnostics.Process

                    If File.Exists(thunderbird) Then

                        'enclose the file names in quotes in case they contain spaces
                        'command line arguments are documented at http://kb.mozillazine.org/Command_line_arguments_-_Thunderbird
                        Dim arguments1 As String = String.Format(" -compose ""subject='{0} attached',attachment='{1}'""", Path.GetFileName(args(0)), args(0))

                        Dim startInfo As New ProcessStartInfo(thunderbird)
                        With startInfo
                            .Arguments = arguments1
                        End With

                        'execute the watermark command line for the first page
                        newProc = Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit()
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                Windows.Forms.MessageBox.Show(String.Format("Could not launch Thunderbird: {0} {1}, error code {2}", thunderbird, arguments1, newProc.ExitCode))
                            End If
                        End If
                    Else
                        Windows.Forms.MessageBox.Show(String.Format("Could not find Thunderbird."))
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
