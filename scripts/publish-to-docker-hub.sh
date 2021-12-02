#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function login() {
  docker login -u "${DOCKER_HUB_USERNAME}" -p "${DOCKER_HUB_ACCESS_TOKEN}"
}

function build_docker_image() {
  PUBLISH_DIRECTORY="${1}"
  APPLICATION_NAME="${2}"
  docker build --tag "${DOCKER_HUB_REPO}/${APPLICATION_NAME}-${VERSION}" \
    --file "${APPLICATION_NAME}-dockerfile" \
    --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
    .
  
  docker build --tag "${DOCKER_HUB_REPO}/${APPLICATION_NAME}-latest" \
      --file "${APPLICATION_NAME}-dockerfile" \
      --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
      .
}

function publish_docker_image() {
  APPLICATION_NAME="${1}"
  
  docker push "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-${VERSION}"
  docker push "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${APPLICATION_NAME}-latest"
}

function main() {
  login
  
  build_docker_image "publish/haus-web" "haus-web"
  build_docker_image "publish/haus-zigbee" "haus-zigbee"
  
  publish_docker_image "haus-web"
  publish_docker_image "haus-zigbee"
}

main