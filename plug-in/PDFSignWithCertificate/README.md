# Automatically Sign a newly created PDF file With Digital Certificate

This sample shows how to create a Win2PDF plug-in to automatically signa  newly created PDF with a digital certificate. To configure the digital certificate used in the signing process, run "Configure Win2PDF Signing Certificate" from the Windows start menu. The plug-in prompts for the certificate password each time that it is invoked.

![Win2PDF Sign With A Digital Certificate Plug-in](https://user-images.githubusercontent.com/6544906/120681410-f61e0880-c460-11eb-8529-c0d09b4237bb.png)

You can download a compiled version at: https://get.win2pdf.com/plug-in/Win2PDF-Sign-With-Certificate-Plug-In.exe

The Win2PDFSignWithCertificate.iss [Inno Setup](https://jrsoftware.org/isinfo.php) script creates a setup program that can be used to install the plug-in.

This plug-in uses the [Win2PDF command line](https://www.win2pdf.com/doc/win2pdf-desktop-command-line.html) to [sign the newly created PDF with a digital certificate](https://www.win2pdf.com/doc/command-line-sign-pdf-with-certificate.html), and requires a licensed version of Win2PDF Pro.  Contact support@win2pdf.com for a time limited evaluation license.
