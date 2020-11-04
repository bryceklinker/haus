import express from 'express';
import {logger, errorLogger} from 'express-winston';
import webpack from 'webpack';
import {devConfig} from '../webpack.config';
import webpackMiddleware from 'webpack-dev-middleware';
import {createLoggerConfig} from '@haus/core';

function createWebpackMiddleware() {
    const compiler = webpack(devConfig);
    return webpackMiddleware(compiler);
}

export function createApp() {
    const app = express();
    app.use(logger(createLoggerConfig()));
    app.use(createWebpackMiddleware());
    app.use(errorLogger(createLoggerConfig()));
    return app;
}
