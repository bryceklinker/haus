import {Router} from 'express';
import {MqttClient} from 'mqtt';
import {logger} from '../../logger';

export function createMqttRouter(client: MqttClient) {
    const router = Router();
    router.post('/publish', async (req, res) => {
        const {topic, payload} = req.body;
        logger.info(`Publishing message to topic ${topic}`, {payload});
        await client.publish(topic, JSON.stringify(payload));
        res.status(200).end();
    });
    return router;
}
