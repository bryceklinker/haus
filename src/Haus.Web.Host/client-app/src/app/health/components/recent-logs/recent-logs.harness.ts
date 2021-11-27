import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RecentLogsComponent} from "./recent-logs.component";
import {HealthModule} from "../../health.module";
import {screen} from "@testing-library/dom";

export class RecentLogsHarness extends HausComponentHarness<RecentLogsComponent> {

  get logs() {
    return screen.queryAllByLabelText('log entry');
  }

  get isLoading() {
    return screen.queryAllByLabelText('loading indicator').length > 0;
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new RecentLogsHarness(result);
  }

  static async render(props: Partial<RecentLogsComponent> = {}) {
    const result = await renderFeatureComponent(RecentLogsComponent, {
      imports: [HealthModule],
      componentProperties: props
    });

    return RecentLogsHarness.fromResult(result);
  }
}
