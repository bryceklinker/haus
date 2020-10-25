import {ZIGBEE_TOPICS} from './zigbee-messages';
import {startTestServer} from '../../testing/start-test-server';
import eventually from '../../testing/eventually';
import {HAUS_TOPICS} from '../common/haus-topics';
import {DISCOVERY_MESSAGES} from '../discovery/discovery-messages';

describe('haus-to-zigbee-relay', () => {
    let server, client, messages;

    beforeAll(async () => {
        messages = [];
        server = await startTestServer();
        client = server.mqttClient;
        client.subscribe('#');
        client.on('message', (topic, message) => {
            console.log(message.toString());
            messages.push({topic, message: JSON.parse(message.toString())});
        });
    });

    test('when haus command received then zigbee message is published', async () => {
        client.publish(HAUS_TOPICS.COMMANDS, JSON.stringify(DISCOVERY_MESSAGES.start()));

        await eventually(() => {
            expect(messages).toContainEqual({
                topic: ZIGBEE_TOPICS.PERMIT_JOIN,
                message: 'true'
            });
        });
    });

    afterAll(() => {
        server.stop();
    });
});
