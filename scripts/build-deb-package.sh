#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

PACKAGING_SOURCE_DIRECTORY="${WORKING_DIRECTORY}/packaging/deb"
DEB_STAGE_DIRECTORY="${PUBLISH_DIRECTORY}/installer/haus-app-deb-stage"
DEB_OUTPUT_PATH="${PUBLISH_DIRECTORY}/installer/haus-app_${VERSION}_amd64.deb"

function stage_package_tree() {
  rm -rf "${DEB_STAGE_DIRECTORY}"
  mkdir -p "${DEB_STAGE_DIRECTORY}"
  cp -r "${PACKAGING_SOURCE_DIRECTORY}/." "${DEB_STAGE_DIRECTORY}/"
}

function render_pinned_compose_file() {
  pushd "${DEB_STAGE_DIRECTORY}" || exit 1
    VERSION="${VERSION}" dotnet run --project "${WORKING_DIRECTORY}/src/Haus.Utilities" -- packaging render-deb-compose
    rm "etc/haus/docker-compose.yml.template"
  popd || exit 1
}

function set_control_version() {
  sed -i "s/^Version: .*/Version: ${VERSION}/" "${DEB_STAGE_DIRECTORY}/DEBIAN/control"
}

function build_deb() {
  mkdir -p "${PUBLISH_DIRECTORY}/installer"
  dpkg-deb --build --root-owner-group "${DEB_STAGE_DIRECTORY}" "${DEB_OUTPUT_PATH}"
}

function main() {
  stage_package_tree
  render_pinned_compose_file
  set_control_version
  build_deb
}

main
