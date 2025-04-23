Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Windows.Forms

Module PDFSendToThunderbird

    ' Constants for application settings
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const THUNDERBIRD_PATH As String = "Thunderbird Path"

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Retrieve the Thunderbird executable path from application settings
            Dim thunderbird As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, "C:\Program Files (x86)\Mozilla Thunderbird\Thunderbird.exe")

            ' Check if the default Thunderbird path exists, otherwise look in alternative locations
            If Not File.Exists(thunderbird) Then
                Dim alt_locations() As String = {"C:\Program Files\Mozilla Thunderbird\Thunderbird.exe",
                    "C:\Program Files (x86)\Thunderbird\Thunderbird.exe",
                    "C:\Program Files\Thunderbird\Thunderbird.exe"}
                For Each tbird As String In alt_locations
                    If File.Exists(tbird) Then
                        thunderbird = tbird
                        ' Save the found Thunderbird path to application settings
                        Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, tbird)
                        Exit For
                    End If
                Next
            End If

            ' If no arguments are provided or Thunderbird is not found, prompt the user to configure the Thunderbird location
            If args.Length = 0 OrElse Not File.Exists(thunderbird) Then
                Dim filePath As String = String.Empty

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                ' Configure the OpenFileDialog to select the Thunderbird executable
                openFileDialog.Title = "Choose Thunderbird Program"
                openFileDialog.InitialDirectory = Path.GetDirectoryName(thunderbird)
                openFileDialog.Filter = "Application (*.exe)|*.exe"
                openFileDialog.RestoreDirectory = True

                ' Save the selected Thunderbird path to application settings
                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, THUNDERBIRD_PATH, openFileDialog.FileName)
                End If
            End If

            ' If a single argument is provided, treat it as the PDF file to attach
            If args.Length = 1 Then
                ' Ensure the file has a .PDF extension
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then
                    Dim newProc As System.Diagnostics.Process

                    ' Check if Thunderbird exists
                    If File.Exists(thunderbird) Then
                        ' Prepare the command-line arguments for Thunderbird
                        ' The arguments specify the subject and attachment for the email
                        Dim arguments1 As String = String.Format(" -compose ""subject='{0} attached',attachment='{1}'""", Path.GetFileName(args(0)), args(0))

                        Dim startInfo As New ProcessStartInfo(thunderbird)
                        With startInfo
                            .Arguments = arguments1
                        End With

                        ' Start Thunderbird with the specified arguments
                        newProc = System.Diagnostics.Process.Start(startInfo)
                        newProc.WaitForExit()

                        ' Check if Thunderbird exited with an error
                        If newProc.HasExited Then
                            If newProc.ExitCode <> 0 Then
                                Windows.Forms.MessageBox.Show(String.Format("Could not launch Thunderbird: {0} {1}, error code {2}", thunderbird, arguments1, newProc.ExitCode))
                            End If
                        End If
                    Else
                        ' Notify the user if Thunderbird could not be found
                        Windows.Forms.MessageBox.Show(String.Format("Could not find Thunderbird."))
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle any exceptions and log them to the Windows Event Log
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.Forms.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module
