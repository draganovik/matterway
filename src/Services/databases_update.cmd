@echo off

echo Updating databases for all API projects...

for /d %%d in (*) do (
  if exist "%%d/%%d.API" (
    echo Updating database for project: %%d
    pushd "%%d/%%d.API"
    dotnet ef database update
    popd
  )
)

echo All databases updated.
