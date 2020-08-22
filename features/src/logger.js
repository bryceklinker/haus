import winston from 'winston';

const LOGGER_CONFIG = {
    level: 'info',
    transports: [
        new winston.transports.Console()
    ],
    format: winston.format.combine(
        winston.format.colorize(),
        winston.format.simple()
    )
}

export const logger = winston.createLogger(LOGGER_CONFIG)