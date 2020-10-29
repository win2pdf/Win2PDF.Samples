# Make an Image Only PDF created by Win2PDF Searchable

This sample shows how to create a Win2PDF plug-in to make an Image Only PDF created by Win2PDF searchable.  Some types of fonts are rendered with an encoding that makes copied or searched text unreadable. Creating the PDF as an "Image Only" PDF and then making the PDF searchable using OCR resolves this problem.

The Win2PDFMakeSearchable.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

This plug-in uses the [Win2PDF command line](https://www.win2pdf.com/doc/win2pdf-desktop-command-line.html) to [make the pdf searchable](https://www.win2pdf.com/doc/command-line-make-searcheable-ocr-pdf.html), and requires a licensed version of [Win2PDF](https://www.win2pdf.com/download/download.htm).  The make searchable command also requires that the [Win2PDF Desktop Searchable](https://helpdesk.win2pdf.com/index.php?/Knowledgebase/Article/View/197/15/win2pdf-desktop-with-ocr-download) component is installed (separate download from Win2PDF). Contact support@win2pdf.com for a time limited evaluation license.