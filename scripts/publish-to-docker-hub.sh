#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function login() {
  docker login -u "${DOCKER_HUB_USERNAME}" -p "${DOCKER_HUB_ACCESS_TOKEN}"
}

function build_docker_image() {
  PUBLISH_DIRECTORY="${1}"
  APPLICATION_NAME="${2}"
  docker build --tag "haus/${APPLICATION_NAME}:${VERSION}" \
    --file "${APPLICATION_NAME}-dockerfile" \
    --build-arg PUBLISH_DIR="${PUBLISH_DIRECTORY}" \
    .
}

function main() {
  login
  build_docker_image "${WEB_HOST_PUBLISH_DIRECTORY}" "haus-web"
  build_docker_image "${ZIGBEE_HOST_PUBLISH_DIRECTORY}" "haus-zigbee"
}