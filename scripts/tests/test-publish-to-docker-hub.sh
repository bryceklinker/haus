#!/usr/bin/env bash
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
FAILURES=0

# Runs scripts/publish-to-docker-hub.sh with a stub `docker` on PATH that
# records every invocation (one line per call, args space-joined) instead of
# touching a real daemon or registry. Sets SANDBOX_DIR for the caller to
# inspect "${SANDBOX_DIR}/docker-calls.log" and clean up.
run_publish_script_with_stub_docker() {
  SANDBOX_DIR="$(mktemp -d)"
  local log="${SANDBOX_DIR}/docker-calls.log"
  : >"${log}"

  cat >"${SANDBOX_DIR}/docker" <<STUB
#!/usr/bin/env bash
echo "\$*" >>"${log}"
exit 0
STUB
  chmod +x "${SANDBOX_DIR}/docker"

  (
    cd "${REPO_ROOT}" &&
      PATH="${SANDBOX_DIR}:${PATH}" \
        DOCKER_HUB_USERNAME="test-user" \
        DOCKER_HUB_ACCESS_TOKEN="test-token" \
        DOCKER_HUB_REPO="test-repo" \
        VERSION="v1.2.3" \
        bash scripts/publish-to-docker-hub.sh
  )
}

test_login_runs_before_any_build() {
  run_publish_script_with_stub_docker >/dev/null 2>&1
  local log="${SANDBOX_DIR}/docker-calls.log"
  local login_line build_line
  login_line=$(grep -n "^login -u test-user -p test-token$" "${log}" | head -1 | cut -d: -f1)
  build_line=$(grep -n "^build " "${log}" | head -1 | cut -d: -f1)
  rm -rf "${SANDBOX_DIR}"
  [[ -n "${login_line}" && -n "${build_line}" && "${login_line}" -lt "${build_line}" ]]
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

run_test test_login_runs_before_any_build

if [[ "${FAILURES}" -gt 0 ]]; then
  echo "${FAILURES} test(s) failed"
  exit 1
fi
echo "All tests passed"
