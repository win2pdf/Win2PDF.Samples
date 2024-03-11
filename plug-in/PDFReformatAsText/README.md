# Reformat a PDF file created by Win2PDF using only text with a fixed width font    

This sample shows how to create a Win2PDF plug-in to automatically reformat PDF files created by Win2PDF using a fixed width font. This can be useful for legacy reports that are incorrectly rendered using a variable width font. Run the "Configure Win2PDF Reformat As Text" shortcut from the Start menu to configure the reformated font size.

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Reformat-As-Text-Plug-In.exe

The Win2PDFReformatAsText.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.  The sample does not allow a user to turn off the duplicate file plug-in.

The plug-in requries that [Win2PDF](https://www.win2pdf.com/download/download.htm) is installed. Contact support@win2pdf.com for a time limited evaluation license.