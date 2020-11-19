import SocketIO from 'socket.io';
import {logger} from '../../logger';
import {MqttClient} from 'mqtt';

export function linkMqttAndSocketIo(client: MqttClient, io: SocketIO.Server) {
    client.subscribe('#');
    client.on('message', (topic, msg) => {
        logger.info('Received mqtt message', {topic});
        io.emit('message', {
            topic,
            payload: msg.toString('utf8')
        });
    });
}
