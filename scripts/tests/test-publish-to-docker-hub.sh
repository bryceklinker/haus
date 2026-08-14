#!/usr/bin/env bash
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
FAILURES=0

# Runs scripts/publish-to-docker-hub.sh with a stub `docker` on PATH that
# records every invocation (one line per call, args space-joined) instead of
# touching a real daemon or registry. Sets SANDBOX_DIR for the caller to
# inspect "${SANDBOX_DIR}/docker-calls.log" and clean up. Sets SCRIPT_EXIT_CODE
# to the script's own exit code. If FAIL_SUBCOMMAND is set (e.g. "login"),
# the stub exits 1 for calls whose first argument matches it.
run_publish_script_with_stub_docker() {
  local fail_subcommand="${FAIL_SUBCOMMAND:-}"
  SANDBOX_DIR="$(mktemp -d)"
  local log="${SANDBOX_DIR}/docker-calls.log"
  : >"${log}"

  cat >"${SANDBOX_DIR}/docker" <<STUB
#!/usr/bin/env bash
echo "\$*" >>"${log}"
if [[ "\$1" == "${fail_subcommand}" && -n "${fail_subcommand}" ]]; then
  exit 1
fi
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
  SCRIPT_EXIT_CODE="$?"
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

test_push_runs_after_all_three_builds() {
  run_publish_script_with_stub_docker >/dev/null 2>&1
  local log="${SANDBOX_DIR}/docker-calls.log"
  local push_line last_build_line build_count
  push_line=$(grep -n "^push --all-tags test-user/test-repo$" "${log}" | head -1 | cut -d: -f1)
  last_build_line=$(grep -n "^build " "${log}" | tail -1 | cut -d: -f1)
  build_count=$(grep -c "^build " "${log}")
  rm -rf "${SANDBOX_DIR}"
  [[ -n "${push_line}" && "${build_count}" -eq 3 && "${push_line}" -gt "${last_build_line}" ]]
}

test_script_exits_nonzero_when_login_fails() {
  FAIL_SUBCOMMAND="login" run_publish_script_with_stub_docker >/dev/null 2>&1
  local exit_code="${SCRIPT_EXIT_CODE}"
  local build_count
  build_count=$(grep -c "^build " "${SANDBOX_DIR}/docker-calls.log")
  rm -rf "${SANDBOX_DIR}"
  [[ "${exit_code}" -ne 0 && "${build_count}" -eq 0 ]]
}

test_script_exits_nonzero_when_push_fails() {
  FAIL_SUBCOMMAND="push" run_publish_script_with_stub_docker >/dev/null 2>&1
  local exit_code="${SCRIPT_EXIT_CODE}"
  rm -rf "${SANDBOX_DIR}"
  [[ "${exit_code}" -ne 0 ]]
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
run_test test_push_runs_after_all_three_builds
run_test test_script_exits_nonzero_when_login_fails
run_test test_script_exits_nonzero_when_push_fails

if [[ "${FAILURES}" -gt 0 ]]; then
  echo "${FAILURES} test(s) failed"
  exit 1
fi
echo "All tests passed"
