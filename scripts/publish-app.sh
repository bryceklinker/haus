#!/usr/bin/env bash
set -ex

source ./scripts/variables.sh

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

  mkdir -p "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/data"
  cp "${WORKING_DIRECTORY}/zigbee2mqtt/configuration.yaml" "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/configuration.yaml"
  cp "${WORKING_DIRECTORY}/zigbee2mqtt/haus-zigbee2mqtt.service" "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}/haus-zigbee2mqtt.service"
  
  pushd "${ZIGBEE_2_MQTT_PACKAGE_DIRECTORY}" || exit 
    zip -rm "../haus-linux-zigbee2mqtt.zip" -- *
  popd
}

function main() {
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

main