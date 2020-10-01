import winston from 'winston';

export const LOGGER_CONFIG = {
    level: process.env.LOG_LEVEL || 'info',
    transports: [
        new winston.transports.Console()
    ],
    format: winston.format.combine(
        winston.format.colorize(),
        winston.format.simple()
    )
}

export const logger = winston.createLogger(LOGGER_CONFIG);
