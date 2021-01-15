import {screen} from "@testing-library/dom";
import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {HealthCheckWidgetComponent} from "./health-check-widget.component";
import {HealthModule} from "../../health.module";

export class HealthCheckWidgetHarness extends HausComponentHarness<HealthCheckWidgetComponent> {

  get showingError() {
    return screen.queryAllByTestId('error').length > 0;
  }

  get showingOk() {
    return screen.queryAllByTestId('ok').length > 0;
  }

  get showingWarning() {
    return screen.queryAllByTestId('warning').length > 0;
  }

  getName() {
    return Promise.resolve(screen.queryAllByTestId('name').map(e => e.innerHTML).join(' '));
  }

  async getStatus() {
    return await this.getInputValueByTestId('status');
  }

  async getDescription() {
    return await this.getInputValueByTestId('description');
  }

  async getCheckDuration() {
    return await this.getInputValueByTestId('check-duration');
  }

  async getExceptionMessage() {
    return await this.getInputValueByTestId('exception-message');
  }

  async getTagsText() {
    return await this.getInputValueByTestId('tags');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new HealthCheckWidgetHarness(result);
  }

  static async render(props: Partial<HealthCheckWidgetComponent> = {}) {
    const result = await renderFeatureComponent(HealthCheckWidgetComponent, {
      imports: [HealthModule],
      componentProperties: props
    });

    return HealthCheckWidgetHarness.fromResult(result);
  }
}
