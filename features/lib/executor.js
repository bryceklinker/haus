const cypress = require('cypress');

function execute(runType) {
    switch (runType) {
        case "run":
            return cypress.run();
        case "open":
            return cypress.open();
        default:
            throw new Error(`Unknown run type ${runType}.`)
    }
}

module.exports = { execute };