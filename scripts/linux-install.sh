#!/usr/bin/env bash
set -ex

HAUS_LOCATION="/home/$(whoami)/haus"

WEB_SERVICE_ZIP_FILE=$1
WEB_SERVICE_LOCATION="${HAUS_LOCATION}/web"
WEB_SERVICE_NAME="haus-web"
WEB_SERVICE_DEFINITION_FILE_NAME="${WEB_SERVICE_NAME}.service"

ZIGBEE_SERVICE_ZIP_FILE=$2
ZIGBEE_SERVICE_NAME="haus-zigbee"
ZIGBEE_SERVICE_DEFINITION_FILE_NAME="${ZIGBEE_SERVICE_NAME}.service"
ZIGBEE_SERVICE_LOCATION="${HAUS_LOCATION}/zigbee"

ZIGBEE_2_MQTTT_SERVICE_ZIP_FILE=$3
ZIGBEE_2_MQTTT_SERVICE_NAME="haus-zigbee2mqtt"
ZIGBEE_2_MQTTT_SERVICE_DEFINITION_FILE_NAME="${ZIGBEE_2_MQTTT_SERVICE_NAME}.service"
ZIGBEE_2_MQTTT_SERVICE_LOCATION="${HAUS_LOCATION}/zigbee2mqtt"

SERVICE_DEFINITION_DIRECTORY="/etc/systemd/system"

function copy_service_definition() {
  SOURCE="$1/$2"
  TARGET="${SERVICE_DEFINITION_DIRECTORY}/$2"
  
  sudo cp "${SOURCE}" "${TARGET}" 
}

function reload_daemons() {
  sudo systemctl daemon-reload
}

function start_service() {
  SERVICE_NAME="$1"
  
  sudo systemctl enable "${SERVICE_NAME}"
  sudo systemctl restart "${SERVICE_NAME}"
}

function install_unzip() {
  sudo apt install unzip
}

function extract_zip_file() {
  ZIP_FILE="$1"
  DESTINATION="$2"
  
  unzip "${ZIP_FILE}" -d "${DESTINATION}"
}

function stop_service() {
  SERVICE_NAME="$1"
  
  sudo systemctl stop "${SERVICE_NAME}" || true 
}

function install_node_dependencies() {
  DIRECTORY="$1"
  pushd "${DIRECTORY}" || exit 1
  npm ci
  popd
}

function main() {
  stop_service "${WEB_SERVICE_NAME}"
  stop_service "${ZIGBEE_SERVICE_NAME}"
  stop_service "${ZIGBEE_2_MQTTT_SERVICE_NAME}"
  
  extract_zip_file "${WEB_SERVICE_ZIP_FILE}" "${WEB_SERVICE_LOCATION}"
  extract_zip_file "${ZIGBEE_SERVICE_ZIP_FILE}" "${ZIGBEE_SERVICE_LOCATION}"
  extract_zip_file "${ZIGBEE_2_MQTTT_SERVICE_ZIP_FILE}" "${ZIGBEE_2_MQTTT_SERVICE_LOCATION}"
  
  install_node_dependencies "${ZIGBEE_2_MQTTT_SERVICE_LOCATION}"
  
  copy_service_definition "${WEB_SERVICE_LOCATION}" "${WEB_SERVICE_DEFINITION_FILE_NAME}"
  copy_service_definition "${ZIGBEE_SERVICE_LOCATION}" "${ZIGBEE_SERVICE_DEFINITION_FILE_NAME}"
  copy_service_definition "${ZIGBEE_2_MQTTT_SERVICE_LOCATION}" "${ZIGBEE_2_MQTTT_SERVICE_DEFINITION_FILE_NAME}"
  
  reload_daemons
  
  start_service "${WEB_SERVICE_NAME}"
  start_service "${ZIGBEE_SERVICE_NAME}"
  start_service "${ZIGBEE_2_MQTTT_SERVICE_NAME}"
}

main