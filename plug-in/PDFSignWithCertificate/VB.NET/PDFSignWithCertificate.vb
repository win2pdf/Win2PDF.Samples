﻿Imports System
Imports System.IO
Imports System.Windows
Imports Microsoft.Win32
Imports System.Diagnostics
Imports System.Windows.Forms

' This module handles signing PDF files with a digital certificate using the Win2PDF command line tool.
Module PDFSignWithCertificate

    ' Constants for company and product information.
    Public Const WIN2PDF_COMPANY As String = "Dane Prairie Systems"
    Public Const WIN2PDF_PRODUCT As String = "Win2PDF"
    Public Const CERTIFICATE_NAME As String = "Certificate Name"

    ' Error codes used for handling process exit statuses.
    Public Const ERROR_LOCKED As Integer = 212
    Public Const ERROR_INVALID_FUNCTION As Integer = 2
    Public Const ERROR_FILE_NOT_FOUND As Integer = 2
    Public Const ERROR_ACCESS_DENIED As Integer = 5
    Public Const ERROR_BAD_FORMAT As Integer = 11
    Public Const ERROR_INVALID_PARAMETER As Integer = 87
    Public Const ERROR_SUCCESS = 0

    ' Entry point of the module.
    ' Handles selecting a certificate file, prompting for a password, and signing a PDF file.
    Sub Main(ByVal args() As String)
        Try
            ' Retrieve the saved certificate file path from application settings.
            Dim certname As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, CERTIFICATE_NAME, "")

            ' If no arguments are provided or the certificate file is missing, prompt the user to select a certificate file.
            If args.Length = 0 OrElse certname = "" _
                OrElse Not File.Exists(certname) Then
                Dim filePath As String = String.Empty

                Dim openFileDialog As Forms.OpenFileDialog = New Forms.OpenFileDialog()

                ' Configure the Open File dialog for selecting a certificate file.
                openFileDialog.Title = "Select Digital Certificate File"
                If certname.Length > 0 Then
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(certname)
                Else
                    openFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                End If

                openFileDialog.Filter = "PKCS#12 certificate(*.pfx;*.p12)|*.pfx;*.p12"
                openFileDialog.RestoreDirectory = True

                ' Save the selected certificate file path to application settings.
                If (openFileDialog.ShowDialog() = DialogResult.OK) Then
                    Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, CERTIFICATE_NAME, openFileDialog.FileName)
                End If
            End If

            ' If arguments are provided, process the PDF file and optional master password.
            If args.Length = 1 OrElse args.Length = 2 Then
                ' Ensure the first argument is a PDF file.
                If Path.GetExtension(args(0)).ToUpper = ".PDF" Then
                    Dim newProc As Diagnostics.Process
                    Dim win2pdfcmdline = Environment.SystemDirectory

                    ' Determine the path to the Win2PDF command line executable based on the system architecture.
                    If System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine) = "ARM64" Then
                        win2pdfcmdline += "\spool\drivers\arm64\3\win2pdfd.exe"
                    ElseIf Environment.Is64BitOperatingSystem Then
                        win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                    Else
                        win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                    End If

                    ' Check if the Win2PDF command line executable exists.
                    If File.Exists(win2pdfcmdline) Then
                        ' Prompt the user to enter the certificate password using a custom dialog.
                        Dim passDlg As New PDFSignEnterCertificatePassword
                        passDlg.ShowDialog()

                        ' If a password is entered, proceed with signing the PDF.
                        If passDlg.Password.Length > 0 Then
                            Dim masterpass As String = ""

                            ' Retrieve the optional master password from the second argument.
                            If args.Length = 2 Then
                                masterpass = args(1)
                            End If

                            ' Construct the command line arguments for signing the PDF.
                            ' Syntax of the sign command:
                            ' win2pdfd.exe sign "sourcepdf" "destpdf" "certfile" "certpassword" ["masterpassword"]
                            ' - "sourcepdf": The path to the source PDF file to be signed.
                            ' - "destpdf": The path to save the signed PDF file. If the same as "sourcepdf", the file is modified in place.
                            ' - "certfile": The path to the PKCS#12 certificate file (.pfx or .p12) used for signing.
                            ' - "certpassword": The password for the certificate file.
                            ' - "masterpassword" (optional): The master password for the PDF if it is encrypted. Use an empty string ("") if not needed.
                            Debug.Assert(File.Exists(certname))
                            Dim arguments1 As String = String.Format("sign ""{0}"" ""{0}"" ""{1}"" ""{2}"" ""{3}""", args(0), certname, passDlg.Password, masterpass)

                            ' Configure the process start information.
                            Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                            With startInfo
                                .Arguments = arguments1
                                .WindowStyle = ProcessWindowStyle.Hidden
                            End With

                            ' Execute the signing process and handle the exit code.
                            newProc = Diagnostics.Process.Start(startInfo)
                            newProc.WaitForExit()
                            If newProc.HasExited Then
                                Select Case newProc.ExitCode
                                    Case ERROR_SUCCESS
                                        ' Signing succeeded, no action needed.
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
                            ' Display an error if no password is entered.
                            Windows.Forms.MessageBox.Show("Invalid Password", WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        ' Display an error if the Win2PDF command line tool is not installed.
                        Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
                    End If
                End If
            End If
        Catch ex As Exception
            ' Handle any exceptions and log them to the Windows Event Log.
            Dim exception_description = String.Format("Win2PDF plug-in exception {0}, stack {1}, targetsite {2}", ex.Message, ex.StackTrace, ex.TargetSite)
            Windows.Forms.MessageBox.Show(exception_description)
            Using eventLog As EventLog = New EventLog("Application")
                eventLog.Source = "Win2PDF"
                eventLog.WriteEntry(exception_description, EventLogEntryType.Error, 101)
            End Using
        End Try
    End Sub

End Module