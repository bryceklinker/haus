import {HealthCheckWidgetHarness} from "./health-check-widget.harness";
import {ModelFactory} from "../../../../testing";
import {HealthStatus} from "../../../shared/models";

describe('HealthCheckWidgetComponent', () => {
  it('should show error status for health check', async () => {
    const check = ModelFactory.createHealthCheckModel({
      isOk: false,
      isError: true,
      isWarn: false,
      status: HealthStatus.Unhealthy
    });

    const harness = await HealthCheckWidgetHarness.render({check});

    expect(harness.showingError).toEqual(true);
    expect(harness.showingOk).toEqual(false);
    expect(harness.showingWarning).toEqual(false);
  })

  it('should show ok status for health check', async () => {
    const check = ModelFactory.createHealthCheckModel({
      isOk: true,
      isError: false,
      isWarn: false,
      status: HealthStatus.Healthy
    });

    const harness = await HealthCheckWidgetHarness.render({check});

    expect(harness.showingOk).toEqual(true);
    expect(harness.showingError).toEqual(false);
    expect(harness.showingError).toEqual(false);
  })

  it('should show warning status for health check', async () => {
    const check = ModelFactory.createHealthCheckModel({
      isOk: false,
      isError: false,
      isWarn: true,
      status: HealthStatus.Degraded
    });

    const harness = await HealthCheckWidgetHarness.render({check});

    expect(harness.showingOk).toEqual(false);
    expect(harness.showingError).toEqual(false);
    expect(harness.showingWarning).toEqual(true);
  })

  it('should show health check details', async () => {
    const check = ModelFactory.createHealthCheckModel({
      durationOfCheckInSeconds: 6.5,
      name: 'Good stuff',
      description: 'Something good is happening',
      exceptionMessage: 'This is not good',
      status: HealthStatus.Healthy,
      tags: [
        'Taggy'
      ]
    });

    const harness = await HealthCheckWidgetHarness.render({check});

    expect(await harness.getName()).toContain('Good stuff');
    expect(await harness.getDescription()).toContain('Something good is happening');
    expect(await harness.getCheckDuration()).toContain('6.5 seconds');
    expect(await harness.getExceptionMessage()).toContain('This is not good');
    expect(await harness.getStatus()).toContain(HealthStatus.Healthy);
    expect(await harness.getTagsText()).toContain('Taggy');
  })
})
