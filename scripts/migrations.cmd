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
if /I "%ACTION%"=="add" (
  set "ACTION_VERB=Adding"
  set "ACTION_FAILURE=add"
  set "ACTION_COMMAND=dotnet ef migrations add ""Initialize"""
  set "ACTION_DONE=All migrations added."
) else if /I "%ACTION%"=="remove" (
  set "ACTION_VERB=Removing"
  set "ACTION_FAILURE=remove"
  set "ACTION_COMMAND=dotnet ef migrations remove"
  set "ACTION_DONE=All migrations removed."
) else (
  call :usage 1
  goto :end
)

set "HAS_ERRORS=0"

pushd "%~dp0\.."
set "SERVICES_DIR=src"

echo !ACTION_VERB! migrations for all API projects...

for /d %%d in ("%SERVICES_DIR%\*.Api") do (
  set "SERVICE_NAME=%%~nxd"
  echo !ACTION_VERB! migration for project: !SERVICE_NAME!
  pushd "%%d"
  !ACTION_COMMAND!
  if errorlevel 1 (
    echo Failed to !ACTION_FAILURE! migration for project: !SERVICE_NAME!
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
echo Usage: %SCRIPT_NAME% ^<add^|remove^>
echo        %SCRIPT_NAME% help^|-h^|--help^|/?
echo.
echo Description:
echo   Adds or removes migrations for all API projects in the src directory.
echo.
echo Examples:
echo   %SCRIPT_NAME% add
echo   %SCRIPT_NAME% remove
echo   %SCRIPT_NAME% help
exit /b

:end
endlocal & exit /b %EXIT_CODE%
