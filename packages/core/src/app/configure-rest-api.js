import express from 'express';
import cors from 'cors';
import {expressErrorLogger, expressLogger, getExpressErrorLogger, getExpressLogger} from '../common/logger';
import {createDiscoveryRouter} from '../discovery/discovery-router';

export function configureRestApi(appSettings) {
    const app = express();
    app.use(cors());
    app.use(getExpressLogger());
    app.use('/discovery', createDiscoveryRouter());
    app.use(getExpressErrorLogger());
    return app;
}
