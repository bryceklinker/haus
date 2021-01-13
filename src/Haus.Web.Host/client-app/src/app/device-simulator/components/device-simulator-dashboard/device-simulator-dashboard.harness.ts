import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorDashboardComponent} from "./device-simulator-dashboard.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";

export class DeviceSimulatorDashboardHarness extends HausComponentHarness<DeviceSimulatorDashboardComponent> {
  get simulatedDevices() {
    return screen.queryAllByTestId('simulated-device-item')
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
