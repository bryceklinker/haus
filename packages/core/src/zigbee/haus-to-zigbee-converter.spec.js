import {convertToZigbee} from './haus-to-zigbee-converter';
import {DISCOVERY_MESSAGES} from '../discovery/discovery-messages';

describe('convertToZigbee', () => {
    test('when discovery start message then returns permit join true message', () => {
        const zigbee = convertToZigbee(DISCOVERY_MESSAGES.start());
        expect(zigbee).toEqual({
            topic: 'zigbee2mqtt/bridge/config/permit_join',
            message: 'true'
        });
    });

    test('when discovery stopped message then returns permit join false message', () => {
        const zigbee = convertToZigbee(DISCOVERY_MESSAGES.stop());
        expect(zigbee).toEqual({
            topic: 'zigbee2mqtt/bridge/config/permit_join',
            message: 'false'
        });
    });

    test('when unknown message is converted then returns unknown message', () => {
        const zigbee = convertToZigbee({type: 'nope', payload: 'something'});

        expect(zigbee).toEqual({
            topic: 'haus/unknown',
            message: 'something'
        });
    });
});
