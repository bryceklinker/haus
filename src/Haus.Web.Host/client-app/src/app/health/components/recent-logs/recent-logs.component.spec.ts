import {RecentLogsHarness} from "./recent-logs.harness";
import {ModelFactory} from "../../../../testing";

describe('RecentLogsComponent', () => {
  test('should show logs when rendered', async () => {
    const logs = [
      ModelFactory.createLogEntry(),
      ModelFactory.createLogEntry(),
      ModelFactory.createLogEntry(),
      ModelFactory.createLogEntry()
    ]
    const harness = await RecentLogsHarness.render({logs});

    expect(harness.logs).toHaveLength(4);
  })

  test('should show log details', async () => {
    const log = ModelFactory.createLogEntry();

    const harness = await RecentLogsHarness.render({logs: [log]});

    expect(harness.container).toHaveTextContent(log.message);
    expect(harness.container).toHaveTextContent(log.timestamp);
    expect(harness.container).toHaveTextContent(log.level);
  })

  test('should be loading when logs is empty', async () => {
    const harness = await RecentLogsHarness.render();

    expect(harness.isLoading).toEqual(true);
  })
})
