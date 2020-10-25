import {convertToZigbee} from './haus-to-zigbee-converter';
import {HAUS_TOPICS} from '../common/haus-topics';

export async function startZigbeeListener(settings, client) {
    client.subscribe(HAUS_TOPICS.COMMANDS);
    client.on('message', async (incomingTopic, incomingMessage) => {
        const {topic, message} = convertToZigbee(JSON.parse(incomingMessage.toString()));
        await client.publish(topic, JSON.stringify(message));
    });
}
