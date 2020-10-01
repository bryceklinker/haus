import {ZIGBEE_LOG_TYPES, ZIGBEE_TOPICS} from '../../zigbee/topics';
import {HAUS_EVENT_TYPES, HAUS_TOPICS} from '../haus-topics';
import {ZigbeeToHausTranslator} from './zigbee-to-haus-translator';
import {zigbeeMessageFactory} from '../../../testing/zigbee/messages';

describe('ZigbeeToHausTranslator', () => {
    test('when device connected then event is translated to device discovered', () => {
        const {topic, payload} = zigbeeMessageFactory.log.deviceConnected({friendly_name: 'bill'});

        const hausEvent = ZigbeeToHausTranslator.translate(topic, JSON.stringify(payload));

        expect(hausEvent.topic).toEqual(HAUS_TOPICS.EVENTS);
        expect(hausEvent.message).toEqual({
            type: HAUS_EVENT_TYPES.DEVICE_DISCOVERED,
            payload: {
                id: 'bill'
            }
        });
    });
});
