import {ZIGBEE_LOG_TYPES, ZIGBEE_TOPICS} from '../../zigbee/topics';
import {HAUS_EVENT_TYPES, HAUS_TOPICS} from '../haus-topics';

const ZIGBEE_TO_HAUS_EVENT_TYPE = {
    [ZIGBEE_LOG_TYPES.DEVICE_CONNECTED]: HAUS_EVENT_TYPES.DEVICE_DISCOVERED
}

function translateLogMessage(message) {
    const {type, message: {friendly_name, ...rest}} = JSON.parse(message);
    return {
        topic: HAUS_TOPICS.EVENTS,
        message: {
            type: ZIGBEE_TO_HAUS_EVENT_TYPE[type],
            payload: {
                ...rest,
                id: friendly_name,
            }
        }
    }
}

function translate(topic, message) {
    switch (topic) {
        case ZIGBEE_TOPICS.LOG:
            return translateLogMessage(message);
        default:
            return null;
    }
}

export const ZigbeeToHausTranslator = {translate};
