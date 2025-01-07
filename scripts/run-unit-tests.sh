#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function run_dotnet_test() {
  PROJECT_NAME=$1
  PROJECT_PATH="${WORKING_DIRECTORY}/tests/${PROJECT_NAME}"
  dotnet coverlet "${PROJECT_PATH}/bin/Debug/net9.0/${PROJECT_NAME}.dll" \
    --target "dotnet" \
    --targetargs "test ${PROJECT_PATH} --no-build" \
    --merge-with "${COVERAGE_FILE_PATH}" \
    --format cobertura \
    --format json \
    --exclude "[Haus.Testing.*]*" \
    --exclude "[*]Haus.Core.Common.Storage.Migrations*"
}

function main() {
  dotnet tool restore
  
  run_dotnet_test "Haus.Core.Models.Tests"
  run_dotnet_test "Haus.Core.Tests"
  run_dotnet_test "Haus.Utilities.Tests"
  run_dotnet_test "Haus.Mqtt.Client.Tests"
  run_dotnet_test "Haus.Web.Host.Tests"
  run_dotnet_test "Haus.Zigbee.Host.Tests"
  
  
  dotnet reportgenerator \
    "-reports:${REPORT_COVERAGE_FILE_PATH}" \
    "-targetdir:coveragereport" \
    "-reporttypes:Html"
    
  yarn web_host_client:test
}

main