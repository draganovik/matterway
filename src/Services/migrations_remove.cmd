@echo off

echo Removing Migrations for all API projects...

for /d %%d in (*) do (
  if exist "%%d/%%d.API" (
    echo Removing Migration for project: %%d
    pushd "%%d/%%d.API"
    dotnet ef migrations remove
    popd
  )
)

echo All Migrations removed.