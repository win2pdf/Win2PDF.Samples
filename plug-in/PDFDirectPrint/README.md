# Print PDF files created by Win2PDF directly to a printer that supports Direct PDF Printing

This sample shows how to create a Win2PDF plug-in to automatically print a newly created PDF to a printer that supports "PDF Direct" Printing. Most business class printers from HP, OKI,  Kyocera, Ricoh, Canon, and Xerox support Direct PDF printing, but most consumer inkjet printers do not support it. If you attempt to use this plug-in on a paper printer that does not support PDF Direct printing, many pages of garbled text will be printed instead of the PDF.

This plug-in requires Win2PDF 10.0.142.1 or above.

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Direct-PDF-Print.exe

The Win2PDFDirectPrint.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.