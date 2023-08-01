@echo off
goto :init

:header
    echo %__NAME% v%__VERSION%
    echo Use the Win2PDF command line to recursively batch convert all 
    echo supported document formats from sourcefolder and place converted
    echo files in the destfolder, replicating folder structure.
    echo.
    goto :eof

:usage
    echo USAGE:
    echo   %__BAT_NAME% [flags] "sourcefolder" "destfolder" destformat
    echo .
    echo where destformat can be:
    echo    pdf
    echo    jpg
    echo    tiff
    echo    png
    echo    docx
    echo    rtf
    echo    odt
    echo    xps
    echo .
    echo see https://www.win2pdf.com/doc/command-line-convert-to-format.html for more information
    echo.
    echo the optional [flags] can be one of:
    echo.  /?, --help           shows this help
    echo.  /v, --version        shows the version
rem    echo.  /e, --verbose        shows detailed output
    goto :eof

:version
    if "%~1"=="full" call :header & goto :eof
    echo %__VERSION%
    goto :eof

:missing_argument
    call :header
    call :usage
    echo.
    echo ****                                   ****
    echo ****    MISSING "REQUIRED ARGUMENT"    ****
    echo ****                                   ****
    echo.
    goto :eof

:init
    set "__NAME=%~n0"
    set "__VERSION=1.0"
    set "__YEAR=2023"

    set "__BAT_FILE=%~0"
    set "__BAT_PATH=%~dp0"
    set "__BAT_NAME=%~nx0"

    set "OptHelp="
    set "OptVersion="
    set "OptVerbose="

    set "sourcefolder="
    set "destfolder="
    set "convert_format="

:parse
    if "%~1"=="" goto :validate

    if /i "%~1"=="/?"         call :header & call :usage "%~2" & goto :end
    if /i "%~1"=="-?"         call :header & call :usage "%~2" & goto :end
    if /i "%~1"=="--help"     call :header & call :usage "%~2" & goto :end

    if /i "%~1"=="/v"         call :version      & goto :end
    if /i "%~1"=="-v"         call :version      & goto :end
    if /i "%~1"=="--version"  call :version full & goto :end

    if /i "%~1"=="/e"         set "OptVerbose=yes"  & shift & goto :parse
    if /i "%~1"=="-e"         set "OptVerbose=yes"  & shift & goto :parse
    if /i "%~1"=="--verbose"  set "OptVerbose=yes"  & shift & goto :parse

    if not defined sourcefolder     set "sourcefolder=%~1"     & shift & goto :parse
    if not defined destfolder  set "destfolder=%~1"  & shift & goto :parse
    if not defined convert_format set "convert_format=%~1" & shift & goto :parse

    shift
    goto :parse

:validate
    if not defined sourcefolder call :missing_argument & goto :end
    if not defined destfolder call :missing_argument & goto :end
    if not defined convert_format call :missing_argument & goto :end
    if not exist "%sourcefolder%" call :header & call :usage & echo "%sourcefolder% does not exist" & goto :end

:main
    if defined OptVerbose (
        echo **** DEBUG IS ON
    )

    echo sourcefolder: "%sourcefolder%"
    echo destfolder: "%destfolder%"
    echo convert format: %convert_format%

    rem first make folder structure
    for /r "%sourcefolder%" %%i in (*.*) do mkdir "%destfolder%%%~pi" 2> nul
    rem convert all files
    @echo on
    for /r "%sourcefolder%" %%i in (*.doc *.docx *.PDF *.RTF *.ODT *.TXT *.HTML *.SVG *.XPS *.JPG *.TIF *.PNG *.BMP *.GIF) do win2pdfd.exe convertto "%%i" "%destfolder%%%~pi%%~ni.%convert_format%" %convert_format%
    @echo off
    

:end
    call :cleanup
    exit /B

:cleanup
    REM The cleanup function is only really necessary if you
    REM are _not_ using SETLOCAL.
    set "__NAME="
    set "__VERSION="
    set "__YEAR="

    set "__BAT_FILE="
    set "__BAT_PATH="
    set "__BAT_NAME="

    set "OptHelp="
    set "OptVersion="
    set "OptVerbose="

    set "sourcefolder="
    set "destfolder="

    goto :eof