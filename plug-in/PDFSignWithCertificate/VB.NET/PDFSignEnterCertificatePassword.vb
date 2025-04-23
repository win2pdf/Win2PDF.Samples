' This class represents a form for entering a certificate password when signing a PDF.
Public Class PDFSignEnterCertificatePassword

    ' Stores the password entered by the user.
    Public Password As String = ""

    ' Handles the click event for the OK button.
    ' Sets the Password property to the text entered in the PasswordTextBox
    ' and closes the form.
    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Password = PasswordTextBox.Text
        Me.Close()
    End Sub

    ' Handles the click event for the Cancel button.
    ' Closes the form without setting the Password property.
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub

End Class
