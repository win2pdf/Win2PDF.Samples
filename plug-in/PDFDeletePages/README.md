# Automatically Delete Pages from a PDF created by Win2PDF

This sample shows how to create a Win2PDF plug-in to automatically delete pages from newly created PDFs.  Some applications have margin issues that cause extra pages to be generated in printed output.  This plug-in deletes these extra pages automatically.

![Win2PDF Delete Extra Pages Plug-in](https://www.win2pdf.com/assets/images/win2pdf/plug-in/win2pdf-delete-pages-plug-in.png)

The Win2PDFDeletePages.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

This plug-in uses the [Win2PDF command line](https://www.win2pdf.com/doc/win2pdf-desktop-command-line.html) to [delete pages](https://www.win2pdf.com/doc/command-line-delete-pages-pdf.html) from the PDF, and requires a licensed version of Win2PDF.  Contact support@win2pdf.com for a time limited evaluation license.
