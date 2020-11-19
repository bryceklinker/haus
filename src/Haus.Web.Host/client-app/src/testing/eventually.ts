type Expectation = (() => void) | (() => Promise<void>);

export async function eventually(expectation: Expectation, timeout: number = 4000, delay: number = 100) {
  const endTime = Date.now() + timeout;
  let expectationException: Error | null = null;
  while (endTime >= Date.now()) {
    try {
      await expectation();
      return;
    } catch (error) {
      expectationException = error;
      await sleep(delay);
    }
  }

  if (expectationException) {
    throw expectationException;
  }
}

function sleep(time: number): Promise<void> {
  return new Promise<void>((resolve) => setTimeout(resolve, time));
}
