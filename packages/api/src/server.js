import express from 'express';
import cors from 'cors';
import {logger, expressErrorLogger, expressLogger} from './logging/logger';
import {healthRouter} from './health/health-router';

let httpServer;

function start(port) {
    return new Promise((resolve) => {
        const app = express();
        app.use(expressLogger);
        app.use(cors());
        app.use('/.health', healthRouter);
        app.use(expressErrorLogger);
        httpServer = app.listen(port, () => {
            logger.info(`Now listening on http://localhost:${port}...`);
            resolve();
        });
    });
}

function stop() {
    if (httpServer) {
        httpServer.close();
    }
}

export const Server = {start, stop};
