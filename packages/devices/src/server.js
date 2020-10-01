import {ZigbeeListener} from './zigbee/zigbee-listener';
import {HausEventBus} from './haus/haus-event-bus';

export function createDevicesServer(zigbeeListener = new ZigbeeListener(), hausEventBus = new HausEventBus()) {
    async function start() {
        await zigbeeListener.start()
        await hausEventBus.start();
    }

    async function stop() {
        await zigbeeListener.stop();
        await hausEventBus.stop();
    }

    return {start, stop};
}
