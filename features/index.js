import {logger} from 'haus-runner';
import {TestExecutor} from "./src";

const executor = new TestExecutor();

executor.execute(process.argv[2])
    .then(exitCode => process.exit(exitCode))
    .catch(err => logger.error(err));