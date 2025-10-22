#!/bin/bash

set -u

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
API_ROOT="${ROOT_DIR}/src"

echo "Adding migrations for all API projects..."

failed_projects=()

shopt -s nullglob
for project_dir in "${API_ROOT}"/*.Api; do
  if [[ -d "${project_dir}" ]]; then
    project_name="$(basename "${project_dir}")"
    echo "Adding migration for project: ${project_name}"
    if ! (cd "${project_dir}" && dotnet ef migrations add "Initialize"); then
      echo "Failed to add migration for project: ${project_name}" >&2
      failed_projects+=("${project_name}")
    fi
  fi
done
shopt -u nullglob

if ((${#failed_projects[@]})); then
  echo "Completed with errors for: ${failed_projects[*]}" >&2
  exit 1
fi

echo "All migrations added."
