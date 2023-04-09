@echo off

echo Adding Migrations for all API projects...

for /d %%d in (*) do (
  if exist "%%d/%%d.API" (
    echo Adding Migration for project: %%d
    pushd "%%d/%%d.API"
    dotnet ef migrations add "Initialize"
    popd
  )
)

echo All Migration added.
