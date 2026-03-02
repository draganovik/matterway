@echo off
setlocal enabledelayedexpansion
set "HAS_ERRORS=0"

pushd "%~dp0\.."
set "SERVICES_DIR=src"

echo Dropping databases for all API projects...

for /d %%d in ("%SERVICES_DIR%\*.Api") do (
  set "SERVICE_NAME=%%~nxd"
  echo Dropping database for project: !SERVICE_NAME!
  pushd "%%d"
  dotnet ef database drop --force
  if errorlevel 1 (
    echo Failed to drop database for project: !SERVICE_NAME!
    set "HAS_ERRORS=1"
  )
  popd
)

if "!HAS_ERRORS!"=="1" (
  echo Completed with errors.
  set "EXIT_CODE=1"
) else (
  echo All databases dropped.
  set "EXIT_CODE=0"
)

popd
endlocal & exit /b %EXIT_CODE%
