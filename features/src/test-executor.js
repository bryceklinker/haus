import cypress from 'cypress';
import {ALL} from "./web-app-options";
import {WebApp} from "./web-app";
import {Database} from "./database";
import {logger} from "./logger";

const FAILURE_EXIT_CODE = 1;
const SUCCESS_EXIT_CODE = 0;

export class TestExecutor {
    constructor() {
        this.apps = ALL.map(opts => new WebApp(opts));
        this.db = new Database();
    }
    
    execute = async (type) => {
        try {
            await this.clean();
            await this.start();
            const result = await this.executeTests(type);
            this.printResult(result);
            return this.wasSuccessful(result)
                ? SUCCESS_EXIT_CODE
                : FAILURE_EXIT_CODE;
        } finally {
            await this.stop();
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
    
    start = async () => {
        for (let i = 0; i < this.apps.length; i++) {
            this.apps[i].publish();
        }
        const promises = this.apps.map(a => a.start());
        this.db.start();
        await Promise.all(promises);
    }
    
    clean = async () => {
        const promises = this.apps.map(a => a.clean());
        await Promise.all(promises);
    }
    
    stop = async () => {
        const promises = this.apps.map(a => a.stop());
        this.db.stop();
        await Promise.all(promises);
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