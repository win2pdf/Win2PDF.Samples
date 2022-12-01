Imports System.Windows.Forms
Imports System.IO

Module PDFDirectPrint
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

    Sub Main(ByVal args() As String)
        Try
            Dim printername As String = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, "")

            If args.Length = 0 OrElse printername = "" Then 'configure printer
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

                Interaction.SaveSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, PRINTER_NAME, printername)
            ElseIf args.Length = 1 Then 'the only parameter is the PDF file name
                Dim newProc As System.Diagnostics.Process
                Dim win2pdfcmdline = Environment.SystemDirectory

                'get the path to the Win2PDF command line executable
                If Environment.Is64BitOperatingSystem Then
                    win2pdfcmdline += "\spool\drivers\x64\3\win2pdfd.exe"
                Else
                    win2pdfcmdline += "\spool\drivers\w32x86\3\win2pdfd.exe"
                End If

                If File.Exists(win2pdfcmdline) Then
                    Dim arguments1 As String = String.Format("printpdfdirect ""{0}"" ""{1}""", args(0), printername)

                    Dim startInfo As New ProcessStartInfo(win2pdfcmdline)
                    With startInfo
                        .Arguments = arguments1
                        .WindowStyle = ProcessWindowStyle.Hidden
                    End With

                    'execute the print direct command
                    newProc = Diagnostics.Process.Start(startInfo)
                    newProc.WaitForExit()
                    If newProc.HasExited Then
                        Select Case newProc.ExitCode
                            Case ERROR_SUCCESS
                                        'do nothing
                            Case ERROR_FILE_NOT_FOUND
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF could not find file: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Case ERROR_ACCESS_DENIED
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed, make sure Win2PDF is licensed: {0}", win2pdfcmdline), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Case Else
                                Windows.Forms.MessageBox.Show(String.Format("Win2PDF command line failed: {0}, error code {1}", win2pdfcmdline, newProc.ExitCode), WIN2PDF_PRODUCT, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                    End If
                Else
                    Windows.Forms.MessageBox.Show(String.Format("Win2PDF Pro is not installed.  Download Win2PDF at https://www.win2pdf.com/download/"))
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
