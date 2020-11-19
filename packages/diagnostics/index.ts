import {Server} from './src/server/server';
import {createLogger} from '@haus/core';

const port = Number.isInteger(process.env.PORT) ? Number(process.env.PORT) : 3001;
Server.start(port)
    .catch(err => createLogger().error('Failed to run application', {err}));
