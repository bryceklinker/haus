import {createServer} from 'http';
import socketIo from 'socket.io'
import {createApp} from './app/create-app';
import {createMqttListener} from './app/create-mqtt-listener';
import {logger} from './logger';

async function start(port = 3000) {
    const app = createApp();
    const server = createServer(app);
    const io = socketIo(server);
    await createMqttListener(io);
    server.listen(port, () => {
        logger.info(`Now listening at http://localhost:${port}...`);
    });
}

export const Server = {start};
