import {connect, MqttClient} from 'mqtt';
import {logger} from '../../logger';

export function createMqttClient(): Promise<MqttClient> {
    return new Promise<MqttClient>((resolve) => {
        const client = connect('mqtt://localhost');
        client.on('connect', () => {
            logger.info('Connected to MQTT');
            resolve(client);
        });
    });
}
