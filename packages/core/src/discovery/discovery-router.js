import {Router} from 'express';
import COMMAND_BUS from '../common/command-bus';

export function createDiscoveryRouter() {
    const router = Router();
    router.post('/start', async (req, res) => {
        await COMMAND_BUS.publish({type: 'discovery/start'});
        res.status(200).end();
    });

    router.post('/stop', async (req, res) => {
        await COMMAND_BUS.publish({type: 'discovery/stop'});
        res.status(200).end();
    })

    return router;
}
