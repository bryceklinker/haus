import {Router} from 'express';
import COMMAND_BUS from '../common/command-bus';
import {DISCOVERY_MESSAGES} from './discovery-messages';

export function createDiscoveryRouter() {
    const router = Router();
    router.post('/start', async (req, res) => {
        await COMMAND_BUS.publish(DISCOVERY_MESSAGES.start());
        res.status(200).end();
    });

    router.post('/stop', async (req, res) => {
        await COMMAND_BUS.publish(DISCOVERY_MESSAGES.stop());
        res.status(200).end();
    })

    return router;
}
