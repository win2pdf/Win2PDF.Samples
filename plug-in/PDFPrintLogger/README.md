# Log PDF files created by Win2PDF to the Windows Event Log

This sample shows how to create a Win2PDF plug-in to automatically add events to the "Application" section of the Windows event log when a PDF is created by Win2PDF. The events are saved in the following format:

```
File C:\Users\craig\Documents\test\Document.pdf was created by Win2PDF
```

You can download the compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Print-Logger-Plug-In.exe

The Win2PDFDeletePages.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.