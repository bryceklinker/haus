#!/usr/bin/env bash
set -ex

export CYPRESS_AUTH_PASSWORD="${AUTH_PASSWORD}"
export CYPRESS_AUTH_USERNAME="${AUTH_USERNAME}"
IS_RELEASE=${IS_RELEASE:-false}
VERSION=${VERSION:-"v0.0.0"}
VERSION=${VERSION:1}
WORKING_DIRECTORY=$(pwd)
CONFIGURATION=${CONFIGURATION:-'Release'}
PUBLISH_DIRECTORY="${WORKING_DIRECTORY}/publish"
COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.json"
REPORT_COVERAGE_FILE_PATH="${WORKING_DIRECTORY}/coverage.cobertura.xml"

UTILITIES_DIRECTORY="${WORKING_DIRECTORY}/src/Haus.Utilities"
WEB_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Web.Host/Haus.Web.Host.csproj"
ZIGBEE_HOST_PROJECT="${WORKING_DIRECTORY}/src/Haus.Zigbee.Host/Haus.Zigbee.Host.csproj"

WEB_HOST_BASE_ASSET_PATH="Haus.Web.Host"
ZIGBEE_HOST_BASE_ASSET_PATH="Haus.Zigbee.Host"

function install_node_packages() {
  yarn install
}

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
    --exclude "[Haus.Testing.*]*" \
    --exclude "[*]Haus.Core.Common.Storage.Migrations*"
}

function generate_typescript_models() {
  pushd "${UTILITIES_DIRECTORY}" || exit
    dotnet run typescript generate-models
  popd
}

function run_tests() {
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

function run_acceptance_tests() {
  yarn acceptance_tests:test
}

function dotnet_publish() {
  RUNTIME_IDENTIFIER=$1
  PROJECT_PATH=$2
  OUTPUT_PATH="${3}/${RUNTIME_IDENTIFIER}"
  ASSET_PATH=$4
  
  dotnet publish "${PROJECT_PATH}" \
    --output "${OUTPUT_PATH}" \
    --configuration "${CONFIGURATION}" \
    --runtime "${RUNTIME_IDENTIFIER}" \
    --self-contained true \
    -p:Version="${VERSION}"
    
   pushd "${OUTPUT_PATH}" || exit
    zip -rm "../../${ASSET_PATH}.${RUNTIME_IDENTIFIER}.zip" -- *
   popd 
}

function create_zigbee2mqtt_linux_package() {
  sudo apt-get install -y make g++ gcc
  
  ZIGBEE_2_MQTT_PACKAGE_DIRECTORY="${PUBLISH_DIRECTORY}/zigbee2mqtt"
  git clone https://github.com/Koenkk/zigbee2mqtt.git "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}"
  cd "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}"
  npm ci
  mkdir -p "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/data"
  cp "${WORKING_DIRECTORY}/zigbee2mqtt/configuration.yaml" "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/configuration.yaml"
  cp "${WORKING_DIRECTORY}/zigbee2mqtt/zigbee2mqtt.service" "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/zigbee2mqtt.service"
  
  pushd "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}" || exit 
    zip -rm "../haus-linux-zigbee2mqtt.zip" -- *
  popd
}

function publish_app() {
  create_zigbee2mqtt_linux_package
  if [ "$IS_RELEASE" = "true" ]; then
    dotnet_publish "linux-x64" "${WEB_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/web_host" "${WEB_HOST_BASE_ASSET_PATH}"
    dotnet_publish "win-x64" "${WEB_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/web_host" "${WEB_HOST_BASE_ASSET_PATH}"
    dotnet_publish "osx-x64" "${WEB_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/web_host" "${WEB_HOST_BASE_ASSET_PATH}"
    
    dotnet_publish "linux-x64" "${ZIGBEE_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/zigbee_host" "${ZIGBEE_HOST_BASE_ASSET_PATH}"
    dotnet_publish "win-x64" "${ZIGBEE_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/zigbee_host" "${ZIGBEE_HOST_BASE_ASSET_PATH}"
    dotnet_publish "osx-x64" "${ZIGBEE_HOST_PROJECT}" "${PUBLISH_DIRECTORY}/zigbee_host" "${ZIGBEE_HOST_BASE_ASSET_PATH}"
    
    create_zigbee2mqtt_linux_package
  fi
}

function main() {
  install_node_packages
  build_solution
  run_tests
  run_acceptance_tests
  publish_app
}

main