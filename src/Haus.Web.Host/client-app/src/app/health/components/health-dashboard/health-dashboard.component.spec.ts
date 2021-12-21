import {HealthDashboardHarness} from "./health-dashboard.harness";
import {ModelFactory} from "../../../../testing";
import {HealthStatus} from "../../../shared/models";

describe('HealthDashboardComponent', () => {
  test('should show error status when report is down', async () => {
    const report = ModelFactory.createHealthReportModel({
      isOk: false,
      isError: true,
      status: HealthStatus.Unhealthy
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.isError).toEqual(true);
    expect(harness.isWarning).toEqual(false);
    expect(harness.isOk).toEqual(false);
  })

  test('should show warning status when report is warning', async () => {
    const report = ModelFactory.createHealthReportModel({
      isOk: false,
      isError: false,
      isWarn: true,
      status: HealthStatus.Degraded
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.isWarning).toEqual(true);
    expect(harness.isError).toEqual(false);
    expect(harness.isOk).toEqual(false);
  })

  test('should show good status when report is ok', async () => {
    const report = ModelFactory.createHealthReportModel({
      isOk: true,
      isError: false,
      status: HealthStatus.Healthy
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.isOk).toEqual(true);
    expect(harness.isError).toEqual(false);
    expect(harness.isWarning).toEqual(false);
  })

  test('should show each health check', async () => {
    const report = ModelFactory.createHealthReportModel({
      checks: [
        ModelFactory.createHealthCheckModel({name: 'bob'}),
        ModelFactory.createHealthCheckModel(),
        ModelFactory.createHealthCheckModel()
      ]
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.checks).toHaveLength(3);
    expect(await harness.getCheckName()).toContain('bob');
  })
})
