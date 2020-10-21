# Apply Multiple Watermarks to a newly created PDF file 

Win2PDF can [apply a watermark](https://www.win2pdf.com/doc/pdf-watermark.html) to a newly created PDF, but what if you need to use more than one watermark, such as a letterhead on page one and a confidential stamp on the remaining pages?  This sample shows how to create a Win2PDF plug-in to automatically apply one watermark to the first page of a PDF, and a second watermark to all remaining pages of a PDF.

[Win2PDF Multiple Watermarks Plug-in](https://www.win2pdf.com/assets/images/win2pdf/plug-in/win2pdf-apply-multiple-watermarks-plug-in.png)

You can download a compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Apply-Multiple-Watermarks-Plug-In.exe

The Win2PDFMultipleWatermarks.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

This plug-in uses the [Win2PDF command line](https://www.win2pdf.com/doc/win2pdf-desktop-command-line.html) to [apply the watermarks](https://www.win2pdf.com/doc/command-line-watermark-pdf.html) to the newly created PDF, and requires a licensed version of Win2PDF Pro.  Contact support@win2pdf.com for a time limited evaluation license.
