import waitOn from "wait-on";
import {logger} from "./logger";

export const DATABASE_OPTIONS = {
    name: 'DB',
    containerName: 'haus-database',
    imageName: 'postgres:12.3-alpine',
    startArgs: [
        '-e', `POSTGRES_USER=haus`,
        '-e', `POSTGRES_PASSWORD=4bP0u@miBKxrvTHiWZgR`,
        '-p', '5432:5432',
    ],
    wait: async () => {
        logger.info('Waiting for database to be ready...');
        await waitOn({
            resources: [
                'tcp:localhost:5432'
            ]
        });
        logger.info('Database is ready.');
    }
}

export const SERVICE_BUS_OPTIONS = {
    name: 'ServiceBus',
    containerName: 'haus-service-bus',
    imageName: 'rabbitmq:3.8.7-management-alpine',
    startArgs: [
        '-p', '15672:15672',
        '-p', '5672:5672',
        '-e', `RABBITMQ_DEFAULT_USER=haus`,
        '-e', `RABBITMQ_DEFAULT_PASS=4bP0u@miBKxrvTHiWZgR`,
        '--hostname', 'haus-service-bus',
    ],
    wait: async () => {
        logger.info('Waiting for service bus to be ready...');
        await waitOn({
            resources: [
                'http://localhost:15672'
            ]
        });
        logger.info('Service bus is ready.');
    }
}

export const ALL_CONTAINER_APPS = [DATABASE_OPTIONS, SERVICE_BUS_OPTIONS];