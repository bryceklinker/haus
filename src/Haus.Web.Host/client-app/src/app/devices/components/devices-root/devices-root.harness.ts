import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DevicesRootComponent} from "./devices-root.component";
import {Action} from "@ngrx/store";
import {DevicesModule} from "../../devices.module";
import {screen} from "@testing-library/dom";
import {DevicesListHarness} from "../devices-list/devices-list.harness";

export class DevicesRootHarness extends HausComponentHarness<DevicesRootComponent> {
  private _devicesListHarness: DevicesListHarness;

  get deviceItems() {
    return this._devicesListHarness.deviceItems;
  }

  private constructor(result: RenderComponentResult<DevicesRootComponent>) {
    super(result);

    this._devicesListHarness = DevicesListHarness.fromResult(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DevicesRootComponent, {
      imports: [DevicesModule],
      actions: actions
    });

    return new DevicesRootHarness(result);
  }
}
