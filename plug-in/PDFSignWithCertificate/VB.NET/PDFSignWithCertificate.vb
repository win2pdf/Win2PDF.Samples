Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.Windows.Forms

Module PDFSignWithCertificate

    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const CERTIFICATE_NAME As String = "Certificate Name"

    Public Const ERROR_LOCKED As Integer = 212
    Public Const ERROR_INVALID_FUNCTION As Integer = 2
    Public Const ERROR_FILE_NOT_FOUND As Integer = 2
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const ERROR_BAD_FORMAT As Integer = 11
    Public Const ERROR_INVALID_PARAMETER As Integer = 87
    Public Const ERROR_SUCCESS = 0


    Sub Main(ByVal args() As String)
        Try
            Dim certname As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, CERTIFICATE_NAME, "")

            If args.Length = 0 OrElse certname = "" _
                OrElse Not File.Exists(certname) Then 'configure certificate file
                Dim filePath As String = String.Empty

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                openFileDialog.Title = "Select Digital Certificate File"
                If certname.Length > 0 Then
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(certname)
                Else
                    openFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                End If

                openFileDialog.Filter = " PKCS#12 certificate(*.pfx;*.p12)|*.pfx;*.p12"
                openFileDialog.RestoreDirectory = True

                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, CERTIFICATE_NAME, openFileDialog.FileName)
                End If
            End If

            If args.Length = 1 Then 'the only parameter is the PDF file name
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then 'ignore if not PDF
                    Dim newProc As Diagnostics.Process
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    'get the path to the Win2PDF command line executable
                    If Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    If File.Exists(win2pdfcmdline) Then
                        'Dim certpass As String = Interaction.InputBox("Enter Certificate Password", WIN2PDF_PRODUCT, "")
                        Dim passDlg As New PDFSignEnterCertificatePassword
                        passDlg.ShowDialog()

                        If passDlg.Password.Length > 0 Then

                            'enclose the file names in quotes in case they contain spaces
                            'sign command line documented at: https://www.win2pdf.com/doc/command-line-sign-pdf-with-certificate.html
                            Debug.Assert(File.Exists(certname))
                            Dim arguments1 As String = String.Format("sign ""{0}"" ""{0}"" ""{1}"" ""{2}""", args(0), certname, passDlg.Password)

                            Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                            With startInfo
                                .Arguments = arguments1
                                .WindowStyle = ProcessWindowStyle.Hidden
                            End With

                            'execute the sign command line
                            newProc = Diagnostics.Process.Start(startInfo)
                            newProc.WaitForExit()
                            If newProc.HasExited Then
                                Select Case newProc.ExitCode
                                    Case ERROR_SUCCESS
                                        'do nothing
                                    Case ERROR_FILE_NOT_FOUND
                                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF could not find file: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Case ERROR_ACCESS_DENIED
                                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF Pro is licensed: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Case ERROR_BAD_FORMAT
                                        Windows.Forms.MessageBox.Show(String.Format("Could not load certificate, incorrect format or incorrect password: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Case Else
                                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                End Select
                            End If
                        Else
                            Windows.Forms.MessageBox.Show("Invalid Password", WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
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
