Imports System.Windows.Forms
Imports System.IO
Imports Microsoft.Win32

Module PDFDirectPrint
    ' Constants for application settings and error codes
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const PRINTER_NAME As String = "Direct Printer"

    Public Const ERROR_LOCKED As Integer = 212
    Public Const ERROR_INVALID_FUNCTION As Integer = 2
    Public Const ERROR_FILE_NOT_FOUND As Integer = 2
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const ERROR_BAD_FORMAT As Integer = 11
    Public Const ERROR_INVALID_PARAMETER As Integer = 87
    Public Const ERROR_SUCCESS = 0

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Retrieve the printer name from application settings
            Dim printername As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, "")

            ' If the setting does not exist, try to load from the registry
            If String.IsNullOrEmpty(printername) Then
                Dim regPath As String = $"SOFTWARE\{WIN2PDF_COMPANY}\{WIN2PDF_PRODUCT}"
                Using regKey As RegistryKey = Registry.LocalMachine.OpenSubKey(regPath)
                    If regKey IsNot Nothing Then
                        Dim regPrinterName = regKey.GetValue(PRINTER_NAME)
                        If regPrinterName IsNot Nothing Then
                            printername = regPrinterName.ToString()
                        End If
                    End If
                End Using
            End If

            ' If no arguments are provided or the printer name is not set, configure the printer
            If args.Length = 0 OrElse String.IsNullOrEmpty(printername) Then
                MsgBox("Select PDF Direct Print printer.", MsgBoxStyle.Information, WIN2PDF_PRODUCT)
                Dim dlgPrint As New Windows.Forms.PrintDialog
                Try
                    With dlgPrint
                        .AllowSelection = True
                        .ShowNetwork = True
                        If printername.Length > 0 Then
                            .PrinterSettings.PrinterName = printername
                        End If
                    End With
                    If dlgPrint.ShowDialog = Windows.Forms.DialogResult.OK Then
                        printername = dlgPrint.PrinterSettings.PrinterName
                    End If
                Catch ex As Exception
                    MsgBox("Print Error: " & ex.Message, MsgBoxStyle.Exclamation, WIN2PDF_PRODUCT)
                End Try

                ' Save the selected printer name to application settings
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, printername)

                ' Also save to HKLM if the user is an administrator
                If IsUserAdministrator() Then
                    Try
                        Dim regPath As String = $"SOFTWARE\{WIN2PDF_COMPANY}\{WIN2PDF_PRODUCT}"
                        Using regKey As RegistryKey = Registry.LocalMachine.CreateSubKey(regPath, True)
                            If regKey IsNot Nothing Then
                                regKey.SetValue(PRINTER_NAME, printername, RegistryValueKind.String)
                            End If
                        End Using
                    Catch rex As Exception
                        MessageBox.Show("Unable to save printer name to HKLM: " & rex.Message, WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End Try
                End If
            ElseIf args.Length = 1 Then
                ' If one argument is provided, treat it as the PDF file name to print
                Dim newProc As System.Diagnostics.Process
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
                    ' Construct the command line arguments for the "printpdfdirect" command
                    ' Syntax: win2pdfd.exe printpdfdirect "sourcefile" "printername"
                    ' - "sourcefile": Path to the PDF file to print (local file or URL)
                    ' - "printername": Name of the printer that supports Direct PDF printing
                    Dim arguments1 As String = String.Format("printpdfdirect ""{0}"" ""{1}""", args(0), printername)

                    Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                    With startInfo
                        .Arguments = arguments1
                        .WindowStyle = ProcessWindowStyle.Hidden
                    End With

                    ' Execute the "printpdfdirect" command
                    newProc = Diagnostics.Process.Start(startInfo)
                    newProc.WaitForExit()

                    ' Handle the process exit code
                    If newProc.HasExited Then
                        Select Case newProc.ExitCode
                            Case ERROR_SUCCESS
                                ' Command executed successfully, no action needed
                            Case ERROR_FILE_NOT_FOUND
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF could not find file: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Case ERROR_ACCESS_DENIED
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Case Else
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                    End If
                Else
                    ' Display a message if the Win2PDF executable is not installed
                    Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                End If
            Else
                ' Handle invalid number of parameters
                MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Log and display any exceptions that occur
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

    ' Check if the current user is an administrator
    Private Function IsUserAdministrator() As Boolean
        Try
            Dim identity = System.Security.Principal.WindowsIdentity.GetCurrent()
            Dim principal = New System.Security.Principal.WindowsPrincipal(identity)
            Return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator)
        Catch
            Return False
        End Try
    End Function

End Module
