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

if /I not "%~1"=="push" (
  call :usage 1
  goto :end
)

set "SOURCE_TAG=%~2"
if /I "%SOURCE_TAG%"=="help" (
  call :usage 0
  goto :end
)
if /I "%SOURCE_TAG%"=="-h" (
  call :usage 0
  goto :end
)
if /I "%SOURCE_TAG%"=="--help" (
  call :usage 0
  goto :end
)
if /I "%SOURCE_TAG%"=="/?" (
  call :usage 0
  goto :end
)

set "DEST_TAG=%~3"
set "NAMESPACE=%~4"

if "%SOURCE_TAG%"=="" (
  call :usage 1
  goto :end
)

if "%DEST_TAG%"=="" set "DEST_TAG=latest"
if "%NAMESPACE%"=="" set "NAMESPACE=draganovik"

set "SERVICES=mtw-catalog-api mtw-customers-api mtw-dashboard-web mtw-db-migrator mtw-identity-api mtw-sales-api mtw-storefront-web"

for %%S in (%SERVICES%) do (
  set "LOCAL_IMAGE=%%S:%SOURCE_TAG%"
  set "REMOTE_IMAGE=%NAMESPACE%/%%S:%DEST_TAG%"

  docker image inspect "!LOCAL_IMAGE!" >nul 2>&1
  if errorlevel 1 (
    echo Missing local image: !LOCAL_IMAGE!
    echo Build it first, then rerun this script.
    exit /b 1
  )

  echo Tagging !LOCAL_IMAGE! -> !REMOTE_IMAGE!
  docker tag "!LOCAL_IMAGE!" "!REMOTE_IMAGE!"

  echo Pushing !REMOTE_IMAGE!
  docker push "!REMOTE_IMAGE!"
)

echo Done.
goto :end

:usage
set "EXIT_CODE=%~1"
echo Usage: %SCRIPT_NAME% push ^<source-tag^> [dest-tag] [namespace]
echo        %SCRIPT_NAME% help^|-h^|--help^|/?
echo        %SCRIPT_NAME% push help^|-h^|--help^|/?
echo.
echo Examples:
echo   %SCRIPT_NAME% push aspire-deploy-20260321165222
echo   %SCRIPT_NAME% push aspire-deploy-20260321165222 latest draganovik
echo   %SCRIPT_NAME% push aspire-deploy-20260321165222 v1.0.0 your-org
echo.
echo Defaults:
echo   dest-tag = latest
echo   namespace = draganovik
echo.
echo This retags local images like:
echo   mtw-catalog-api:^<source-tag^>
echo.
echo and pushes them to:
echo   ^<namespace^>/mtw-catalog-api:^<dest-tag^>
exit /b

:end
endlocal & exit /b %EXIT_CODE%
