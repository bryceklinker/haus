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

function remove_locally_built_images() {
  for service in haus-web haus-zigbee haus-site; do
    docker rmi "${DOCKER_HUB_USERNAME}/${DOCKER_HUB_REPO}:${service}-${VERSION}"
  done
}

# This is the pipeline's gate: there's no separate staging environment to
# promote through (HAUS is self-hosted, installed by an operator, not
# deployed by this pipeline), so installing for real on the runner itself
# substitutes for one. GitHub-hosted ubuntu-* runners are already disposable
# systemd+Docker VMs discarded at job end, so installing directly here is
# safe -- unlike a persistent dev machine, there's no risk of colliding with
# a real running HAUS install (see scripts/deb-verify/ for a nested,
# isolated way to exercise this on a persistent machine instead).
function install_and_smoke_test() {
  sudo apt-get install -y "${DEB_OUTPUT_PATH}"

  # No wait on the unit's own active/failed state here: haus-app.service's
  # best-effort restart (see postinst) can still surface an unexpected
  # failure, so the real gate is the per-container polling below, not
  # systemctl's unit-level verdict. haus_mqtt/haus_web/haus_site have no
  # external hardware dependency and must come up; haus_zigbee needs a
  # physical Zigbee dongle at /dev/ttyACM0 that a build runner won't have,
  # so it's checked but not required -- its ExecStart is isolated from the
  # other three in haus-app.service specifically so a missing dongle can't
  # race with and block their startup.
  for service in haus_mqtt haus_web haus_site; do
    timeout 90 bash -c "until [ -n \"\$(docker compose --project-directory /etc/haus ps --status running -q ${service})\" ]; do sleep 2; done"
  done
  docker compose --project-directory /etc/haus ps haus_zigbee || true

  timeout 60 bash -c 'until (exec 3<>/dev/tcp/localhost/5001) 2>/dev/null; do sleep 2; done'

  sudo apt-get purge -y haus-app
}

function main() {
  stage_package_tree
  render_pinned_compose_file
  set_control_version
  build_deb
  install_and_smoke_test
}

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
  main
fi
