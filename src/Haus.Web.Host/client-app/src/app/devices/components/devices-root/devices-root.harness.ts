import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DevicesRootComponent} from "./devices-root.component";
import {Action} from "@ngrx/store";
import {DevicesModule} from "../../devices.module";
import {screen} from "@testing-library/dom";

export class DevicesRootHarness extends HausComponentHarness<DevicesRootComponent> {
  get deviceItems() {
    return screen.queryAllByTestId('device-item');
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DevicesRootComponent, {
      imports: [DevicesModule],
      actions: actions
    });

    return new DevicesRootHarness(result);
  }
}
