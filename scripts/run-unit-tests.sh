#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

function main() {
  dotnet tool restore

  rm -rf "${TEST_RESULTS_DIRECTORY}"

  dotnet test Haus.slnx \
    --no-build \
    --filter "TestCategory!=Acceptance" \
    --settings .runsettings \
    --collect:"XPlat Code Coverage" \
    --results-directory "${TEST_RESULTS_DIRECTORY}"

  dotnet reportgenerator \
    "-reports:${COVERAGE_REPORT_GLOB}" \
    "-targetdir:coveragereport" \
    "-reporttypes:Html"
}

main
