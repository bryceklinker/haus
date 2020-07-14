const rimraf = require('rimraf');

const {APP_DIST_PATH} = require('./app-paths');

function cleanAll() {
    return new Promise(resolve => {
        rimraf(APP_DIST_PATH, {}, resolve);
    })
}

module.exports = {cleanAll};