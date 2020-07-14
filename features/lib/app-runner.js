const {spawn, spawnSync} = require('child_process');
const path = require('path');
const treeKill = require('tree-kill');
const waitOn = require('wait-on');

const {APP_DIST_PATH} = require('./app-paths');
const {APPS, IDENTITY_APP, WEB_APP} = require('./app-configs');

function getPublishPath(name) {
    return path.resolve(path.join(APP_DIST_PATH, name));
}

function publishApp({name, projectPath}) {
    const outputPath = getPublishPath(name);
    console.log(`Publishing ${name} to path: ${outputPath}...`);
    const result = spawnSync('dotnet', ['publish', '--configuration', 'Release', '--output', outputPath], {
        cwd: projectPath
    });
    if (result.status !== 0) {
        throw new Error(`Failed to publish ${name} at ${projectPath}`);
    }
    console.log(`Finished publishing ${name}.`);
}

function runApp({name, projectPath, httpPort, httpsPort}) {
    const dllName = path.basename(projectPath);
    console.log(`Running ${name} using dll name: '${dllName}'...`);
    const appProcess = spawn('dotnet', [`${dllName}.dll`], {
        cwd: getPublishPath(name),
        env: {
            ...process.env,
            ASPNETCORE_ENVIRONMENT: 'Acceptance',
            ASPNETCORE_URLS: `http://+:${httpPort};https://+:${httpsPort}`
        }
    });

    appProcess.stdout.on('data', data => console.log(`[${name}]: ${data}`));
    appProcess.stderr.on('data', data => console.error(`[${name}]: ${data}`));
    appProcess.on('exit', () => console.log(`Exiting ${name}`));
    appProcess.on('close', () => console.log(`Closing ${name}`));
    if (appProcess.exitCode) {
        throw new Error(`${name} crashed: ${appProcess.exitCode}`);
    }
    return appProcess;
}

async function waitForAppToBeReady({name, baseUrl}) {
    console.log(`Waiting for ${name} to be ready...`);
    await waitOn({
        resources: [
            `${baseUrl.replace('https', 'https-get')}/.health`
        ],
        strictSSL: false,
        delay: 1000,
        timeout: 30000
    });
    console.log(`${name} now ready at ${baseUrl}...`);
}

function stopApp(appProcess) {
    return new Promise(resolve => {
        console.log(`Killing process: ${appProcess.pid}`);
        treeKill(appProcess.pid, 'SIGTERM', resolve);
    });
}

async function startAll() {
    APPS.forEach(app => publishApp(app));
    const processes = APPS.map(app => runApp(app));
    const waits = APPS.map(app => waitForAppToBeReady(app));
    await Promise.all(waits);
    return processes;
}

async function stopAll(processes) {
    const kills = processes.forEach(p => stopApp(p));
    await Promise.all(kills);
}

module.exports = {
    startAll,
    stopAll
};