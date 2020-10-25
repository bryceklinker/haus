import {HAUS_TOPICS} from './haus-topics';
import MQTT_CLIENT from './mqtt-client';

async function publish(command) {
    const client = await MQTT_CLIENT.getClient();
    await client.publish(HAUS_TOPICS.COMMANDS, JSON.stringify(command));
}

export const COMMAND_BUS = {publish};

export default COMMAND_BUS;
