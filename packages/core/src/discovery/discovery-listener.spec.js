import {startTestServer} from '../../testing/start-test-server';
import {DeviceModel} from '../devices/device-model';
import {eventually} from '../../testing/eventually';

describe('discovery-listener', () => {
    let testServer;

    beforeAll(async () => {
        testServer = await startTestServer();
    });

    test('when device is discovered then device is saved', async () => {
        const message = {
            type: 'new_device',
            payload: {
                friendly_name: 'idk'
            }
        };
        await testServer.mqttClient.publish('haus/events', JSON.stringify(message));

        await eventually(async () => {
            expect(await testServer.createDb().findAll(DeviceModel)).toHaveLength(1);
        });
    });

    afterAll(() => {
        testServer.stop();
    });
});
