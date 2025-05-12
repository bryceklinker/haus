#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function login() {
  echo "skipped..."
#  docker login -u "${DOCKER_HUB_USERNAME}" -p "${DOCKER_HUB_ACCESS_TOKEN}"
}

function build_service_docker_image() {
  PUBLISH_DIRECTORY="${1}"
  APPLICATION_NAME="${2}"
  ENTRY_FILE="${3}"
  
  docker build \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-${VERSION}" \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-latest" \
    --file "haus-dockerfile" \
    --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
    --build-arg ENTRY_FILE="${ENTRY_FILE}" \
    .
}

function build_site_docker_image() {
  PUBLISH_DIRECTORY="${1}"
  APPLICATION_NAME="${2}"
    
  docker build \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-${VERSION}" \
    --tag "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-latest" \
    --file "haus-site-dockerfile" \
    --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
    .
}

function publish_docker_images() {
  echo "skipped..."
#  docker push --all-tags "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}"
}

function main() {
  login
  
  build_service_docker_image "publish/haus-web" "haus-web" "Haus.Web.Host.dll"
  build_service_docker_image "publish/haus-zigbee" "haus-zigbee" "Haus.Zigbee.Host.dll"
  build_site_docker_image "publish/haus-site" "haus-site" "Haus.Zigbee.Host.dll"
  
#  publish_docker_images
}

main