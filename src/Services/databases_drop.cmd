@echo off

echo Dropping databases for all API projects...

for /d %%d in (*) do (
  if exist "%%d/%%d.API" (
    echo Dropping database for project: %%d
    pushd "%%d/%%d.API"
    dotnet ef database drop --force
    popd
  )
)

echo All databases droped.
