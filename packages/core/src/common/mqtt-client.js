import {connect} from 'mqtt';

let mqttClient = null;

function getClient() {
    if (mqttClient) {
        return Promise.resolve(mqttClient);
    }

    return new Promise((resolve, reject) => {
        mqttClient = connect('mqtt://localhost');
        mqttClient.on('connect', () => {
            resolve(mqttClient);
        });
    });
}

export const MQTT_CLIENT = {getClient};
export default MQTT_CLIENT;
