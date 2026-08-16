#!/usr/bin/env bash
set -uo pipefail

source "$(dirname "${BASH_SOURCE[0]}")/lib/test_harness.sh"

# Builds a sandbox with $SANDBOX_DIR/legacy (the pre-.deb manual install
# layout) and $SANDBOX_DIR/data (standing in for $DATA_DIR, pre-populated
# the way postinst's `mkdir -p` call does before migrate_legacy_data runs).
prepare_migration_sandbox() {
  SANDBOX_DIR="$(mktemp -d)"
  mkdir -p "${SANDBOX_DIR}/data/haus_web/logs" \
           "${SANDBOX_DIR}/data/haus_zigbee/logs" \
           "${SANDBOX_DIR}/data/haus_mqtt/data" \
           "${SANDBOX_DIR}/data/haus_mqtt/log"
}

# Runs a snippet of bash that has sourced postinst's function definitions
# (its own case "$1" in ... esac never matches with no positional args, so
# sourcing never runs the real configure steps), with DATA_DIR/the marker
# prefix redirected into the sandbox and find_legacy_data_dir stubbed to
# point at the sandbox's fake legacy dir instead of scanning real /home/*.
run_with_sourced_postinst() {
  local snippet="$1"
  (
    cd "${REPO_ROOT}" &&
      bash -c "
        source packaging/deb/DEBIAN/postinst
        DATA_DIR='${SANDBOX_DIR}/data'
        LEGACY_MIGRATION_MARKER='${SANDBOX_DIR}/data/.legacy-data-migrated'
        find_legacy_data_dir() { echo '${SANDBOX_DIR}/legacy'; }
        ${snippet}
      "
  )
}

test_first_time_migration_copies_every_subdir_and_sets_its_marker() {
  prepare_migration_sandbox
  mkdir -p "${SANDBOX_DIR}/legacy/haus_web" "${SANDBOX_DIR}/legacy/haus_zigbee" "${SANDBOX_DIR}/legacy/haus_mqtt/data"
  echo "legacy-db" >"${SANDBOX_DIR}/legacy/haus_web/haus.db"
  echo "legacy-state" >"${SANDBOX_DIR}/legacy/haus_zigbee/state.json"
  echo "legacy-mqtt" >"${SANDBOX_DIR}/legacy/haus_mqtt/data/mosquitto.db"

  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1

  local result=0
  [ "$(cat "${SANDBOX_DIR}/data/haus_web/haus.db" 2>/dev/null)" = "legacy-db" ] || result=1
  [ "$(cat "${SANDBOX_DIR}/data/haus_zigbee/state.json" 2>/dev/null)" = "legacy-state" ] || result=1
  [ "$(cat "${SANDBOX_DIR}/data/haus_mqtt/data/mosquitto.db" 2>/dev/null)" = "legacy-mqtt" ] || result=1
  [ -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_web" ] || result=1
  [ -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_zigbee" ] || result=1
  [ -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_mqtt" ] || result=1

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

test_legacy_zigbee2mqtt_directory_migrates_into_haus_zigbee() {
  prepare_migration_sandbox
  mkdir -p "${SANDBOX_DIR}/legacy/haus_zigbee2mqtt"
  echo "old-zigbee2mqtt-state" >"${SANDBOX_DIR}/legacy/haus_zigbee2mqtt/state.json"

  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1

  local result=0
  [ "$(cat "${SANDBOX_DIR}/data/haus_zigbee/state.json" 2>/dev/null)" = "old-zigbee2mqtt-state" ] || result=1
  [ -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_zigbee" ] || result=1

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

# Reproduces the real-world lockout: an earlier .deb boot already had
# Haus.Web.Host auto-create haus_web/haus.db before migration logic ever
# ran, so the destination has stray content that isn't the real legacy db.
test_pre_existing_destination_data_is_not_overwritten_and_marker_is_not_set() {
  prepare_migration_sandbox
  echo "auto-created-empty-db" >"${SANDBOX_DIR}/data/haus_web/haus.db"
  mkdir -p "${SANDBOX_DIR}/legacy/haus_web"
  echo "real-legacy-db" >"${SANDBOX_DIR}/legacy/haus_web/haus.db"

  local stderr_output
  stderr_output=$(run_with_sourced_postinst "migrate_legacy_data" 2>&1 >/dev/null)

  local result=0
  [ "$(cat "${SANDBOX_DIR}/data/haus_web/haus.db")" = "auto-created-empty-db" ] || result=1
  [ ! -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_web" ] || result=1
  case "${stderr_output}" in
    *haus_web*) : ;;
    *) result=1 ;;
  esac

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

# The bug this fixes: once an operator clears the stray file, a later
# postinst run (e.g. a package reinstall/upgrade) must still be able to
# complete the migration -- not stay locked out by an earlier skip.
test_skip_does_not_permanently_lock_out_a_later_successful_migration() {
  prepare_migration_sandbox
  echo "auto-created-empty-db" >"${SANDBOX_DIR}/data/haus_web/haus.db"
  mkdir -p "${SANDBOX_DIR}/legacy/haus_web"
  echo "real-legacy-db" >"${SANDBOX_DIR}/legacy/haus_web/haus.db"
  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1

  rm "${SANDBOX_DIR}/data/haus_web/haus.db"
  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1

  local result=0
  [ "$(cat "${SANDBOX_DIR}/data/haus_web/haus.db" 2>/dev/null)" = "real-legacy-db" ] || result=1
  [ -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_web" ] || result=1

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

test_marker_present_skips_without_recopying() {
  prepare_migration_sandbox
  mkdir -p "${SANDBOX_DIR}/legacy/haus_web"
  echo "legacy-db" >"${SANDBOX_DIR}/legacy/haus_web/haus.db"
  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1
  echo "newer-real-data" >"${SANDBOX_DIR}/data/haus_web/haus.db"

  run_with_sourced_postinst "migrate_legacy_data" >/dev/null 2>&1

  local result=0
  [ "$(cat "${SANDBOX_DIR}/data/haus_web/haus.db")" = "newer-real-data" ] || result=1

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

test_no_legacy_dir_is_a_clean_noop() {
  prepare_migration_sandbox

  run_with_sourced_postinst "
    find_legacy_data_dir() { return 1; }
    migrate_legacy_data
  " >/dev/null 2>&1
  local exit_code=$?

  local result=0
  [ "${exit_code}" -eq 0 ] || result=1
  [ ! -f "${SANDBOX_DIR}/data/.legacy-data-migrated-haus_web" ] || result=1

  rm -rf "${SANDBOX_DIR}"
  return "${result}"
}

run_test test_first_time_migration_copies_every_subdir_and_sets_its_marker
run_test test_legacy_zigbee2mqtt_directory_migrates_into_haus_zigbee
run_test test_pre_existing_destination_data_is_not_overwritten_and_marker_is_not_set
run_test test_skip_does_not_permanently_lock_out_a_later_successful_migration
run_test test_marker_present_skips_without_recopying
run_test test_no_legacy_dir_is_a_clean_noop

report_results_and_exit
