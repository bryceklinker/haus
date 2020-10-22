import {startDiscoveryListener} from '../discovery/discovery-listener';
import MQTT_CLIENT from '../common/mqtt-client';

export async function configureMqttApp(appSettings) {
    MQTT_CLIENT.configure(appSettings);
    const client = await MQTT_CLIENT.getClient();
    await startDiscoveryListener(appSettings);
    return {mqttClient: client};
}
