import MQTT_CLIENT from './mqtt-client';

const COMMAND_TOPIC = 'haus/commands';

async function publish(command) {
    const client = await MQTT_CLIENT.getClient();
    await client.publish(COMMAND_TOPIC, JSON.stringify(command));
}

export const COMMAND_BUS = {publish};

export default COMMAND_BUS;
