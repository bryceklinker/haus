import express from 'express';
import cors from 'cors';
import {expressErrorLogger, expressLogger} from '../common/logger';
import {createDiscoveryRouter} from '../discovery/discovery-router';

export function configureRestApi(appSettings) {
    const app = express();
    app.use(cors());
    app.use(expressLogger);
    app.use('/discovery', createDiscoveryRouter());
    app.use(expressErrorLogger);
    return app;
}
