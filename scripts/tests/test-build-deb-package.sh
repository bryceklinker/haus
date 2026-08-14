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

# Runs main() with every step except remove_locally_built_images stubbed to
# just log its own name to the (shared) docker-calls.log -- so the real
# remove_locally_built_images's actual `docker rmi` calls land in the same
# log, in the same order they really ran in, alongside its siblings.
run_main_with_sibling_steps_stubbed() {
  local log="${SANDBOX_DIR}/docker-calls.log"
  run_with_sourced_build_deb_package "
    stage_package_tree() { echo stage_package_tree >>'${log}'; }
    render_pinned_compose_file() { echo render_pinned_compose_file >>'${log}'; }
    set_control_version() { echo set_control_version >>'${log}'; }
    build_deb() { echo build_deb >>'${log}'; }
    install_and_smoke_test() { echo install_and_smoke_test >>'${log}'; }
    main
  "
}

test_remove_locally_built_images_runs_before_install_and_smoke_test() {
  prepare_stub_docker_sandbox
  run_main_with_sibling_steps_stubbed >/dev/null 2>&1
  local log="${SANDBOX_DIR}/docker-calls.log"
  local last_rmi_line install_line
  last_rmi_line=$(grep -n "^rmi " "${log}" | tail -1 | cut -d: -f1)
  install_line=$(grep -n "^install_and_smoke_test$" "${log}" | head -1 | cut -d: -f1)
  rm -rf "${SANDBOX_DIR}"
  [[ -n "${last_rmi_line}" && -n "${install_line}" && "${last_rmi_line}" -lt "${install_line}" ]]
}

run_test test_remove_locally_built_images_removes_all_three_tags
run_test test_remove_locally_built_images_runs_before_install_and_smoke_test

report_results_and_exit
