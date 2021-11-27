import {screen} from "@testing-library/dom";
import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {HealthCheckWidgetComponent} from "./health-check-widget.component";
import {HealthModule} from "../../health.module";

export class HealthCheckWidgetHarness extends HausComponentHarness<HealthCheckWidgetComponent> {

  get showingError() {
    return screen.queryAllByLabelText('error').length > 0;
  }

  get showingOk() {
    return screen.queryAllByLabelText('ok').length > 0;
  }

  get showingWarning() {
    return screen.queryAllByLabelText('warning').length > 0;
  }

  get showingExceptionMessage() {
    return screen.queryAllByRole('textbox', {name: 'exception-message'}).length > 0;
  }

  get showingDescription() {
    return screen.queryAllByRole('textbox', {name: 'description'}).length > 0;
  }

  getName() {
    return Promise.resolve(screen.queryAllByLabelText('name').map(e => e.innerHTML).join(' '));
  }

  async getStatus() {
    return await this.getInputValueByLabel('status');
  }

  async getDescription() {
    return await this.getInputValueByLabel('description');
  }

  async getCheckDuration() {
    return await this.getInputValueByLabel('check duration');
  }

  async getExceptionMessage() {
    return await this.getInputValueByLabel('exception message');
  }

  async getTagsText() {
    return await this.getInputValueByLabel('tags');
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
