import Controller from 'zigbee2mqtt/lib/controller';

describe('Server', () => {
    let controller;

    beforeAll(() => {
        controller = new Controller();
    })

    test('when started then listens for devices', async () => {
        await controller.start();
    });
});
