# Automatically Split a PDF created by Win2PDF into individual files with 1 page per file

This sample shows how to create a Win2PDF plug-in to automatically split a newly created PDF into individual files.

The Win2PDFSplitPages.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

This plug-in uses the [Win2PDF command line](https://www.win2pdf.com/doc/win2pdf-desktop-command-line.html) to [split pages](https://www.win2pdf.com/doc/command-line-split-pages-pdf.html) from the PDF, and requires a licensed version of Win2PDF.  Contact support@win2pdf.com for a time limited evaluation license.