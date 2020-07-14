const path = require('path');

const APP_DIST_PATH = path.resolve(path.join('./dist'));
const REPO_ROOT_PATH = path.resolve(path.join(__dirname, '..', '..'));
const IDENTITY_WEB_PATH = path.resolve(path.join(REPO_ROOT_PATH, 'src', 'identity', 'Haus.Identity.Web'));
const WEB_PATH = path.resolve(path.join(REPO_ROOT_PATH, 'src', 'web', 'Haus.Web'));

module.exports = {
    APP_DIST_PATH,
    REPO_ROOT_PATH,
    IDENTITY_WEB_PATH,
    WEB_PATH
};