#!/bin/bash

set -euo pipefail

SERVICES=(
  "mtw-catalog-api"
  "mtw-customers-api"
  "mtw-dashboard-web"
  "mtw-db-migrator"
  "mtw-identity-api"
  "mtw-sales-api"
  "mtw-storefront-web"
)

usage() {
  cat <<'EOF'
Usage:
  push_matterway_images_to_dockerhub.sh <source-tag> [dest-tag] [namespace]

Examples:
  push_matterway_images_to_dockerhub.sh aspire-deploy-20260321165222
  push_matterway_images_to_dockerhub.sh aspire-deploy-20260321165222 latest draganovik
  push_matterway_images_to_dockerhub.sh aspire-deploy-20260321165222 v1.0.0 your-org

Defaults:
  dest-tag = latest
  namespace = draganovik

This retags local images like:
  mtw-catalog-api:<source-tag>

and pushes them to:
  <namespace>/mtw-catalog-api:<dest-tag>
EOF
}

SOURCE_TAG="${1:-}"
DEST_TAG="${2:-latest}"
NAMESPACE="${3:-draganovik}"

if [[ -z "${SOURCE_TAG}" ]]; then
  usage >&2
  exit 1
fi

for service in "${SERVICES[@]}"; do
  local_image="${service}:${SOURCE_TAG}"
  remote_image="${NAMESPACE}/${service}:${DEST_TAG}"

  if ! docker image inspect "${local_image}" >/dev/null 2>&1; then
    echo "Missing local image: ${local_image}" >&2
    echo "Build it first, then rerun this script." >&2
    exit 1
  fi

  echo "Tagging ${local_image} -> ${remote_image}"
  docker tag "${local_image}" "${remote_image}"

  echo "Pushing ${remote_image}"
  docker push "${remote_image}"
done

echo "Done."
