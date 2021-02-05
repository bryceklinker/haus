import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {LatestVersionDetailsRootComponent} from "./latest-version-details-root.component";
import {Action} from "@ngrx/store";
import {LatestVersionDetailsHarness} from "../latest-version-details/latest-version-details.harness";
import {screen} from "@testing-library/dom";
import {LatestVersionErrorHarness} from "../latest-version-error/latest-version-error.harness";
import {ShellActions} from "../../state";

export class LatestVersionDetailsRootHarness extends HausComponentHarness<LatestVersionDetailsRootComponent> {
  private readonly _latestVersionDetailsHarness: LatestVersionDetailsHarness;
  private readonly _latestVersionErrorHarness: LatestVersionErrorHarness;

  get errorElement() {
    return this._latestVersionErrorHarness.errorElement;
  }

  get isShowingLatestVersionDetails() {
    return this._latestVersionDetailsHarness.isShowing;
  }

  get isShowingLatestError() {
    return screen.queryAllByTestId('latest-version-error').length > 0;
  }

  get descriptionElement() {
    return this._latestVersionDetailsHarness.descriptionElement;
  }

  get versionElement() {
    return this._latestVersionDetailsHarness.versionElement;
  }

  get packages() {
    return this._latestVersionDetailsHarness.packages;
  }

  async downloadPackage() {
    await this._latestVersionDetailsHarness.downloadPackage();
  }

  async retry() {
    await this._latestVersionErrorHarness.retry();
  }

  simulateDownloadComplete() {
    this.actionsSubject.next(ShellActions.downloadPackage.success());
  }

  constructor(result: RenderComponentResult<any>) {
    super(result);

    this._latestVersionDetailsHarness = LatestVersionDetailsHarness.fromResult(result);
    this._latestVersionErrorHarness = LatestVersionErrorHarness.fromResult(result);
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LatestVersionDetailsRootHarness(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderAppComponent(LatestVersionDetailsRootComponent, {
      actions
    });
    return LatestVersionDetailsRootHarness.fromResult(result);
  }
}
