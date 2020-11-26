#!/usr/bin/env bash
set -ex

CONFIGURATION=${CONFIGURATION:-'Release'}
PUBLISH_DIRECTORY=${PUBLISH_DIRECTORY:-'./publish'}
PUBLISH_RUNTIME=${PUBLISH_RUNTIME:-'linux-x64'}

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

function publish_app() {
  dotnet publish $WEB_HOST_PROJECT --output "${PUBLISH_DIRECTORY}/web_host" --configuration "${CONFIGURATION}" --runtime "${PUBLISH_RUNTIME}"
  dotnet publish $ZIGBEE_HOST_PROJECT --output "${PUBLISH_DIRECTORY}/zigbee_host" --configuration "${CONFIGURATION}" --runtime "${PUBLISH_RUNTIME}"
}

function main() {
  build_solution
  run_tests
  publish_app
}

main