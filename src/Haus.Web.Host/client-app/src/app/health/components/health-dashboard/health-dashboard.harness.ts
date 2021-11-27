import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {HealthDashboardComponent} from "./health-dashboard.component";
import {HealthModule} from "../../health.module";
import {screen} from "@testing-library/dom";
import {HealthCheckWidgetHarness} from "../health-check-widget/health-check-widget.harness";

export class HealthDashboardHarness extends HausComponentHarness<HealthDashboardComponent> {
  private readonly _healthCheckHarness: HealthCheckWidgetHarness;

  get isVisible() {
    return screen.queryAllByLabelText('health dashboard').length > 0;
  }

  get isWaiting() {
    return screen.queryAllByLabelText('waiting for report').length > 0;
  }

  get isError() {
    return screen.queryAllByLabelText('error indicator').length > 0;
  }

  get isWarning() {
    return screen.queryAllByLabelText('warning indicator').length > 0;
  }

  get isOk() {
    return screen.queryAllByLabelText('ok indicator').length > 0;
  }

  get checks() {
    return screen.queryAllByLabelText('health check');
  }

  async getCheckName() {
    return this._healthCheckHarness.getName();
  }

  private constructor(result: RenderComponentResult<HealthDashboardComponent>) {
    super(result);

    this._healthCheckHarness = HealthCheckWidgetHarness.fromResult(result);
  }


  static fromResult(result: RenderComponentResult<any>) {
    return new HealthDashboardHarness(result);
  }

  static async render(props: Partial<HealthDashboardComponent> = {}) {
    const result = await renderFeatureComponent(HealthDashboardComponent, {
      imports: [HealthModule],
      componentProperties: props
    })

    return new HealthDashboardHarness(result);
  }
}
