#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function dotnet_publish() {
  PROJECT_PATH="${1}"
  OUTPUT_PATH="${2}"
  
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}" \
    --configuration "${CONFIGURATION}" \
    -p:Version="${VERSION}"
}

function main() {
  if [ "$IS_RELEASE" = "true" ]; then
    dotnet_publish "${WEB_HOST_PROJECT}" "${WEB_HOST_PUBLISH_DIRECTORY}"
    dotnet_publish "${ZIGBEE_HOST_PROJECT}" "${ZIGBEE_HOST_PUBLISH_DIRECTORY}"
  fi
}

main