import {HealthRootHarness} from "./health-root.harness";
import {HealthActions} from "../../state";
import {ModelFactory} from "../../../../testing";
import {EventsActions} from "../../../shared/events";

describe('HealthRootComponent', () => {
  test('should start health monitoring', async () => {
    const harness = await HealthRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(HealthActions.start());
  })

  test('should stop health monitoring when destroyed', async () => {
    const harness = await HealthRootHarness.render();

    harness.destroy();

    expect(harness.dispatchedActions).toContainEqual(HealthActions.stop());
  })

  test('should show dashboard', async () => {
    const report = ModelFactory.createHealthReportModel({
      isError: true,
      durationOfCheckInSeconds: 1.4
    });

    const harness = await HealthRootHarness.render(HealthActions.healthReceived(report));

    expect(harness.hasDashboard).toEqual(true);
  })

  test('should show waiting for health report when health report has not been received', async () => {
    const harness = await HealthRootHarness.render();

    expect(harness.hasDashboard).toEqual(false);
    expect(harness.isWaitingForReport).toEqual(true);
  })

  test('should show recent events', async () => {
    const harness = await HealthRootHarness.render(
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: ''}),
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: ''}),
      EventsActions.fromHausEvent({type: '', payload: null, timestamp: ''})
    );

    expect(harness.recentEvents).toHaveLength(3);
  })

  test('should show recent logs', async () => {
    const harness = await HealthRootHarness.render(
      HealthActions.loadRecentLogs.success(
        ModelFactory.createListResult(
          ModelFactory.createLogEntry(),
          ModelFactory.createLogEntry(),
        ),
      )
    );

    expect(harness.recentLogs).toHaveLength(2);
  })
})
