export async function eventually(expectation, interval = 500, timeout = 10000) {
    const endTime = Date.now() + timeout;
    let error = null;
    while (endTime >= Date.now()) {
        try {
            await expectation();
            return;
        } catch (err) {
            error = err;
            await sleep(interval);
        }
    }

    if (error) {
        throw error;
    }
}

function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
}
