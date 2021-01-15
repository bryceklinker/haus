import {HealthRootHarness} from "./health-root.harness";
import {HealthActions} from "../../state";
import {ModelFactory} from "../../../../testing";

describe('HealthRootComponent', () => {
  it('should start health monitoring', async () => {
    const harness = await HealthRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(HealthActions.start());
  })

  it('should stop health monitoring when destroyed', async () => {
    const harness = await HealthRootHarness.render();

    harness.destroy();

    expect(harness.dispatchedActions).toContainEqual(HealthActions.stop());
  })

  it('should show dashboard', async () => {
    const report = ModelFactory.createHealthReportModel({
      isError: true,
      durationOfCheckInSeconds: 1.4
    });

    const harness = await HealthRootHarness.render(HealthActions.healthReceived(report));

    expect(harness.hasDashboard).toEqual(true);
  })

  it('should show waiting for health report when health report has not been received', async () => {
    const harness = await HealthRootHarness.render();

    expect(harness.hasDashboard).toEqual(false);
    expect(harness.isWaitingForReport).toEqual(true);
  })
})
