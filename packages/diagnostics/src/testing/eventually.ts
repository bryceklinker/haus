type Expectation = () => void | Promise<void>;

export async function eventually(expectation: Expectation, timeout: number = 4000, interval: number = 100) {
    const endTime = Date.now() + timeout;
    let error: Error | null | undefined = null;

    while (endTime > Date.now()) {
        try {
            await expectation();
            return;
        } catch(e) {
            error = e;
            await sleep(interval);
        }

        if (error) {
            throw error;
        }
    }
}

function sleep(time: number): Promise<void> {
    return new Promise<void>((resolve) => setTimeout(resolve, time));
}
