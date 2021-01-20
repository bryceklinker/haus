#!/usr/bin/env bash
set -ex

HAUS_LOCATION="/home/$(whoami)/haus"
HAUS_WEB_LOCATION="${HAUS_LOCATION}/web"
WEB_SERVICE_NAME="haus-web"
ZIGBEE_SERVICE_NAME="haus-zigbee"
WEB_SERVICE_DEFINITION_FILE_NAME="${WEB_SERVICE_NAME}.service"
ZIGBEE_SERVICE_DEFINITION_FILE_NAME="${ZIGBEE_SERVICE_NAME}.service"
HAUS_ZIGBEE_LOCATION="${HAUS_LOCATION}/zigbee"
SERVICE_DEFINITION_DIRECTORY="/etc/systemd/system"

function copy_service_definition() {
  SOURCE="$1/$2"
  TARGET="${SERVICE_DEFINITION_DIRECTORY}/$2"
  
  sudo cp "${SOURCE}" "${TARGET}" 
}

function reload_daemons() {
  sudo systemctl daemon-reload
}

function enable_service() {
  SERVICE_NAME="$1"
  
  sudo systemctl enable "${SERVICE_NAME}"
  sudo systemctl restart "${SERVICE_NAME}"
}

function main() {
  copy_service_definition "${HAUS_WEB_LOCATION}" "${WEB_SERVICE_DEFINITION_FILE_NAME}"
  copy_service_definition "${HAUS_ZIGBEE_LOCATION}" "${ZIGBEE_SERVICE_DEFINITION_FILE_NAME}"
  
  reload_daemons
  
  enable_service "${WEB_SERVICE_NAME}"
  enable_service "${ZIGBEE_SERVICE_NAME}"
}

main