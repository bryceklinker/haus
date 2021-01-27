import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {screen} from "@testing-library/dom";
import {LightingHarness} from "../../../shared/components/lighting/lighting.harness";

export class SimulatedDeviceWidgetHarness extends HausComponentHarness<SimulatedDeviceWidgetComponent> {
  private _lightingHarness: LightingHarness;

  get lightingExists() {
    return this._lightingHarness.exists;
  }

  get lighting() {
    return screen.getByTestId('lighting')
  }

  get simulatedMetadata() {
    return screen.queryAllByTestId('simulated-metadata-item');
  }

  async triggerOccupancyChange() {
    await this.clickButtonByTestId('trigger-occupancy-change-btn');
  }

  async getIsOccupied() {
    return this.isSlideToggleCheckedByTestId('is-occupied-toggle');
  }


  private constructor(result: RenderComponentResult<SimulatedDeviceWidgetComponent>) {
    super(result);

    this._lightingHarness = LightingHarness.fromResult(result);
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new SimulatedDeviceWidgetHarness(result);
  }

  static async render(props: Partial<SimulatedDeviceWidgetComponent>) {
    const result = await renderFeatureComponent(SimulatedDeviceWidgetComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    });

    return SimulatedDeviceWidgetHarness.fromResult(result);
  }
}
