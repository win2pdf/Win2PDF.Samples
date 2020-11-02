# Archive newly created PDF files created by Win2PDF in a designated backup location

This sample shows how to create a Win2PDF plug-in to automatically archive newly created PDF files created by Win2PDF in a designated archive file folder. The files are appended to an archive PDF named based on the current date.  The archive file folder can reside on a shared network location, or in a cloud backed location such as OneDrive, DropBox, or Google Drive.  Run the "Configure Win2PDF Archive" shortcut from the Start menu to configure the archive file folder location.

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Archive-File-Plug-In.exe

The Win2PDFArchiveFile.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.  The sample does not allow a user to turn off the duplicate file plug-in.

The plug-in requries that [Win2PDF](https://www.win2pdf.com/download/download.htm) is installed. Contact support@win2pdf.com for a time limited evaluation license.