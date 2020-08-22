import path from 'path';
import {spawnSync, spawn} from 'child_process'

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

function run({projectPath, publishPath, httpPort, httpsPort}) {
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

export default { publish, run };