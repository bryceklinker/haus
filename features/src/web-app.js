import waitOn from "wait-on";
import treeKill from "tree-kill";
import path from 'path';

import {logger} from "./logger";
import dotnet from './dotnet-cli';
import rimraf from "rimraf";

const DIST_PATH = path.resolve(path.join(__dirname, 'dist'));

export class WebApp {
    get outputPath() {
        return path.resolve(path.join(DIST_PATH, this.name));
    }

    get baseUrl() {
        return `https://localhost:${this.httpsPort}`;
    }

    get waitUrl() {
        const httpsGetUrl = this.baseUrl.replace('https', 'https-get');
        return `${httpsGetUrl}/.health`;
    }

    constructor({name, projectPath, httpPort, httpsPort}) {
        this.name = name;
        this.projectPath = projectPath;
        this.httpPort = httpPort;
        this.httpsPort = httpsPort;
    }

    publish = () => {
        logger.info(`Publishing ${this.name} to ${this.outputPath}...`)
        const {success, output} = dotnet.publish({projectPath: this.projectPath, outputPath: this.outputPath});
        if (success) {
            logger.info(`Successfully published ${this.name} to ${this.outputPath}`);
        } else {
            logger.error(`Failed to publish ${this.name} to ${this.outputPath}\r\n${output}`)
        }
    };

    start = () => {
        logger.info(`Starting ${this.name} from ${this.outputPath}...`);
        this.app = dotnet.run({
            publishPath: this.outputPath,
            projectPath: this.projectPath,
            httpPort: this.httpPort,
            httpsPort: this.httpsPort
        });
        this.app.stdout.on('data', data => logger.info(`[${this.name}]: ${data}`));
        this.app.stderr.on('data', data => logger.error(`[${this.name}]: ${data}`));
        this.app.on('exit', () => logger.info(`Exiting ${this.name}`));
        this.app.on('close', () => logger.info(`Closing ${this.name}`));
    }

    wait = async () => {
        logger.info(`Waiting for ${this.name} to be ready...`);
        await waitOn({
            resources: [this.waitUrl],
            strictSSL: false,
            delay: 1000,
            timeout: 30000
        });
        logger.info(`${this.name} is ready at ${this.baseUrl}`);
    }
    
    clean = async () => {
        return new Promise((resolve) => {
            rimraf(this.outputPath, {}, resolve);
        })
    }

    stop = async () => {
        return new Promise((resolve) => {
            logger.info(`Killing process: ${this.app.pid}`);
            treeKill(this.app.pid, 'SIGTERM', resolve);
        });
    }
}