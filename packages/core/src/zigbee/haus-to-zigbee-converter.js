import {ZIGBEE_TOPICS} from './zigbee-messages';
import {DISCOVERY_MESSAGES} from '../discovery/discovery-messages';

export function convertToZigbee({type, payload}) {
    switch (type) {
        case DISCOVERY_MESSAGES.TYPES.START:
            return {topic: ZIGBEE_TOPICS.PERMIT_JOIN, message: 'true'};
        case DISCOVERY_MESSAGES.TYPES.STOP:
            return {topic: ZIGBEE_TOPICS.PERMIT_JOIN, message: 'false'};
        default:
            return {topic: 'haus/unknown', message: payload};
    }
}
