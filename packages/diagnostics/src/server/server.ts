import {createServer} from 'http';
import socketIo from 'socket.io'
import {createApp} from './app/create-app';
import {linkMqttAndSocketIo} from './app/mqtt/link-mqtt-and-socket-io';
import {logger} from './logger';
import {createMqttClient} from './app/mqtt/create-mqtt-client';

async function start(port = 3000) {
    const mqttClient = await createMqttClient();
    const app = createApp(mqttClient);
    const server = createServer(app);
    const io = socketIo(server);
    linkMqttAndSocketIo(mqttClient, io);
    server.listen(port, () => {
        logger.info(`Now listening at http://localhost:${port}...`);
    });
}

export const Server = {start};
