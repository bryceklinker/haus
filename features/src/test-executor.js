import cypress from 'cypress';
import path from 'path';
import {HausApp, logger} from 'haus-runner'

const FAILURE_EXIT_CODE = 1;
const SUCCESS_EXIT_CODE = 0;
const REPOSITORY_ROOT = path.resolve(path.join(__dirname, '..', '..'))

export class TestExecutor {
    constructor() {
        this.runner = new HausApp(REPOSITORY_ROOT);
    }
    
    execute = async (type) => {
        try {
            await this.runner.clean();
            this.runner.publish();
            await this.runner.startPublishedApp();
            
            const result = await this.executeTests(type);
            this.printResult(result);
            return this.wasSuccessful(result)
                ? SUCCESS_EXIT_CODE
                : FAILURE_EXIT_CODE;
        } finally {
            await this.runner.stop();
        }
    }
    
    executeTests = async (type) => {
        switch (type) {
            case 'run':
                return await cypress.run();
            case 'open':
                return await cypress.open();
            default:
                throw new Error(`Unknown execution type: ${type}`);
        }
    }
    
    printResult = (result) => {
        logger.info(`Total Failed: ${result.totalFailed}`);
        logger.info(`Total Passed: ${result.totalPassed}`);
        logger.info(`Total Pending: ${result.totalPending}`);
        logger.info(`Total Skipped: ${result.totalSkipped}`);
        logger.info(`Total Suites: ${result.totalSuites}`);
        logger.info(`Total Tests: ${result.totalTests}`);
    }
    
    wasSuccessful = ({failures = 0, totalFailed = 0}) => {
        return failures === 0
            && totalFailed === 0;
    }
}