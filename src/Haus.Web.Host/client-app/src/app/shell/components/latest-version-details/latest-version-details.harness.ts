import {screen} from '@testing-library/dom';
import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {LatestVersionDetailsComponent} from "./latest-version-details.component";
import userEvent from "@testing-library/user-event";

export class LatestVersionDetailsHarness extends HausComponentHarness<LatestVersionDetailsComponent> {

  get isShowing() {
    return screen.queryAllByTestId('latest-version-details').length > 0;
  }

  get versionElement() {
    return screen.getByTestId('version');
  }

  get descriptionElement() {
    return screen.getByTestId('description');
  }

  get packages() {
    return screen.queryAllByTestId('application-package');
  }

  async getIsNewerRelease() {
    return await this.isSlideToggleCheckedByTestId('is-newer');
  }

  async getIsOfficialRelease() {
    return await this.isSlideToggleCheckedByTestId('is-official');
  }

  async getReleaseDate() {
    return await this.getInputValueByTestId('release-date');
  }

  async downloadPackage() {
    userEvent.click(screen.getByTestId('application-package'));
    await this.whenRenderingDone();
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LatestVersionDetailsHarness(result);
  }

  static async render(props: Partial<LatestVersionDetailsComponent> = {}) {
    const result = await renderAppComponent(LatestVersionDetailsComponent, {componentProperties: props});
    return LatestVersionDetailsHarness.fromResult(result);
  }
}
