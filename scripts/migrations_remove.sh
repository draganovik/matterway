#!/bin/bash

set -u

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
SERVICES_DIR="${ROOT_DIR}/src/Services"

echo "Removing migrations for all API projects..."

failed_projects=()

for dir in "${SERVICES_DIR}"/*; do
  if [[ -d "${dir}" ]]; then
    service_name="$(basename "${dir}")"
    service_project="${dir}/${service_name}.API"
    if [[ -d "${service_project}" ]]; then
      echo "Removing migration for project: ${service_name}"
      if ! (cd "${service_project}" && dotnet ef migrations remove); then
        echo "Failed to remove migration for project: ${service_name}" >&2
        failed_projects+=("${service_name}")
      fi
    fi
  fi
done

if ((${#failed_projects[@]})); then
  echo "Completed with errors for: ${failed_projects[*]}" >&2
  exit 1
fi

echo "All migrations removed."
