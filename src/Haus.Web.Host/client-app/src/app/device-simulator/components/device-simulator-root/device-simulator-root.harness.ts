import {Action} from "@ngrx/store";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorRootComponent} from "./device-simulator-root.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {DeviceSimulatorDashboardHarness} from "../device-simulator-dashboard/device-simulator-dashboard.harness";

export class DeviceSimulatorRootHarness extends HausComponentHarness<DeviceSimulatorRootComponent> {
  private _deviceSimulatorDashboardHarness: DeviceSimulatorDashboardHarness;

  get simulatedDevices() {
    return this._deviceSimulatorDashboardHarness.simulatedDevices;
  }

  async triggerOccupancyChange() {
    await this._deviceSimulatorDashboardHarness.triggerOccupancyChange();
  }

  private constructor(result: RenderComponentResult<DeviceSimulatorRootComponent>) {
    super(result);

    this._deviceSimulatorDashboardHarness = DeviceSimulatorDashboardHarness.fromResult(result);
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DeviceSimulatorRootComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions
    });
    return new DeviceSimulatorRootHarness(result);
  }
}
