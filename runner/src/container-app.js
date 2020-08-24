import {spawn, spawnSync} from "child_process";
import {logger} from "./logger";

export class ContainerApp {
    constructor({name, containerName, startArgs, imageName, wait}) {
        this.name = name;
        this.startArgs = startArgs;
        this.imageName = imageName;
        this.containerName = containerName;
        this.wait = wait;
    }
    
    start = () => {
        this.stop();
        const args = [
            'run',
            ...this.startArgs,
            '--name', this.containerName,
            this.imageName
        ];
        this.container = spawn('docker', args);
        this.container.stdout.on('data', data => logger.info(`[${this.name}]: ${data}`));
        this.container.stderr.on('data', data => logger.info(`[${this.name}]: ${data}`));
        this.container.on('exit', data => logger.info(`[${this.name}]: Exiting ${this.containerName}: ${data}`));
        this.container.on('close', data => logger.info(`[${this.name}]: Closing ${this.containerName}: ${data}`));
    }
    
    remove = () => {
        logger.info(`Removing existing ${this.containerName} container...`);
        spawnSync('docker', ['container', 'rm', this.containerName]);
        logger.info(`Removed existing ${this.containerName} container.`);
    }

    stop = () => {
        logger.info(`Stopping ${this.containerName} container...`);
        spawnSync('docker', ['container', 'stop', this.containerName]);
        logger.info(`Stopped ${this.containerName} container.`);
        this.remove();
    }
}