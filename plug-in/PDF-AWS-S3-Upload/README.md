# Automatically Upload a PDF file created by Win2PDF to Amazon Web Services S3 cloud storage, and optionally delete the file when the upload is complete

This sample shows how to create a Win2PDF plug-in to automatically upload newly created PDF files created by Win2PDF to Amazon Web Services S3 cloud storage. It can be configured to automatically delete the PDF when the viewer is closed. If a file with the same name exists in the S3 bucket, it will be overwritten. It uses the AWS SDK for .NET Version 3, and credentials are stored in the shared AWS credentials file (%USERPROFILE%\.aws\credentials).

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-S3-Upload-PDF-Plug-In.exe

The Win2PDFViewFile.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

The plug-in requries that [Win2PDF](https://www.win2pdf.com/download/download.htm) is installed. Contact support@win2pdf.com for a time limited evaluation license.