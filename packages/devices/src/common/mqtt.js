import {config} from './config';
import {connect} from 'mqtt';

function subscribe(handler, topic = '#', mqtt_url = config.mqtt_url) {
    const client = connect(mqtt_url);
    client.on('connect', () => {
        client.subscribe(topic);
    });

    client.on('message', async (topic, message) => await handler(topic, message));
    return client;
}

async function publish(topic, message, mqtt_url = config.mqtt_url) {
    const client = connect(mqtt_url);
    client.on('connect', async () => {
        await client.publish(topic, message);
        await client.end();
    });
}

export const MQTT = {subscribe, publish};
