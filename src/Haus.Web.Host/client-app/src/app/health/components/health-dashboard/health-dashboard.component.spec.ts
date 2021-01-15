import {HealthDashboardHarness} from "./health-dashboard.harness";
import {ModelFactory} from "../../../../testing";
import {HealthStatus} from "../../../shared/models";

describe('HealthDashboardComponent', () => {
  it('should show error status when report is down', async () => {
    const report = ModelFactory.createHealthReportModel({
      isOk: false,
      isError: true,
      status: HealthStatus.Unhealthy
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.hasErrors).toEqual(true);
    expect(harness.isOk).toEqual(false);
  })

  it('should show good status when report is ok', async () => {
    const report = ModelFactory.createHealthReportModel({
      isOk: true,
      isError: false,
      status: HealthStatus.Healthy
    });

    const harness = await HealthDashboardHarness.render({report});

    expect(harness.hasErrors).toEqual(false);
    expect(harness.isOk).toEqual(true);
  })

  it('should show each health check', async () => {
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
