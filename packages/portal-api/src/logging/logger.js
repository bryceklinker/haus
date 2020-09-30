import winston from 'winston';
import expressWinston from 'express-winston';

const LOGGER_CONFIG = {
    level: 'info',
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
    expressFormat: true,
    meta: true,
    colorize: true
}

export const logger = winston.createLogger(LOGGER_CONFIG);
export const expressLogger = expressWinston.logger(EXPRESS_LOGGER_CONFIG);
export const expressErrorLogger = expressWinston.errorLogger(EXPRESS_LOGGER_CONFIG);
