import settings from './settings';
import winston, {createLogger} from 'winston';
import {logger as createExpressLogger, errorLogger as createExpressErrorLogger} from 'express-winston';

const LOGGER_CONFIG = {
    level: settings.logLevel,
    meta: true,
    transports: [
        new winston.transports.Console()
    ],
    format: winston.format.combine(
        winston.format.colorize(),
        winston.format.simple()
    )
};

const EXPRESS_LOGGER_CONFIG = {
    ...LOGGER_CONFIG,
    expressFormat: true
}

export const logger = createLogger(LOGGER_CONFIG);
export const expressLogger = createExpressLogger(EXPRESS_LOGGER_CONFIG);
export const expressErrorLogger = createExpressErrorLogger(EXPRESS_LOGGER_CONFIG);
export default logger;
