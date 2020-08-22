import {spawn, spawnSync} from 'child_process'
import {logger} from './logger';

const CONTAINER_NAME = 'haus-database';
const IMAGE_NAME = 'postgres:12.3-alpine';
const DB_PASSWORD = '4bP0u@miBKxrvTHiWZgR';
const DB_USER = 'haus';

export class Database {    
    start = () => {
        this.stop();
        const args = [
            'run',
            '-e', `POSTGRES_PASSWORD=${DB_PASSWORD}`,
            '-e', `POSTGRES_USER=${DB_USER}`,
            '-p', '5432:5432',
            '--name', CONTAINER_NAME,
            IMAGE_NAME
        ];

        this.db = spawn('docker', args);
        this.db.stdout.on('data', data => logger.info(`[DB]: ${data}`));
        this.db.stderr.on('data', data => logger.info(`[DB]: ${data}`));
        this.db.on('exit', data => logger.info(`[DB]: Exiting database: ${data}`));
        this.db.on('close', data => logger.info(`[DB]: Closing database: ${data}`));
    }
    
    remove = () => {
        logger.info('Removing existing database container...');
        spawnSync('docker', ['container', 'rm', CONTAINER_NAME]);
        logger.info('Removed existing database container.');
    }
    
    stop = () => {
        logger.info('Stopping database container...');
        spawnSync('docker', ['container', 'stop', CONTAINER_NAME]);
        logger.info('Stopped database container.');
        this.remove();
    }
}