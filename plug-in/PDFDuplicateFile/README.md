# Duplicate a PDF file created by Win2PDF in a designated backup location

This sample shows how to create a Win2PDF plug-in to automatically duplicate newly created PDF files created by Win2PDF in a designated duplicate file folder. If a file with the same name exists in the duplicate file folder, the file will be overwritten. The duplicate file folder can reside on a shared network location, or in a cloud backed location such as OneDrive, DropBox, or Google Drive.  Run the "Configure Win2PDF Duplicate File" shortcut from the Start menu to configure the duplicate file folder location.

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Duplicate-File-Plug-In.exe

The Win2PDFDuplicateFile.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.  The sample does not allow a user to turn off the duplicate file plug-in.

The plug-in requries that [Win2PDF](https://www.win2pdf.com/download/download.htm) is installed. Contact support@win2pdf.com for a time limited evaluation license.