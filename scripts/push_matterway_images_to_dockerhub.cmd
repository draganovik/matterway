@echo off
setlocal enabledelayedexpansion

set "SOURCE_TAG=%~1"
set "DEST_TAG=%~2"
set "NAMESPACE=%~3"

if "%SOURCE_TAG%"=="" (
  echo Usage: push_matterway_images_to_dockerhub.cmd ^<source-tag^> [dest-tag] [namespace]
  echo.
  echo Defaults:
  echo   dest-tag = latest
  echo   namespace = draganovik
  exit /b 1
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
endlocal
