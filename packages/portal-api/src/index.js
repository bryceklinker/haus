import {Server} from './server';
import {logger} from './logging/logger';

const PORT = process.env.PORT || 3001;
Server.start(PORT)
    .catch(error => {
        logger.error(`Failed to start server on port ${PORT}`, {error: error.message, stack: error.stack});
    });
