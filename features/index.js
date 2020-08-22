import {TestExecutor} from "./src";
import {logger} from "./src/logger";

const executor = new TestExecutor();

executor.execute(process.argv[2])
    .then(exitCode => process.exit(exitCode))
    .catch(err => logger.error(err));