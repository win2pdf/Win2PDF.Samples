Imports System
Imports System.IO
Imports System.Threading
Imports System.Windows
Imports System.Windows.Forms

Module PDFReformatAsText
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const TEXTSIZE_SETTING As String = "Reformat Text Size"
    Public Const PAPERSIZE_SETTING As String = "Reformat Text Paper"
    Public Const MARGINSIZE_SETTING As String = "Reformat Text Margin"
    Public Const APPNAME As String = "Win2PDF Reformat As Text Plug-in"
    Public Const TXT2PDF_POSTFIX_NAME As String = "-formattedtxt2pdf-win2pdf.pdf"

    Public Const ERROR_LOCKED As Integer = 212
    Public Const ERROR_INVALID_FUNCTION As Integer = 2
    Public Const ERROR_FILE_NOT_FOUND As Integer = 2
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const ERROR_BAD_FORMAT As Integer = 11
    Public Const ERROR_INVALID_PARAMETER As Integer = 87
    Public Const ERROR_SUCCESS = 0

    Dim mut As Mutex = New Mutex(False, APPNAME)

    Function LaunchProcess(win2pdfcmdline As String, arguments1 As String) As Boolean
        Dim newProc As System.Diagnostics.Process
        LaunchProcess = False
        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
        With startInfo
            .Arguments = arguments1
            .WindowStyle = ProcessWindowStyle.Hidden
        End With

        'execute the command
        newProc = Diagnostics.Process.Start(startInfo)
        newProc.WaitForExit()
        If newProc.HasExited Then
            Select Case newProc.ExitCode
                Case ERROR_SUCCESS
                    LaunchProcess = True
                Case ERROR_FILE_NOT_FOUND
                    Windows.Forms.MessageBox.Show(String.Format("Win2PDF could not find file: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case ERROR_ACCESS_DENIED
                    Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case Else
                    Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
        End If
        newProc.Close()
    End Function

    Sub Main(ByVal args() As String)

        Try
            'only allow on instance of the plug-in to run at a time to prevent recursion in formattedtxt2pdf
            If (Not mut.WaitOne(1000, False)) Then
                Return
            End If

            If args.Length = 0 Then 'configure the formattedtxt2pdf parameters
                Dim papersize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, "letter")
                papersize = InputBox("Enter paper size", APPNAME, papersize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, papersize)

                Dim textsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, "12")
                textsize = InputBox("Enter text size in points", APPNAME, textsize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, textsize)

                Dim marginsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, "72")
                marginsize = InputBox("Enter margin size in points (1/72 inch)", APPNAME, marginsize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, marginsize)
            ElseIf args.Length = 1 Then 'the only parameter is the PDF file name
                If Not args(0).EndsWith(TXT2PDF_POSTFIX_NAME) Then 'skip recursive call in formattedtxt2pdf
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    'get the path to the Win2PDF command line executable
                    If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
                        win2pdfcmdline += "\spool\drivers\arm64\3\win2pdfd.exe"
                    ElseIf Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    If File.Exists(win2pdfcmdline) Then
                        Dim fontsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, "12")
                        Dim papersize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, "letter")
                        Dim marginsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, "72")
                        Dim txtfile As String = args(0) + ".txt"
                        Dim arguments1 As String = String.Format("extracttext ""{0}"" ""{1}"" 1", args(0), txtfile)

                        'first extract formatted text
                        If LaunchProcess(win2pdfcmdline, arguments1) Then
                            Dim newpdf As String = args(0) + TXT2PDF_POSTFIX_NAME 'save as new name to avoid recursion
                            arguments1 = String.Format("formattedtxt2pdf ""{0}"" ""{1}"" {2} {3} {4} {4} {4} {4}", txtfile, newpdf, papersize, fontsize, marginsize)
                            'next, convert formatted text back to PDF
                            If LaunchProcess(win2pdfcmdline, arguments1) Then
                                File.Delete(args(0))
                                File.Move(newpdf, args(0))
                            End If
                        End If
                        File.Delete(txtfile)
                    Else
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF is not installed. Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            Else
                Windows.MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        Finally
            mut.ReleaseMutex()
        End Try
    End Sub

End Module
