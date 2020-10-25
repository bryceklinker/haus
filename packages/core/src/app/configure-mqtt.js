import MQTT_CLIENT from '../common/mqtt-client';
import {MQTT_LISTENERS} from './mqtt-listeners';

export async function configureMqttApp(appSettings) {
    MQTT_CLIENT.configure(appSettings);
    const client = await MQTT_CLIENT.getClient();
    await MQTT_LISTENERS.map(async factory => {
        await factory(appSettings, client);
    });
    return {mqttClient: client};
}
