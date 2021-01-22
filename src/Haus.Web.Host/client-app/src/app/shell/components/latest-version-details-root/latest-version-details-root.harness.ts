import {HausComponentHarness, renderAppComponent, RenderComponentResult} from "../../../../testing";
import {LatestVersionDetailsRootComponent} from "./latest-version-details-root.component";
import {Action} from "@ngrx/store";
import {LatestVersionDetailsHarness} from "../latest-version-details/latest-version-details.harness";

export class LatestVersionDetailsRootHarness extends HausComponentHarness<LatestVersionDetailsRootComponent> {
  private readonly _latestVersionDetailsHarness: LatestVersionDetailsHarness;

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

  constructor(result: RenderComponentResult<any>) {
    super(result);

    this._latestVersionDetailsHarness = LatestVersionDetailsHarness.fromResult(result);
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
