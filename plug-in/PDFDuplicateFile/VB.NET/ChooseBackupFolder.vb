Imports System.Windows.Forms

Public Class ChooseBackupFolder
    Private Sub btnBrowseFolder_Click(sender As Object, e As EventArgs) Handles btnBrowseFolder.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            txtBackupFolder.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        SaveSetting(PDFDuplicateFile.WIN2PDF_COMPANY, PDFDuplicateFile.WIN2PDF_PRODUCT, PDFDuplicateFile.DUPFOLDER_SETTING, txtBackupFolder.Text)
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub CenterForm(ByVal frm As Form, Optional ByVal parent As Form = Nothing)
        Dim r As Drawing.Rectangle
        If parent IsNot Nothing Then
            r = parent.RectangleToScreen(parent.ClientRectangle)
        Else
            r = Screen.FromPoint(frm.Location).WorkingArea
        End If

        Dim x = r.Left + (r.Width - frm.Width) \ 2
        Dim y = r.Top + (r.Height - frm.Height) \ 2
        frm.Location = New Drawing.Point(x, y)
    End Sub


    Private Sub ChooseBackupFolder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtBackupFolder.Text = Interaction.GetSetting(WIN2PDF_COMPANY, WIN2PDF_PRODUCT, DUPFOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup")
        CenterForm(Me)
    End Sub
End Class