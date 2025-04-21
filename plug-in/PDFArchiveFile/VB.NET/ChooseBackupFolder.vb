Imports System.Windows.Forms
Imports Microsoft.WindowsAPICodePack.Dialogs

Public Class ChooseBackupFolder
    ' Event handler for the "Browse Folder" button click
    Private Sub btnBrowseFolder_Click(sender As Object, e As EventArgs) Handles btnBrowseFolder.Click
        ' Open a folder picker dialog to allow the user to select a folder
        Dim dialog As CommonOpenFileDialog = New CommonOpenFileDialog()
        dialog.IsFolderPicker = True ' Ensure the dialog only allows folder selection
        If dialog.ShowDialog() = CommonFileDialogResult.Ok Then
            ' Set the selected folder path to the text box
            txtBackupFolder.Text = dialog.FileName
        End If
    End Sub

    ' Event handler for the "OK" button click
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        ' Save the selected folder path to the application settings
        SaveSetting(PDFArchiveFile.WIN2PDF_COMPANY, PDFArchiveFile.WIN2PDF_PRODUCT, PDFArchiveFile.ARCHIVE_FOLDER_SETTING, txtBackupFolder.Text)
        ' Close the form
        Me.Close()
    End Sub

    ' Event handler for the "Cancel" button click
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ' Close the form without saving any changes
        Me.Close()
    End Sub

    ' Method to center the form on the screen or relative to a parent form
    ' Parameters:
    ' - frm: The form to be centered
    ' - parent: (Optional) The parent form relative to which the form will be centered
    Private Sub CenterForm(ByVal frm As Form, Optional ByVal parent As Form = Nothing)
        Dim r As Drawing.Rectangle
        If parent IsNot Nothing Then
            ' Get the rectangle of the parent form's client area
            r = parent.RectangleToScreen(parent.ClientRectangle)
        Else
            ' Get the working area of the screen where the form is located
            r = Screen.FromPoint(frm.Location).WorkingArea
        End If

        ' Calculate the new position to center the form
        Dim x = r.Left + (r.Width - frm.Width) \ 2
        Dim y = r.Top + (r.Height - frm.Height) \ 2
        frm.Location = New Drawing.Point(x, y)
    End Sub

    ' Event handler for the form's Load event
    Private Sub ChooseBackupFolder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load the previously saved backup folder path from application settings
        txtBackupFolder.Text = Interaction.GetSetting(PDFArchiveFile.WIN2PDF_COMPANY, PDFArchiveFile.WIN2PDF_PRODUCT, PDFArchiveFile.ARCHIVE_FOLDER_SETTING, My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\backup")
        ' Center the form on the screen
        CenterForm(Me)
    End Sub

End Class