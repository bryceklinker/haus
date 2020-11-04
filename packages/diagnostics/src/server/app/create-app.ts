import express from 'express';
import bodyParser from 'body-parser';
import {logger, errorLogger} from 'express-winston';
import webpack from 'webpack';
import {devConfig} from '../webpack.config';
import webpackMiddleware from 'webpack-dev-middleware';
import {createLoggerConfig} from '@haus/core';
import {createMqttRouter} from './mqtt/create-mqtt-router';
import {MqttClient} from 'mqtt';

function createWebpackMiddleware() {
    const compiler = webpack(devConfig);
    return webpackMiddleware(compiler);
}

export function createApp(client: MqttClient) {
    const app = express();
    app.use(logger(createLoggerConfig()));
    app.use(bodyParser.json());
    app.use(createWebpackMiddleware());
    app.use('/mqtt', createMqttRouter(client));
    app.use(errorLogger(createLoggerConfig()));
    return app;
}
