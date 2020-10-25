import {startTestServer} from '../../testing/start-test-server';
import {DeviceModel} from '../devices/device-model';
import eventually from '../../testing/eventually';

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
            const devices = await testServer.createDb().findAll(DeviceModel);
            expect(devices).toContainEqual(expect.objectContaining({
                external_id: 'idk',
                device_config: {friendly_name: 'idk'}
            }));
        });
    });

    afterAll(() => {
        testServer.stop();
    });
});