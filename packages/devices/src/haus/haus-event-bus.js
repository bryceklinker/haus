import {ZIGBEE_TOPICS} from '../zigbee/topics';
import {ZigbeeToHausTranslator} from './translators/zigbee-to-haus-translator';
import {MQTT} from '../common/mqtt';
import {logger} from '@haus/logging';

export class HausEventBus {
    start = async () => {
        this.client = await MQTT.subscribe(this.handleZigbeeMessage);
    }

    stop = async () => {
        if (this.client) {
            this.client.end();
        }
    }

    startDiscovery = async () => {
        await MQTT.publish(ZIGBEE_TOPICS.PERMIT_JOIN, 'true');
    }

    stopDiscovery = async () => {
        await MQTT.publish(ZIGBEE_TOPICS.PERMIT_JOIN, 'false');
    }

    handleZigbeeMessage = async (topic, message) => {
        logger.debug('HAUS EVENT BUS', {topic, message: message.toString()})
        const hausEvent = ZigbeeToHausTranslator.translate(topic, message);
        if (hausEvent) {
            await this.client.publish(hausEvent.topic, JSON.stringify(hausEvent.message));
        }
    }
}


