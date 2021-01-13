import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorRootComponent} from "./device-simulator-root.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";

export class DeviceSimulatorRootHarness extends HausComponentHarness<DeviceSimulatorRootComponent> {
  get simulatedDevices() {
    return screen.queryAllByTestId('simulated-device-item')
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DeviceSimulatorRootComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions
    });
    return new DeviceSimulatorRootHarness(result);
  }
}
