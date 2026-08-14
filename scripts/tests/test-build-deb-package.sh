#!/usr/bin/env bash
set -uo pipefail

source "$(dirname "${BASH_SOURCE[0]}")/lib/test_harness.sh"

# Runs a snippet of bash in a fresh subprocess that has sourced
# scripts/build-deb-package.sh's function definitions (guarded so `main`
# itself never runs) with the stub docker sandbox prepared by
# prepare_stub_docker_sandbox on PATH.
run_with_sourced_build_deb_package() {
  local snippet="$1"
  (
    cd "${REPO_ROOT}" &&
      PATH="${SANDBOX_DIR}:${PATH}" \
        DOCKER_HUB_USERNAME="test-user" \
        DOCKER_HUB_REPO="test-repo" \
        VERSION="v1.2.3" \
        bash -c "source scripts/build-deb-package.sh && ${snippet}"
  )
}

test_remove_locally_built_images_removes_all_three_tags() {
  prepare_stub_docker_sandbox
  run_with_sourced_build_deb_package "remove_locally_built_images" >/dev/null 2>&1
  local log="${SANDBOX_DIR}/docker-calls.log"
  local result=0
  # VERSION is set to "v1.2.3" above; scripts/variables.sh strips the leading
  # "v" (matching the tags publish-to-docker-hub.sh actually pushes).
  grep -qx "rmi test-user/test-repo:haus-web-1.2.3" "${log}" || result=1
  grep -qx "rmi test-user/test-repo:haus-zigbee-1.2.3" "${log}" || result=1
  grep -qx "rmi test-user/test-repo:haus-site-1.2.3" "${log}" || result=1
  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

test_remove_locally_built_images_runs_before_install_and_smoke_test() {
  local main_body
  main_body=$(sed -n '/^function main()/,/^}/p' "${REPO_ROOT}/scripts/build-deb-package.sh")
  local remove_line install_line
  remove_line=$(echo "${main_body}" | grep -n "remove_locally_built_images" | head -1 | cut -d: -f1)
  install_line=$(echo "${main_body}" | grep -n "install_and_smoke_test" | head -1 | cut -d: -f1)
  [[ -n "${remove_line}" && -n "${install_line}" && "${remove_line}" -lt "${install_line}" ]]
}

run_test test_remove_locally_built_images_removes_all_three_tags
run_test test_remove_locally_built_images_runs_before_install_and_smoke_test

report_results_and_exit
