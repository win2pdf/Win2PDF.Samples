//common code used by Win2PDF plug-in setup scripts
[code]
function GetWin2PDFFile: String;
begin

  if isWin64() Then
  begin
     Result := ExpandConstant('{sys}\spool\drivers\x64\3');
  end
  else
  begin
     Result := ExpandConstant('{sys}\spool\drivers\w32x86\3');
  end;
  Result := Result + '\win2pdf.exe';
end;

function InitializeSetup(): Boolean;
var
  pluginstalled: String;
  ErrCode: Integer;
  Win2PDFInstalled: Boolean;
begin
  Win2PDFInstalled := FileExists(GetWin2PDFFile());

  if Win2PDFInstalled then
    Log('Win2PDF file found')
  else
    Log('Win2PDF file not found');

  // check if Win2PDF is installed
  if (Not Win2PDFInstalled) then
    begin
        MsgBox('If Win2PDF is not installed, download and run the Win2PDF setup program before installing the plug-in.', mbCriticalError, MB_OK);
        ShellExec('open', 'https://www.win2pdf.com/download/download.htm', '', '', SW_SHOW, ewNoWait, ErrCode);
        result := false;
        Log('Win2PDF not found.');
    end;
  //check if another plug-in is already installed
  if RegQueryStringValue(HKEY_CURRENT_USER, 'Software\Dane Prairie Systems\Win2PDF', 
    'default post action', pluginstalled) then
    if Pos(ExpandConstant('{#MyAppExeName}'), pluginstalled) = 0 then
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