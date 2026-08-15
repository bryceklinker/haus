#!/usr/bin/env bash
# Shared harness for scripts/tests/*.sh: a stub `docker` on PATH that
# records invocations instead of touching a real daemon or registry, plus a
# minimal pass/fail test runner. Source this, don't execute it.

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
FAILURES=0

# Creates a sandbox dir with a stub `docker` on PATH that records every
# invocation (one line per call, args space-joined) to
# "${SANDBOX_DIR}/docker-calls.log". Sets the global SANDBOX_DIR for the
# caller to inspect and clean up. If FAIL_SUBCOMMAND is set (e.g. "login"),
# the stub exits 1 for calls whose first argument matches it.
prepare_stub_docker_sandbox() {
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

report_results_and_exit() {
  if [[ "${FAILURES}" -gt 0 ]]; then
    echo "${FAILURES} test(s) failed"
    exit 1
  fi
  echo "All tests passed"
}
