import {createDevicesServer} from './server';

describe('Server', () => {
    let zigbeeFake, server;

    beforeEach(async () => {
        zigbeeFake = {
            start: jest.fn().mockResolvedValue(null),
            stop: jest.fn().mockResolvedValue(null)
        };

        server = createDevicesServer(zigbeeFake);
    });

    test('when started then zigbee listener is started', async () => {
        await server.start();

        expect(zigbeeFake.start).toHaveBeenCalled();
    });

    test('when stopped then zigbee listener is stopped', async () => {
        await server.stop();
        expect(zigbeeFake.stop).toHaveBeenCalled();
    })
});
