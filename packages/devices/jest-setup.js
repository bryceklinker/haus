const path = require('path');

function setupZigbee2MqttEnvironmentVariables() {
    require('dotenv').config({
        path: path.resolve(__dirname, '.env')
    });
    // process.env.ZIGBEE2MQTT_DATA = path.resolve(__dirname, 'zigbee_data');
    // process.env.ZIGBEE2MQTT_CONFIG = path.resolve(__dirname, 'configuration.yaml');
}

setupZigbee2MqttEnvironmentVariables();
