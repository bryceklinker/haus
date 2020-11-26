#!/usr/bin/env bash
set -ex

CONFIGURATION=${CONFIGURATION:-'Release'}
PUBLISH_DIRECTORY=${PUBLISH_DIRECTORY:-'./publish'}

FRONT_END_DIRECTORY="./src/Haus.Web.Host/client-app"
WEB_HOST_PROJECT="./src/Haus.Web.Host/Haus.Web.Host.csproj"
ZIGBEE_HOST_PROJECT="./src/Haus.Zigbee.Host/Haus.Zigbee.Host.csproj"

function build_solution() {
  dotnet build --configuration "${CONFIGURATION}"
}

function run_tests() {
  dotnet test
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
  dotnet_publish $WEB_HOST_PROJECT "${PUBLISH_DIRECTORY}/web_host"
  dotnet_publish $ZIGBEE_HOST_PROJECT "${PUBLISH_DIRECTORY}/zigbee_host"
}

function main() {
  build_solution
  run_tests
  publish_app
}

main