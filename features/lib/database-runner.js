const {spawn, spawnSync} = require('child_process');
const treeKill = require('tree-kill');

const CONTAINER_NAME = 'haus-database';
const IMAGE_NAME = 'postgres:12.3-alpine';
const DB_PASSWORD = '4bP0u@miBKxrvTHiWZgR';
const DB_USER = 'haus';

function startDatabase() {
    stopDatabase()
    const args = [
        'run',
        '-e', `POSTGRES_PASSWORD=${DB_PASSWORD}`,
        '-e', `POSTGRES_USER=${DB_USER}`,
        '-p', '5432:5432',
        '--name', CONTAINER_NAME,
        IMAGE_NAME
    ];
    
    console.log('Starting database container...')
    const dbProcess = spawn('docker', args);
    dbProcess.stdout.on('data', data => console.log(`[DB]: ${data}`));
    dbProcess.stderr.on('data', data => console.error(`[DB]: ${data}`));
    dbProcess.on('exit', data => console.log(`[DB]: Exiting database: ${data}`));
    dbProcess.on('close', data => console.log(`[DB]: Closing database: ${data}`));
}

function removeDatabaseContainer() {
    console.log('Removing existing database container...');
    spawnSync('docker', ['container', 'rm', CONTAINER_NAME]);
    console.log('Removed existing database container.');
}

function stopDatabaseContainer() {
    console.log('Stopping database container...');
    const result = spawnSync('docker', ['container', 'stop', CONTAINER_NAME]);
    console.log('Stopped database container.');
}

function stopDatabase() {
    stopDatabaseContainer();
    removeDatabaseContainer();
}

module.exports = {startDatabase, stopDatabase};