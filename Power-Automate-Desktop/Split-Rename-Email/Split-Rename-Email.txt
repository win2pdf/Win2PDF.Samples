SET FileNameSearch TO $'''VENDOR #'''
SET EmailSearch TO $'''Email:'''
Display.SelectFile Title: $'''Select PDF to split''' FileFilter: $'''*.pdf''' IsTopMost: False CheckIfFileExists: True SelectedFile=> SourcePDF ButtonPressed=> ButtonPressed
Display.SelectFolder Description: $'''Select Output Folder''' IsTopMost: False SelectedFolder=> DestFolder ButtonPressed=> ButtonPressed2
# Make sure destination folder is empty
Folder.GetFiles Folder: DestFolder FileFilter: $'''*.pdf''' IncludeSubfolders: False FailOnAccessDenied: True SortBy1: Folder.SortBy.NoSort SortDescending1: False SortBy2: Folder.SortBy.NoSort SortDescending2: False SortBy3: Folder.SortBy.NoSort SortDescending3: False Files=> Files
LOOP FOREACH CurrentItem IN Files
    EXIT Code: 0 ErrorMessage: $'''Output folder must be empty'''
END
# Split Pages using Win2PDF Command Line
System.RunDOSCommand DOSCommandOrApplication: $'''C:\\Windows\\System32\\spool\\drivers\\x64\\3\\win2pdfd.exe splitpages \"%SourcePDF%\" \"%DestFolder%\" 1 0''' StandardOutput=> CommandOutput StandardError=> CommandErrorOutput ExitCode=> CommandExitCode
# Rename PDFs using Win2PDF Command Line
Folder.GetFiles Folder: DestFolder FileFilter: $'''*.pdf''' IncludeSubfolders: False FailOnAccessDenied: True SortBy1: Folder.SortBy.NoSort SortDescending1: False SortBy2: Folder.SortBy.NoSort SortDescending2: False SortBy3: Folder.SortBy.NoSort SortDescending3: False Files=> Files
LOOP FOREACH CurrentItem IN Files
    System.RunDOSCommand DOSCommandOrApplication: $'''C:\\Windows\\System32\\spool\\drivers\\x64\\3\\win2pdfd.exe getcontentsearch \"%CurrentItem%\" \"\" \"%FileNameSearch%\"''' StandardOutput=> CommandOutput3 StandardError=> CommandErrorOutput3 ExitCode=> CommandExitCode3
    IF IsNotEmpty(CommandOutput3) THEN
        Text.Trim Text: CommandOutput3 TrimOption: Text.TrimOption.Both TrimmedText=> TrimmedText
        File.Rename Files: CurrentItem NewName: $'''%DestFolder%\\Remittance Detail – %TrimmedText%''' KeepExtension: True IfFileExists: File.IfExists.Overwrite RenamedFiles=> RenamedFiles
    ELSE
        Display.ShowMessageWithTimeout Title: $'''Win2PDF Renamer''' Message: $'''Could not find FileNameSearch field for file %CurrentItem%''' Icon: Display.Icon.None Buttons: Display.Buttons.OK DefaultButton: Display.DefaultButton.Button1 IsTopMost: False Timeout: 10 ButtonPressed=> ButtonPressed4
    END
END
# Extract Email Address using Win2PDF Command Line and Email
Folder.GetFiles Folder: DestFolder FileFilter: $'''*.pdf''' IncludeSubfolders: False FailOnAccessDenied: True SortBy1: Folder.SortBy.NoSort SortDescending1: False SortBy2: Folder.SortBy.NoSort SortDescending2: False SortBy3: Folder.SortBy.NoSort SortDescending3: False Files=> Files
LOOP FOREACH CurrentItem IN Files
    System.RunDOSCommand DOSCommandOrApplication: $'''C:\\Windows\\System32\\spool\\drivers\\x64\\3\\win2pdfd.exe getcontentsearch \"%CurrentItem%\" \"\" %EmailSearch%''' StandardOutput=> CommandOutput3 StandardError=> CommandErrorOutput3 ExitCode=> CommandExitCode3
    IF IsNotEmpty(CommandOutput3) THEN
        Text.Trim Text: CommandOutput3 TrimOption: Text.TrimOption.Both TrimmedText=> TrimmedText
        # Insert "Send email" action here and configure with an SMTP server.  The "To" address should use the %TrimmedText% Flow variable. Display Message is a placeholder for debugging.
        Display.ShowMessage Title: $'''Found email''' Message: $'''\"%TrimmedText%\"''' Icon: Display.Icon.None Buttons: Display.Buttons.OK DefaultButton: Display.DefaultButton.Button1 IsTopMost: False ButtonPressed=> ButtonPressed3
        DISABLE Email.Send SMTPServer: $'''smtp.gmail.com''' Port: 587 EnableSSL: True AcceptUntrustedCertificates: False SendFrom: $'''craig.lebakken@gmail.com''' SendTo: $'''craig@lebakken.com''' Body: $'''''' IsBodyHtml: False Attachments: CurrentItem
    ELSE
        Display.ShowMessageWithTimeout Title: $'''Win2PDF Renamer''' Message: $'''Could not find \"Email:\" field for file %CurrentItem%''' Icon: Display.Icon.None Buttons: Display.Buttons.OK DefaultButton: Display.DefaultButton.Button1 IsTopMost: False Timeout: 10 ButtonPressed=> ButtonPressed4
    END
END