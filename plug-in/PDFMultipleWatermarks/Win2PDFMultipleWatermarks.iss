; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Win2PDF Apply Multiple Watermarks Plug-In"
#define MyAppVersion "1.0"
#define MyAppPublisher "Dane Prairie Systems, LLC"
#define MyAppURL "https://www.win2pdf.com"
#define MyAppExeName "PDFApplyMultipleWatermarks.exe"
#define MyWin2PDFPrinterName "Win2PDF" ;change to install plug-in for a different named Win2PDF printer

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{9024CCCB-C60E-44EA-9D60-F2AE8A887499}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\Win2PDF\Plug-Ins
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
OutputBaseFilename=Win2PDF-Apply-Multiple-Watermarks-Plug-In
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "VB.NET\bin\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
;configure Win2PDF to use the plug-in
;use HKLM to install for all users
;use "SOFTWARE\Dane Prairie Systems\PrinterName" to install for a specific printer
Root: HKCU; Subkey: "SOFTWARE\Dane Prairie Systems\{#MyWin2PDFPrinterName}"; ValueType:string; ValueName: "default post action"; ValueData: "{app}\{#MyAppExeName} ""%s"""; Flags: uninsdeletevalue
;Allow user to turn "Apply Multiple Watermarks" on or off in the Win2PDF File Save window. Remove this to always apply.
Root: HKCU; Subkey: "SOFTWARE\Dane Prairie Systems\{#MyWin2PDFPrinterName}"; ValueType:string; ValueName: "post action checkbox label"; ValueData: "Apply Multiple Watermarks"; Flags: uninsdeletevalue

[Icons]
Name: "{group}\Configure Win2PDF Multiple Watermarks"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"

[Code]
function InitializeSetup(): Boolean;
var
  plugin_installed: String;
begin
  //check if another plug-in is already installed
  if RegQueryStringValue(HKEY_CURRENT_USER, 'Software\Dane Prairie Systems\Win2PDF', 
    'default post action', plugin_installed) then
    if Pos(ExpandConstant('{#MyAppExeName}'), plugin_installed) = 0 then
      begin
        MsgBox('Another Win2PDF plug-in is already installed.  Please uninstall from Add or Remove Programs.', mbCriticalError, MB_OK);
        result := false;
      end
    else //the current Win2PDF plug is installed, allow an upgrade
      begin
        result := true;
      end
  else
    result := true;
end;
