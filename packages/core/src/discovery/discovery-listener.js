import EVENT_BUS from '../common/event-bus';
import {DeviceModel} from '../devices/device-model';

export async function startDiscoveryListener(appSettings) {
    await EVENT_BUS.subscribe('new_device', async ({payload}) => await DeviceModel.createFromNewDevice(payload))
}
