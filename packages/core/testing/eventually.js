export async function eventually(expectation, interval = 50, timeout = 4000) {
    let error = null;
    const endTime = Date.now() + timeout;
    while (endTime >= Date.now()) {
        try {
            await expectation();
            return;
        } catch (e) {
            error = e;
            await sleep(interval);
        }
    }

    if (error) {
        throw error;
    }
}


function sleep(time) {
    return new Promise((resolve) => {
        setTimeout(resolve, time);
    })
}
