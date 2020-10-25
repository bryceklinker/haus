import express from 'express';
import cors from 'cors';
import {getExpressErrorLogger, getExpressLogger} from '../common/logger';
import {REST_API_ROUTERS} from './rest-api-routers';

export function configureRestApi(appSettings) {
    const app = express();
    app.use(cors());
    app.use(getExpressLogger());
    REST_API_ROUTERS.forEach(config => {
        app.use(config.path, config.factory(appSettings));
    })
    app.use(getExpressErrorLogger());
    return app;
}
