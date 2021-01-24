import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {LatestVersionErrorComponent} from "./latest-version-error.component";
import {screen} from "@testing-library/dom";

export class LatestVersionErrorHarness extends HausComponentHarness<LatestVersionErrorComponent>{
  get errorElement() {
    return screen.getByTestId('error-message');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LatestVersionErrorHarness(result);
  }

  static async render(props: Partial<LatestVersionErrorComponent>) {
    const result = await renderAppComponent(LatestVersionErrorComponent, {
      componentProperties: props
    });
    return LatestVersionErrorHarness.fromResult(result);
  }

  async retry() {
    await this.clickButtonByTestId('retry-btn');
  }
}
