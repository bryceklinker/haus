#!/usr/bin/env bash
set -ex

IS_RELEASE=${IS_RELEASE:-false}
VERSION=${VERSION:-"0.0.0"}
WORKING_DIRECTORY=$(pwd)
CONFIGURATION=${CONFIGURATION:-'Release'}
PUBLISH_DIRECTORY="${WORKING_DIRECTORY}/publish"
COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.json"
REPORT_COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.cobertura.xml"

UTILITIES_DIRECTORY="${WORKING_DIRECTORY}/src/Haus.Utilities"
FRONT_END_DIRECTORY="${WORKING_DIRECTORY}/src/Haus.Web.Host/client-app"
WEB_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Web.Host/Haus.Web.Host.csproj"
ZIGBEE_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Zigbee.Host/Haus.Zigbee.Host.csproj"

WEB_HOST_BASE_ASSET_PATH="${PUBLISH_DIRECTORY}/Haus.Web.Host"
ZIGBEE_HOST_BASE_ASSET_PATH="${PUBLISH_DIRECTORY}/Haus.Zigbee.Host"

function build_solution() {
  dotnet build /p:Version="${VERSION}"
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
  RUNTIME_IDENTIFIER=$1
  PROJECT_PATH=$2
  OUTPUT_PATH=$3
  ASSET_PATH=$4
  
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}/${RUNTIME_IDENTIFIER}" \
    --configuration "${CONFIGURATION}" \
    --runtime "${RUNTIME_IDENTIFIER}" \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    --self-contained true
    
  zip -r "${ASSET_PATH}.${RUNTIME_IDENTIFIER}.zip" "${OUTPUT_PATH}"
}

function publish_app() {
  if [ "$IS_RELEASE" = "true" ]; then
    dotnet_publish "linux-x64" "${WEB_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/web_host" "${WEB_HOST_BASE_ASSET_PATH}"
    dotnet_publish "linux-x64" "${ZIGBEE_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/zigbee_host" "${ZIGBEE_HOST_BASE_ASSET_PATH}"  
  fi
  
  
}

function main() {
  build_solution
  run_tests
  publish_app
}

main