const cypress = require('cypress');
const waitOn = require('wait-on');

const EXIT_CODES = {
    SERVER_DID_NOT_START: 1,
    TESTS_FAILED: 2,
    UNEXPECTED_ERROR: 3
}

async function waitForServer() {
    try {
        await waitOn({
            resources: [
                'http://localhost:5000'
            ]
        })
    } catch (err) {
        console.error('Server failed to start', err);
        process.exit(EXIT_CODES.SERVER_DID_NOT_START);
    }
}

async function executeTests(executionStyle = 'run') {
    try {
        const executor = executionStyle === 'run' ? () => cypress.run() : () => cypress.open();
        const results = await executor();
        console.log(results);
    } catch (err) {
        console.error('Tests Failed', err);
        process.exit(EXIT_CODES.TESTS_FAILED);
    }
}

async function main(executionStyle) {
    await waitForServer();
    await executeTests(executionStyle);
}

main(process.argv[2])
    .catch(err => {
        console.log('Unexpected error', err);
        process.exit(EXIT_CODES.UNEXPECTED_ERROR);
    });

