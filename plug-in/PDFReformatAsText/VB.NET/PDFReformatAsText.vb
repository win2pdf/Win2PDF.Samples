Imports System
Imports System.IO
Imports System.Threading
Imports System.Windows
Imports System.Windows.Forms

Module PDFReformatAsText
    ' Constants for application and settings
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const TEXTSIZE_SETTING As String = "Reformat Text Size"
    Public Const PAPERSIZE_SETTING As String = "Reformat Text Paper"
    Public Const MARGINSIZE_SETTING As String = "Reformat Text Margin"
    Public Const APPNAME As String = "Win2PDF Reformat As Text Plug-in"
    Public Const TXT2PDF_POSTFIX_NAME As String = "-formattedtxt2pdf-win2pdf.pdf"

    ' Error codes for process execution
    Public Const ERROR_LOCKED As Integer = 212
    Public Const ERROR_INVALID_FUNCTION As Integer = 2
    Public Const ERROR_FILE_NOT_FOUND As Integer = 2
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const ERROR_BAD_FORMAT As Integer = 11
    Public Const ERROR_INVALID_PARAMETER As Integer = 87
    Public Const ERROR_SUCCESS = 0

    ' Mutex to ensure only one instance of the application runs at a time
    Dim mut As Mutex = New Mutex(False, APPNAME)

    ' Function to launch an external process with specified arguments
    Function LaunchProcess(win2pdfcmdline As String, arguments1 As String) As Boolean
        Dim newProc As System.Diagnostics.Process
        LaunchProcess = False
        Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
        With startInfo
            .Arguments = arguments1
            .WindowStyle = ProcessWindowStyle.Hidden
        End With

        ' Execute the command
        newProc = Diagnostics.Process.Start(startInfo)
        newProc.WaitForExit()

        ' Handle process exit codes
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

    ' Main entry point of the application
    Sub Main(ByVal args() As String)
        Try
            ' Ensure only one instance of the application runs at a time
            If (Not mut.WaitOne(1000, False)) Then
                Return
            End If

            ' If no arguments are provided, configure the text-to-PDF parameters
            If args.Length = 0 Then
                ' Get and save paper size setting
                Dim papersize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, "letter")
                papersize = InputBox("Enter paper size", APPNAME, papersize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, papersize)

                ' Get and save text size setting
                Dim textsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, "12")
                textsize = InputBox("Enter text size in points", APPNAME, textsize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, textsize)

                ' Get and save margin size setting
                Dim marginsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, "72")
                marginsize = InputBox("Enter margin size in points (1/72 inch)", APPNAME, marginsize)
                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, marginsize)

                ' If one argument is provided, process the PDF file
            ElseIf args.Length = 1 Then
                ' Skip recursive calls for already processed files
                If Not args(0).EndsWith(TXT2PDF_POSTFIX_NAME) Then
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    ' Determine the path to the Win2PDF command line executable
                    If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
                        win2pdfcmdline += "\spool\drivers\arm64\3\win2pdfd.exe"
                    ElseIf Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    ' Check if the executable exists
                    If File.Exists(win2pdfcmdline) Then
                        ' Retrieve settings for text size, paper size, and margin size
                        Dim fontsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, TEXTSIZE_SETTING, "12")
                        Dim papersize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PAPERSIZE_SETTING, "letter")
                        Dim marginsize As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, MARGINSIZE_SETTING, "72")
                        Dim txtfile As String = args(0) + ".txt"
                        Dim arguments1 As String = String.Format("extracttext ""{0}"" ""{1}"" 1", args(0), txtfile)

                        ' Extract formatted text from the PDF
                        If LaunchProcess(win2pdfcmdline, arguments1) Then
                            Dim newpdf As String = args(0) + TXT2PDF_POSTFIX_NAME ' Save as a new name to avoid recursion
                            ' Use the "formattedtxt2pdf" command to convert the text file to a PDF
                            ' Command format: win2pdfd.exe formattedtxt2pdf "source.txt" "dest.pdf" papersize textsize marginleft margintop marginright marginbottom
                            ' - "source.txt": The input text file to be converted.
                            ' - "dest.pdf": The output PDF file to be created.
                            ' - "papersize": The paper size for the PDF (e.g., letter, A4, legal, etc.).
                            ' - "textsize": The font size in points (e.g., 12 for 12pt font).
                            ' - "marginleft", "margintop", "marginright", "marginbottom": Margins in points (1/72 inch).
                            arguments1 = String.Format("formattedtxt2pdf ""{0}"" ""{1}"" {2} {3} {4} {4} {4} {4}", txtfile, newpdf, papersize, fontsize, marginsize)

                            ' Convert the formatted text back to a PDF
                            If LaunchProcess(win2pdfcmdline, arguments1) Then
                                File.Delete(args(0)) ' Delete the original PDF
                                File.Move(newpdf, args(0)) ' Rename the new PDF to the original name
                            End If
                        End If
                        File.Delete(txtfile) ' Delete the temporary text file
                    Else
                        ' Display an error if Win2PDF is not installed
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF is not installed. Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            Else
                ' Display an error for invalid number of parameters
                Windows.MessageBox.Show("Invalid number of parameters")
            End If
        Catch ex As Exception
            ' Handle exceptions and log them to the Windows Event Log
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        Finally
            ' Release the mutex to allow other instances to run
            mut.ReleaseMutex()
        End Try
    End Sub

End Module
