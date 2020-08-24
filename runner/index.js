import readline from 'readline';
import path from 'path';

import{ HausApp, logger } from "./src";

const REPOSITORY_ROOT = path.resolve(path.join(__dirname, '..'));

function waitForUserToStop() {
    return new Promise((resolve) => {
        const rl = readline.createInterface({
            input: process.stdin,
            output: process.stdout
        });
        rl.question('Press enter to exit', () => {
          rl.close();
          resolve();
        });
    })
}

async function main() {
    const app = new HausApp(REPOSITORY_ROOT);
    try {
        await app.start();
        await app.waitToForAppToBeReady();
        logger.info('Haus is ready...');
        await waitForUserToStop();
    } finally {
        await app.stop();
    }
}

main().catch(err => logger.error(err));
