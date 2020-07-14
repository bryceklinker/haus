const { IDENTITY_WEB_PATH, WEB_PATH } = require('./app-paths');

const IDENTITY_APP = {
    name: 'identity',
    projectPath: IDENTITY_WEB_PATH,
    baseUrl: 'https://localhost:5003',
    httpsPort: 5003,
    httpPort: 5002
};

const WEB_APP = {
    name: 'web',
    projectPath: WEB_PATH,
    baseUrl: 'https://localhost:5001',
    httpsPort: 5001,
    httpPort: 5000
};

const APPS = [
    IDENTITY_APP,
    WEB_APP
]

module.exports = { APPS, IDENTITY_APP, WEB_APP };