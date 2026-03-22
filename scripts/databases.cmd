@echo off
setlocal enabledelayedexpansion
set "SCRIPT_NAME=%~n0"

if /I "%~1"=="help" (
  call :usage 0
  goto :end
)
if /I "%~1"=="-h" (
  call :usage 0
  goto :end
)
if /I "%~1"=="--help" (
  call :usage 0
  goto :end
)
if /I "%~1"=="/?" (
  call :usage 0
  goto :end
)

set "ACTION=%~1"
if /I "%ACTION%"=="update" (
  set "ACTION_VERB=Updating"
  set "ACTION_FAILURE=update"
  set "ACTION_COMMAND=dotnet ef database update"
  set "ACTION_DONE=All databases updated."
) else if /I "%ACTION%"=="drop" (
  set "ACTION_VERB=Dropping"
  set "ACTION_FAILURE=drop"
  set "ACTION_COMMAND=dotnet ef database drop --force"
  set "ACTION_DONE=All databases dropped."
) else (
  call :usage 1
  goto :end
)

set "HAS_ERRORS=0"

pushd "%~dp0\.."
set "SERVICES_DIR=src"

echo !ACTION_VERB! databases for all API projects...

for /d %%d in ("%SERVICES_DIR%\*.Api") do (
  set "SERVICE_NAME=%%~nxd"
  echo !ACTION_VERB! database for project: !SERVICE_NAME!
  pushd "%%d"
  !ACTION_COMMAND!
  if errorlevel 1 (
    echo Failed to !ACTION_FAILURE! database for project: !SERVICE_NAME!
    set "HAS_ERRORS=1"
  )
  popd
)

if "!HAS_ERRORS!"=="1" (
  echo Completed with errors.
  set "EXIT_CODE=1"
) else (
  echo !ACTION_DONE!
  set "EXIT_CODE=0"
)

popd
goto :end

:usage
set "EXIT_CODE=%~1"
echo Usage: %SCRIPT_NAME% ^<update^|drop^>
echo        %SCRIPT_NAME% help^|-h^|--help^|/?
echo.
echo Description:
echo   Updates or drops databases for all API projects in the src directory.
echo.
echo Examples:
echo   %SCRIPT_NAME% update
echo   %SCRIPT_NAME% drop
echo   %SCRIPT_NAME% help
exit /b

:end
endlocal & exit /b %EXIT_CODE%
