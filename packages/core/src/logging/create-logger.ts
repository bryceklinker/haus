import {transports, format, createLogger as winstonCreateLogger} from 'winston';

type LogLevel = 'info' | 'debug' | 'warn' | 'error';

const DEFAULT_LOGGER_CONFIG = {
    level: 'info',
    format: format.combine(
        format.colorize(),
        format.simple()
    ),
    transports: [
        new transports.Console()
    ],
}

export interface LoggerConfigOptions {
    level?: LogLevel;
    metadata?: any;
}

export function createLoggerConfig({level = 'info', metadata = {}}: LoggerConfigOptions = {}) {
    return {
        ...DEFAULT_LOGGER_CONFIG,
        level,
        defaultMeta: metadata
    }
}

export function createLogger({level = 'info', metadata = {}}: LoggerConfigOptions = {}) {
    return winstonCreateLogger(createLoggerConfig({level, metadata}));
}
