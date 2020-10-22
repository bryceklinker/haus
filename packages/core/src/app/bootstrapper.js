import settings from '../common/settings';
import {configureLogger, getLogger} from '../common/logger';
import MQTT_CLIENT from '../common/mqtt-client';
import {configureRestApi} from './configure-rest-api';
import {configureMqttApp} from './configure-mqtt';

async function startRestApi(port, appSettings) {
    const app = configureRestApi(appSettings);
    return new Promise((resolve, reject) => {
        const server = app.listen(port, () => {
            const baseUrl = `http://localhost:${server.address().port}`;
            getLogger().info(`Now listening at ${baseUrl}...`);
            resolve({
                baseUrl,
                close: () => server.close()
            });
        });
    });
}

async function startMqttApp(appSettings) {
    const mqttClient = await MQTT_CLIENT.getClient();
    await configureMqttApp(appSettings);
    return {
        mqttClient,
        close: () => mqttClient.end()
    };
}

export function bootstrapApp(appSettings) {
    configureLogger(settings);
    const app = {
        start: async (port) => {
            app.restApi = await startRestApi(port, appSettings);
            app.mqttApp = await startMqttApp(appSettings);
            app.baseUrl = app.restApi.baseUrl;
            app.mqttClient = app.mqttApp.mqttClient;
        },
        stop: () => {
            if (app.restApi) {
                app.restApi.close();
            }

            if (app.mqttApp) {
                app.mqttApp.close();
            }
        }
    };

    return app;
}
