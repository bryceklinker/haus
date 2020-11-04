import SocketIO from 'socket.io';
import {connect} from 'mqtt';
import {logger} from '../logger';

export function createMqttListener(io: SocketIO.Server): Promise<void> {
    return new Promise<void>((resolve, reject) => {
        const client = connect('mqtt://localhost');
        client.on('connect', () => {
            logger.info('Connected to MQTT');
            client.subscribe('#');
            client.on('message', (topic, msg) => {
                logger.info('Received mqtt message', {topic});
                io.emit('message', {
                    topic,
                    payload: msg.toString('utf8')
                });
            });
            resolve();
        });
    });
}
