import {MqttListener} from '../../testing/mqtt-listener';
import {HausEventBus} from './haus-event-bus';
import {ZIGBEE_LOG_TYPES, ZIGBEE_TOPICS} from '../zigbee/topics';
import {eventually} from '../../testing/eventually';
import {zigbeeMessageFactory} from '../../testing/zigbee/messages';

describe('haus-event-bus', () => {
    let mqttListener, eventBus;

    beforeEach(async () => {
        mqttListener = new MqttListener();
        await mqttListener.subscribe();

        eventBus = new HausEventBus();
        await eventBus.start();
    });

    test('when discovery event received then permits devices to join the network', async () => {
        await eventBus.startDiscovery();

        await eventually(() => {
            expect(mqttListener.receivedMessages).toContainEqual({
                topic: ZIGBEE_TOPICS.PERMIT_JOIN,
                message: 'true'
            });
        });
    });

    test('when discovery stopped then forbids devices from joining', async () => {
        await eventBus.stopDiscovery();

        await eventually(() => {
            expect(mqttListener.receivedMessages).toContainEqual({
                topic: ZIGBEE_TOPICS.PERMIT_JOIN,
                message: 'false'
            });
        });
    });

    test('when device discovered then publishes device found message', async () => {
        const {topic, payload} = zigbeeMessageFactory.log.deviceConnected();
        await mqttListener.publish(topic, JSON.stringify(payload));

        await eventually(() => {
            expect(mqttListener.receivedMessages).toContainEqual({
                topic: 'haus/events',
                message: JSON.stringify({
                    type: 'device_discovered',
                    payload: {
                        id: payload.message.friendly_name
                    }
                })
            });
        });
    });

    afterEach(async () => {
        await mqttListener.stop();
        await eventBus.stop();
    });
});
