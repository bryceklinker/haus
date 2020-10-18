import express from 'express';
import {createDiscoveryRouter} from './discovery/discovery-router';
import MQTT_CLIENT from './common/mqtt-client';

function createExpressApp() {
    const app = express();
    app.use('/discovery', createDiscoveryRouter());
    return app;
}
const app = {
    mqttClient: null,
    server: null,
    baseUrl: null,
    listen: (port) => {
        return new Promise(async (resolve, reject) => {
            app.mqttClient = await MQTT_CLIENT.getClient();
            app.server = createExpressApp().listen(port, () => {
                app.baseUrl = `http://localhost:${app.server.address().port}`;
                resolve(app);
            });

        });
    },
    close: () => {
        app.server.close();
        app.mqttClient.end();
    }
}
export default app;

