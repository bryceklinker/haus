import {startDiscoveryListener} from '../discovery/discovery-listener';

export async function configureMqttApp(appSettings) {
    await startDiscoveryListener(appSettings);
}
