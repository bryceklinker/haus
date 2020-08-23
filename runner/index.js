import readline from 'readline';
import{ HausApp } from "./src/haus-app";
import {logger} from "./src/logger";

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
    const app = new HausApp();
    try {
        app.start();
        await app.waitToForAppToBeReady();
        logger.info('Haus is ready...');
        await waitForUserToStop();
    } finally {
        await app.stop();
    }
}

main().catch(err => logger.error(err));
