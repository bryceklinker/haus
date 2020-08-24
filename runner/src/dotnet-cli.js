import {spawn, spawnSync} from 'child_process'
import path from "path";

function publish({projectPath, outputPath, configuration = 'Release'}) {
    const args = [
        'publish',
        '--configuration', configuration,
        '--output', outputPath
    ]
    const result = spawnSync('dotnet', args, {
        cwd: projectPath
    });
    return { success: result.status === 0, output: result.stdout };
}

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

function exec({projectPath, publishPath, httpPort, httpsPort}) {
    const dllName = `${path.basename(projectPath)}.dll`;
    const runProcess = spawn('dotnet', [dllName], {
        cwd: publishPath,
        env: {
            ...process.env,
            ASPNETCORE_ENVIRONMENT: 'Acceptance',
            ASPNETCORE_URLS: `http://+:${httpPort};https://+:${httpsPort}`
        }
    });
    if (runProcess.exitCode) {
        throw new Error(`Failed to run ${dllName} in ${publishPath}`);
    }
    return runProcess;
}

export default { run, exec, publish };