#!/usr/bin/env bash
set -ex

# Installs an already-built haus-app .deb inside an isolated, throwaway
# systemd+Docker container (scripts/deb-verify/) instead of the real host.
# Unlike a disposable CI runner, a developer's machine is often a persistent
# box that may already be running a real HAUS install -- this avoids ever
# touching that real systemd/apt state or its containers. Run
# `VERSION=v0.0.0-dev make deb-package` (or equivalent) first to produce the
# .deb this script installs.

source ./scripts/variables.sh

CONTAINER_NAME="haus-deb-verify"
IMAGE_NAME="haus-deb-verify:local"
DEB_PATH="${PUBLISH_DIRECTORY}/installer/haus-app_${VERSION}_amd64.deb"

function build_verify_image() {
  docker build -t "${IMAGE_NAME}" -f scripts/deb-verify/Dockerfile scripts/deb-verify
}

function start_verify_container() {
  docker rm -f "${CONTAINER_NAME}" >/dev/null 2>&1 || true
  docker run -d --name "${CONTAINER_NAME}" \
    --privileged \
    --cgroupns=host \
    -v /sys/fs/cgroup:/sys/fs/cgroup:rw \
    "${IMAGE_NAME}"
  timeout 30 bash -c "until docker exec ${CONTAINER_NAME} systemctl is-system-running 2>/dev/null | grep -qE 'running|degraded'; do sleep 1; done"
}

function install_package() {
  docker cp "${DEB_PATH}" "${CONTAINER_NAME}:/root/haus-app.deb"
  docker exec "${CONTAINER_NAME}" bash -c 'apt-get update -qq && dpkg -i /root/haus-app.deb'
}

function main() {
  build_verify_image
  start_verify_container
  install_package
  echo "Installed inside ${CONTAINER_NAME}. Inspect with:"
  echo "  docker exec ${CONTAINER_NAME} systemctl status haus-app"
  echo "  docker exec ${CONTAINER_NAME} docker compose --project-directory /etc/haus ps"
  echo "Tear down with: docker rm -f ${CONTAINER_NAME}"
}

main
