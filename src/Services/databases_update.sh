#!/bin/bash

echo "Updating databases for all API projects..."

for dir in *; do
  if [[ -d "${dir}" && -d "${dir}/${dir}.API" ]]; then
    echo "Updating database for project: ${dir}"
    pushd "${dir}/${dir}.API" > /dev/null
    dotnet ef database update
    popd > /dev/null
  fi
done

echo "All databases updated."
