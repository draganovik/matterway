@echo off
setlocal enabledelayedexpansion
set "HAS_ERRORS=0"

pushd "%~dp0\.."
set "SERVICES_DIR=src\Services"

echo Updating databases for all API projects...

for /d %%d in ("%SERVICES_DIR%\*") do (
  set "SERVICE_NAME=%%~nxd"
  if exist "%%d\!SERVICE_NAME!.API" (
    echo Updating database for project: !SERVICE_NAME!
    pushd "%%d\!SERVICE_NAME!.API"
    dotnet ef database update
    if errorlevel 1 (
      echo Failed to update database for project: !SERVICE_NAME!
      set "HAS_ERRORS=1"
    )
    popd
  )
)

if "!HAS_ERRORS!"=="1" (
  echo Completed with errors.
  set "EXIT_CODE=1"
) else (
  echo All databases updated.
  set "EXIT_CODE=0"
)

popd
endlocal & exit /b %EXIT_CODE%
