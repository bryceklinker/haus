import {startZigbeeListener} from '../zigbee/zigbee-listener';
import {startDiscoveryListener} from '../discovery/discovery-listener';

export const MQTT_LISTENERS = [
    startZigbeeListener,
    startDiscoveryListener
]
