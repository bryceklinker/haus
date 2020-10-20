import EVENT_BUS from '../common/event-bus';
import {DeviceModel} from '../devices/device-model';

export async function startDiscoveryListener(appSettings) {
    await EVENT_BUS.subscribe('new_device', ({payload}) => {
        DeviceModel.create({
            external_id: payload.friendly_name,
            device_config: payload
        })
    })
}
