import {startTestServer} from '../../testing/start-test-server';
import {eventually} from '../../testing/eventually';

describe('discovery-router', () => {
    let testServer, publishedCommands;

    beforeAll(async () => {
        publishedCommands = [];

        testServer = await startTestServer();
        listenForCommands(testServer.mqttClient);
    });

    test('when starting discovery then start discovery command published', async () => {
        const response = await testServer.client.post('/discovery/start');

        await eventually(() => {
            expect(response.status).toEqual(200);
            expect(publishedCommands).toContainEqual({
                type: 'discovery/start'
            });
        });
    });

    test('when stopping discovery then stop discovery command published', async () => {
        const response = await testServer.client.post('/discovery/stop');

        await eventually(() => {
            expect(response.status).toEqual(200);
            expect(publishedCommands).toContainEqual({
                type: 'discovery/stop'
            });
        });
    })

    afterAll(() => {
        testServer.stop();
    });

    function listenForCommands(mqttClient) {
        mqttClient.subscribe('haus/commands');
        mqttClient.on('message', (topic, message) => {
            publishedCommands.push(JSON.parse(message.toString()));
        });
    }
});
