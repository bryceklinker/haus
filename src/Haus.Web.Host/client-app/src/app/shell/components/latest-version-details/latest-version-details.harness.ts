import {screen} from '@testing-library/dom';
import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {LatestVersionDetailsComponent} from "./latest-version-details.component";
import userEvent from "@testing-library/user-event";

export class LatestVersionDetailsHarness extends HausComponentHarness<LatestVersionDetailsComponent> {

  get isShowing() {
    return screen.queryAllByLabelText('latest version details').length > 0;
  }

  get versionElement() {
    return screen.getByLabelText('version');
  }

  get descriptionElement() {
    return screen.getByLabelText('description');
  }

  get packages() {
    return screen.queryAllByLabelText('application package');
  }

  get releaseDate() {
    return screen.getByLabelText('release date');
  }

  async getIsNewerRelease() {
    return await this.isSlideToggleCheckedByLabel('is newer');
  }

  async getIsOfficialRelease() {
    return await this.isSlideToggleCheckedByLabel('is official');
  }

  async downloadPackage() {
    userEvent.click(screen.getByRole('button', {name: 'application package'}));
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
