import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorDashboardComponent} from "./device-simulator-dashboard.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {SimulatedDeviceWidgetHarness} from "../simulated-device-widget/simulated-device-widget.harness";

export class DeviceSimulatorDashboardHarness extends HausComponentHarness<DeviceSimulatorDashboardComponent> {
  private readonly _simulatedDeviceHarness: SimulatedDeviceWidgetHarness;

  get simulatedDevices() {
    return screen.queryAllByTestId('simulated-device-item')
  }

  async triggerOccupancyChange() {
    await this._simulatedDeviceHarness.triggerOccupancyChange();
  }

  constructor(result: RenderComponentResult<DeviceSimulatorDashboardComponent>) {
    super(result);

    this._simulatedDeviceHarness = SimulatedDeviceWidgetHarness.fromResult(result);
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DeviceSimulatorDashboardHarness(result);
  }

  static async render(props: Partial<DeviceSimulatorDashboardComponent>) {
    const result = await renderFeatureComponent(DeviceSimulatorDashboardComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    });

    return new DeviceSimulatorDashboardHarness(result);
  }
}
