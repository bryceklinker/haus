import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RecentEventsComponent} from "./recent-events.component";
import {HealthModule} from "../../health.module";
import {screen} from "@testing-library/dom";

export class RecentEventsHarness extends HausComponentHarness<RecentEventsComponent> {

  get events() {
    return screen.queryAllByTestId('haus-event');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new RecentEventsHarness(result);
  }

  static async render(props: Partial<RecentEventsComponent>) {
    const result = await renderFeatureComponent(RecentEventsComponent, {
      imports: [HealthModule],
      componentProperties: props
    });

    return RecentEventsHarness.fromResult(result);
  }
}
