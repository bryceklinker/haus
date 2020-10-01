import expressWinston from 'express-winston';
import {LOGGER_CONFIG, logger} from '@haus/logging';

const EXPRESS_LOGGER_CONFIG = {
    ...LOGGER_CONFIG,
    expressFormat: true,
    meta: true,
    colorize: true
}

export {logger} from '@haus/logging';
export const expressLogger = expressWinston.logger(EXPRESS_LOGGER_CONFIG);
export const expressErrorLogger = expressWinston.errorLogger(EXPRESS_LOGGER_CONFIG);
