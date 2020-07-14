const { appRunner, databaseRunner, executor, resultsHandler, cleaner} = require('./lib');

async function main(arg) {
    let apps = [];
    let exitCode = 1;
    try {
        await cleaner.cleanAll();
        databaseRunner.startDatabase()
        apps = await appRunner.startAll();
        const results = await executor.execute(arg);
        exitCode = resultsHandler.handle(results);
    } finally {
        databaseRunner.stopDatabase();
        await appRunner.stopAll(apps);
    }
    process.exit(exitCode);
}

main(process.argv[2])
    .catch(err => console.error(`Tests failed: ${err}`))