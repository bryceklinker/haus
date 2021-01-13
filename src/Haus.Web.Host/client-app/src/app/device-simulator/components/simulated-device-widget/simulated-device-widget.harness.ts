import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {screen} from "@testing-library/dom";

export class SimulatedDeviceWidgetHarness extends HausComponentHarness<SimulatedDeviceWidgetComponent> {
  get lighting() {
    return screen.getByTestId('lighting')
  }

  get simulatedMetadata() {
    return screen.queryAllByTestId('simulated-metadata-item');
  }

  static async render(props: Partial<SimulatedDeviceWidgetComponent>) {
    const result = await renderFeatureComponent(SimulatedDeviceWidgetComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    });

    return new SimulatedDeviceWidgetHarness(result);
  }
}
