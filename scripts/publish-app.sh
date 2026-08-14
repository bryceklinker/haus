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

function package_service() {
  PACKAGE_DIRECTORY="${PUBLISH_DIRECTORY}/installer"
  mkdir -p "${PACKAGE_DIRECTORY}"
  
  cp "${WORKING_DIRECTORY}/haus-app.service" "${PACKAGE_DIRECTORY}/"
  cp "${WORKING_DIRECTORY}/docker-compose.yml" "${PACKAGE_DIRECTORY}/"
  cp "${WORKING_DIRECTORY}/mosquitto.conf" "${PACKAGE_DIRECTORY}/"
  
  pushd "${PACKAGE_DIRECTORY}" || exit 1
    zip -rm "../service_package.zip" -- *
  popd || exit 1
}

function main() {
  dotnet_publish "${SITE_HOST_PROJECT}" "${SITE_HOST_PUBLISH_DIRECTORY}"
  dotnet_publish "${WEB_HOST_PROJECT}" "${WEB_HOST_PUBLISH_DIRECTORY}"
  dotnet_publish "${ZIGBEE_HOST_PROJECT}" "${ZIGBEE_HOST_PUBLISH_DIRECTORY}"
  # Local/CI only -- the fake dongle simulator is never part of the production service package.
  dotnet_publish "${ZIGBEE_SIMULATOR_PROJECT}" "${ZIGBEE_SIMULATOR_PUBLISH_DIRECTORY}"

  package_service
}

main