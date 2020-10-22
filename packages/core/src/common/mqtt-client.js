import {connect} from 'mqtt';

let mqttClient = null;
let url = null;

function configure({mqttUrl}) {
    url = mqttUrl;
    if (mqttClient) {
        mqttClient.end();
    }
}
function getClient() {
    if (mqttClient) {
        return Promise.resolve(mqttClient);
    }

    return new Promise((resolve, reject) => {
        mqttClient = connect(url);
        mqttClient.on('connect', () => {
            resolve(mqttClient);
        });
    });
}

export const MQTT_CLIENT = {getClient, configure};
export default MQTT_CLIENT;
