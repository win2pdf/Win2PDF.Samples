@echo off
setlocal enabledelayedexpansion

REM ============================================================
REM PDF Invoice Renamer - Searches for two search terms such as invoice numbers
REM and renames PDF files accordingly
REM ============================================================

REM Configuration
set "PDF_FOLDER=%~1"
set "SEARCH_TEXT=MO-"
set "SEARCH2_TEXT=Final"
set "RENAME_PREFIX=MO-"
set "FIELD_LENGTH=20"
set "WIN2PDF_PATH=win2pdfd.exe"

REM If no folder specified, use current directory
if "%PDF_FOLDER%"=="" set "PDF_FOLDER=%CD%"

echo ============================================================
echo PDF Invoice Renamer
echo ============================================================
echo Folder: %PDF_FOLDER%
echo Search Text: %SEARCH_TEXT%
echo Search 2 Text: %SEARCH2_TEXT%
echo ============================================================
echo.

REM Check if Win2PDF is accessible
where "%WIN2PDF_PATH%" >nul 2>&1
if errorlevel 1 (
    echo ERROR: win2pdfd.exe not found in PATH
    echo Please ensure Win2PDF is installed and in your system PATH
    pause
    exit /b 1
)

REM Counter for statistics
set /a TOTAL_FILES=0
set /a RENAMED_FILES=0
set /a FAILED_FILES=0

REM Process each PDF file in the folder
for %%F in ("%PDF_FOLDER%\*.pdf") do (
    set /a TOTAL_FILES+=1
    echo Processing: %%~nxF
    
    REM Search for invoice number
    for /f "delims=" %%I in ('"%WIN2PDF_PATH% getcontentsearch "%%F" "" "%SEARCH_TEXT%" "%FIELD_LENGTH%" "') do (
        set "INVOICE_NUM=%%I"
    )
    REM Search for 2nd search field
    for /f "delims=" %%I in ('"%WIN2PDF_PATH% getcontentsearch "%%F" "" "%SEARCH2_TEXT%" "%FIELD_LENGTH%" "') do (
        set "INVOICE_NUM=!INVOICE_NUM!%%I"
    )

    REM Check if invoice number was found
    if defined INVOICE_NUM (
        REM Clean up invoice number (remove spaces, special characters)
        set "INVOICE_NUM=!INVOICE_NUM: =!"
        set "INVOICE_NUM=!INVOICE_NUM:	=!"
        
        REM Create new filename
        set "NEW_NAME=%RENAME_PREFIX%!INVOICE_NUM!.pdf"
        set "NEW_PATH=%PDF_FOLDER%\!NEW_NAME!"
        
        REM Check if file with new name already exists
        if exist "!NEW_PATH!" (
            if /i not "%%F"=="!NEW_PATH!" (
                echo   WARNING: File already exists: !NEW_NAME!
                set /a FAILED_FILES+=1
            ) else (
                echo   Skipped: Already named correctly
            )
        ) else (
            REM Rename the file
            ren "%%F" "!NEW_NAME!"
            if !errorlevel! equ 0 (
                echo   SUCCESS: Renamed to !NEW_NAME!
                set /a RENAMED_FILES+=1
            ) else (
                echo   ERROR: Failed to rename file
                set /a FAILED_FILES+=1
            )
        )
    ) else (
        echo   WARNING: Invoice number not found
        set /a FAILED_FILES+=1
    )
    
    REM Clear variable for next iteration
    set "INVOICE_NUM="
    echo.
)

REM Display summary
echo ============================================================
echo Summary
echo ============================================================
echo Total files processed: %TOTAL_FILES%
echo Successfully renamed:  %RENAMED_FILES%
echo Failed/Skipped:        %FAILED_FILES%
echo ============================================================
echo.

endlocal