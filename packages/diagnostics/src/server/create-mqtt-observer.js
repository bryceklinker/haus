import {connect} from 'mqtt';

export async function createMqttObserver(io) {
    return new Promise((resolve) => {
        const client = connect('mqtt://localhost');
        client.on('connect', () => {
            client.subscribe('#');
            client.on('message', (topic, message) => {
                io.emit('message', {
                    topic,
                    message: message.toString()
                });
            });
            resolve(client);
        });
    })

}
