#!/usr/bin/env bash
set -ex

WORKING_DIRECTORY=$(pwd)
CONFIGURATION=${CONFIGURATION:-'Release'}
PUBLISH_DIRECTORY="${WORKING_DIRECTORY}/publish"
COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.json"
REPORT_COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.cobertura.xml"
IS_RELEASE=${IS_RELEASE:-false}

UTILITIES_DIRECTORY="${WORKING_DIRECTORY}/src/Haus.Utilities"
FRONT_END_DIRECTORY="${WORKING_DIRECTORY}/src/Haus.Web.Host/client-app"
WEB_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Web.Host/Haus.Web.Host.csproj"
ZIGBEE_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Zigbee.Host/Haus.Zigbee.Host.csproj"

function build_solution() {
  dotnet build
}

function run_dotnet_test() {
  PROJECT_NAME=$1
  PROJECT_PATH="${WORKING_DIRECTORY}/tests/${PROJECT_NAME}"
  dotnet coverlet "${PROJECT_PATH}/bin/Debug/net5.0/${PROJECT_NAME}.dll" \
    --target "dotnet" \
    --targetargs "test ${PROJECT_PATH} --no-build" \
    --merge-with "${COVERAGE_FILE_PATH}" \
    --format cobertura \
    --format json \
    --exclude "[Haus.Testing.*]*"
}

function generate_typescript_models() {
  pushd "${UTILITIES_DIRECTORY}" || exit
    dotnet run typescript generate-models
  popd
}

function run_tests() {
  dotnet tool restore
  
  run_dotnet_test "Haus.Core.Tests"
  run_dotnet_test "Haus.Utilities.Tests"
  run_dotnet_test "Haus.Web.Host.Tests"
  run_dotnet_test "Haus.Zigbee.Host.Tests"
  
  dotnet reportgenerator \
    "-reports:${REPORT_COVERAGE_FILE_PATH}" \
    "-targetdir:coveragereport" \
    "-reporttypes:Html"
    
  pushd "${FRONT_END_DIRECTORY}" || exit
    yarn install
    yarn test
  popd || exit
}

function dotnet_publish() {
  PROJECT_PATH=$1
  OUTPUT_PATH=$2
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}/linux-x64" \
    --configuration "${CONFIGURATION}" \
    --runtime "linux-x64" \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    --self-contained true
    
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}/osx-x64" \
    --configuration "${CONFIGURATION}" \
    --runtime "osx-x64" \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    --self-contained true
    
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}/win-x64" \
    --configuration "${CONFIGURATION}" \
    --runtime "win-x64" \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    --self-contained true
}

function publish_app() {
  if [ "${IS_RELEASE}" == "false" ]; then
    exit 0
  fi
  
  dotnet_publish $WEB_HOST_PROJECT "${PUBLISH_DIRECTORY}/web_host"
  dotnet_publish $ZIGBEE_HOST_PROJECT "${PUBLISH_DIRECTORY}/zigbee_host"
}

function main() {
  build_solution
  run_tests
  publish_app
}

main