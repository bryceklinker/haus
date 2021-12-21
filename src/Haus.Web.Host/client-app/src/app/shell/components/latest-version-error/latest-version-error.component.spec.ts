import {LatestVersionErrorHarness} from "./latest-version-error.harness";
import {TestingEventEmitter} from "../../../../testing";

describe('LatestVersionErrorComponent', () => {
  test('should notify of retrying', async () => {
    const emitter = new TestingEventEmitter<void>();

    const harness = await LatestVersionErrorHarness.render({retry: emitter});
    await harness.retry();

    expect(emitter.emit).toHaveBeenCalled();
  })

  test('should show error message', async () => {
    const error = new Error('What just happened?');

    const harness = await LatestVersionErrorHarness.render({error});

    expect(harness.errorElement).toHaveTextContent('What just happened?');
  })
})
