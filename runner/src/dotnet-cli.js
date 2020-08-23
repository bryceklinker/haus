import {spawn} from 'child_process'

function run({projectPath, httpPort, httpsPort, environment = 'Development'}) {
    const runProcess = spawn('dotnet', ['run'], {
        cwd: projectPath,
        env: {
            ...process.env,
            ASPNETCORE_ENVIRONMENT: environment,
            ASPNETCORE_URLS: `http://0.0.0.0:${httpPort};https://0.0.0.0:${httpsPort}`
        }
    });
    if (runProcess.exitCode) {
        throw new Error(`Failed to run ${projectPath}`);
    }
    return runProcess;
}

export default { run };