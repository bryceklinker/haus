#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

export HTTPS_CERT_PATH="${WORKING_DIRECTORY}/cert.pfx"
export HTTPS_CERT_PASSWORD=""

function login() {
  docker login -u "${DOCKER_HUB_USERNAME}" -p "${DOCKER_HUB_ACCESS_TOKEN}"
}

function build_docker_image() {
  PUBLISH_DIRECTORY="${1}"
  APPLICATION_NAME="${2}"
  ENTRY_FILE="${3}"
  
  docker build \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-${VERSION}" \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-latest" \
    --file "haus-dockerfile" \
    --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
    --build-arg ENTRY_FILE="${ENTRY_FILE}" \
    --build-arg HTTPS_CERT_PASSWORD="${HTTPS_CERT_PASSWORD}" \
    .
}

function publish_docker_images() {
  docker push --all-tags "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}"
}

function generate_https_cert() {
  HTTPS_CERT_PASSWORD=$(uuidgen)
  dotnet dev-certs https --export-path "${HTTPS_CERT_PATH}" --password "${HTTPS_CERT_PASSWORD}"
  cp "${HTTPS_CERT_PATH}" "${WEB_HOST_PUBLISH_DIRECTORY}"
  cp "${HTTPS_CERT_PATH}" "${ZIGBEE_HOST_PUBLISH_DIRECTORY}"
}

function main() {
  login
  
  generate_https_cert
  
  build_docker_image "publish/haus-web" "haus-web" "Haus.Web.Host.dll"
  build_docker_image "publish/haus-zigbee" "haus-zigbee" "Haus.Zigbee.Host.dll"
  
  publish_docker_images
}

main