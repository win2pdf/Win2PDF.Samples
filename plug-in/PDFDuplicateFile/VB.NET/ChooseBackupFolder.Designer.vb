<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChooseBackupFolder
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnBrowseFolder = New System.Windows.Forms.Button()
        Me.txtBackupFolder = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnBrowseFolder
        '
        Me.btnBrowseFolder.Location = New System.Drawing.Point(437, 45)
        Me.btnBrowseFolder.Name = "btnBrowseFolder"
        Me.btnBrowseFolder.Size = New System.Drawing.Size(105, 34)
        Me.btnBrowseFolder.TabIndex = 0
        Me.btnBrowseFolder.Text = "Browse"
        Me.btnBrowseFolder.UseVisualStyleBackColor = True
        '
        'txtBackupFolder
        '
        Me.txtBackupFolder.Location = New System.Drawing.Point(12, 49)
        Me.txtBackupFolder.Name = "txtBackupFolder"
        Me.txtBackupFolder.Size = New System.Drawing.Size(397, 26)
        Me.txtBackupFolder.TabIndex = 1
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(319, 124)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(105, 36)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(437, 124)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(105, 36)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(190, 20)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Duplicate Folder Location"
        '
        'ChooseBackupFolder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(566, 181)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtBackupFolder)
        Me.Controls.Add(Me.btnBrowseFolder)
        Me.Name = "ChooseBackupFolder"
        Me.Text = "Choose Folder for Duplicate Files"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FolderBrowserDialog1 As Windows.Forms.FolderBrowserDialog
    Friend WithEvents btnBrowseFolder As Windows.Forms.Button
    Friend WithEvents txtBackupFolder As Windows.Forms.TextBox
    Friend WithEvents btnOK As Windows.Forms.Button
    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents Label1 As Windows.Forms.Label
End Class
