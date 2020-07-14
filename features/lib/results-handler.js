function wasSuccessful({failures = 0, totalFailed = 0}) {
    return failures === 0
        && totalFailed === 0;
}

function showResults({
                         totalFailed = 0,
                         totalPassed = 0,
                         totalPending = 0,
                         totalSkipped = 0,
                         totalSuites = 0,
                         totalTests = 0
                     }) {
    console.log(`Total Failed: ${totalFailed}`);
    console.log(`Total Passed: ${totalPassed}`);
    console.log(`Total Pending: ${totalPending}`);
    console.log(`Total Skipped: ${totalSkipped}`);
    console.log(`Total Suites: ${totalSuites}`);
    console.log(`Total Tests: ${totalTests}`);
}

function handle(results) {
    showResults(results);
    return wasSuccessful(results) ? 0 : 1;
}

module.exports = { handle } ;