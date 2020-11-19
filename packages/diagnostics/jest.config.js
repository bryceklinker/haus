const base = require('../../jest.config');
const packageJson = require('./package.json');

module.exports = {
    ...base,
    name: packageJson.name,
    displayName: packageJson.name,
    setupFilesAfterEnv: ['<rootDir>/src/jest.setup.ts']
}
