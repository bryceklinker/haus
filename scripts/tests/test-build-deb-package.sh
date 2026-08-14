#!/usr/bin/env bash
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
FAILURES=0

# Creates a sandbox dir with a stub `docker` on PATH that records every
# invocation (one line per call, args space-joined) to
# "${SANDBOX_DIR}/docker-calls.log" instead of touching a real daemon or
# registry. Sets the global SANDBOX_DIR for the caller to inspect and clean up.
prepare_stub_docker_sandbox() {
  SANDBOX_DIR="$(mktemp -d)"
  local log="${SANDBOX_DIR}/docker-calls.log"
  : >"${log}"

  cat >"${SANDBOX_DIR}/docker" <<STUB
#!/usr/bin/env bash
echo "\$*" >>"${log}"
exit 0
STUB
  chmod +x "${SANDBOX_DIR}/docker"
}

# Runs a snippet of bash in a fresh subprocess that has sourced
# scripts/build-deb-package.sh's function definitions (guarded so `main`
# itself never runs) with the stub docker sandbox on PATH.
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

run_test() {
  local name="$1"
  if "${name}"; then
    echo "PASS: ${name}"
  else
    echo "FAIL: ${name}"
    FAILURES=$((FAILURES + 1))
  fi
}

run_test test_remove_locally_built_images_removes_all_three_tags

if [[ "${FAILURES}" -gt 0 ]]; then
  echo "${FAILURES} test(s) failed"
  exit 1
fi
echo "All tests passed"
