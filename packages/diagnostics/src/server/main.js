import {createServer} from 'http';
import createSocketIo from 'socket.io';
import {createExpressApp} from './create-express-app';
import {createMqttObserver} from './create-mqtt-observer';


async function main(port) {
    const app = createExpressApp();
    const server = createServer(app);
    const io = createSocketIo(server);
    await createMqttObserver(io);
    return new Promise((resolve) => {
        server.listen(port, () => {
            console.log(`Now listening at http://localhost:${port}...`);
            resolve();
        });
    })
}

main(process.env.PORT || 3000)
    .catch(err => {
        console.error(err)
        process.exit(1);
    });

