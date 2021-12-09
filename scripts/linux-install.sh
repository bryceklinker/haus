#!/usr/bin/env bash
set -ex

HAUS_LOCATION="/home/$(whoami)/haus"
HAUS_APP_SERVICE_NAME="haus-app"
HAUS_APP_SERVICE_DEFINITION_FILE_NAME="${HAUS_LOCATION}/${HAUS_APP_SERVICE_NAME}.service"
HAUS_APP_ZIP_FILE_LOCATION="${1}"
SERVICE_DEFINITION_DIRECTORY="/etc/systemd/system"
HTTPS_CERT_PATH="${HAUS_LOCATION}/cert.pfx"
HTTPS_CERT_PASSWORD="$(uuidgen)"
ENV_FILE_PATH="${HAUS_LOCATION}/.env"

function copy_service_definition() {
  SOURCE="${HAUS_APP_SERVICE_DEFINITION_FILE_NAME}"
  TARGET="${SERVICE_DEFINITION_DIRECTORY}/${HAUS_APP_SERVICE_NAME}.service"
  
  sudo cp "${SOURCE}" "${TARGET}" 
}

function reload_daemons() {
  sudo systemctl daemon-reload
}

function start_service() {
  sudo systemctl enable "${HAUS_APP_SERVICE_NAME}"
  sudo systemctl restart "${HAUS_APP_SERVICE_NAME}"
}

function install_unzip() {
  sudo apt install unzip
}

function extract_zip_file() {
  unzip "${HAUS_APP_ZIP_FILE_LOCATION}" -d "${HAUS_LOCATION}"
}

function stop_service() {
  sudo systemctl stop "${HAUS_APP_SERVICE_NAME}" || true 
}

function generate_https_cert() {
    dotnet dev-certs https --export-path "${HTTPS_CERT_PATH}" --password "${HTTPS_CERT_PASSWORD}"
    
    rm "${ENV_FILE_PATH}" || true
    echo "HTTPS_CERT_PASSWORD=${HTTPS_CERT_PASSWORD}" >> "${ENV_FILE_PATH}"
}

function create_data_directory {
  mkdir -p "${HAUS_LOCATION}/data"
}

function main() {
  generate_https_cert
  extract_zip_file
  create_data_directory
  stop_service
  copy_service_definition
  reload_daemons
  start_service
}

main