import settings from './settings';
import winston, {createLogger, log} from 'winston';
import {logger as createExpressLogger, errorLogger as createExpressErrorLogger} from 'express-winston';

let LOGGER_CONFIG = {
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

let EXPRESS_LOGGER_CONFIG = {
    ...LOGGER_CONFIG,
    expressFormat: true
}

export function getLogger() {
    return createLogger(LOGGER_CONFIG);
}

export function getExpressLogger() {
    return createExpressLogger(EXPRESS_LOGGER_CONFIG);
}

export function getExpressErrorLogger() {
    return createExpressErrorLogger(EXPRESS_LOGGER_CONFIG);
}

export function configureLogger({logLevel}) {
    LOGGER_CONFIG = {
        ...LOGGER_CONFIG,
        level: logLevel
    };

    EXPRESS_LOGGER_CONFIG = {
        ...EXPRESS_LOGGER_CONFIG,
        level: logLevel
    }
}
