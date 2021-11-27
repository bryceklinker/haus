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
    return screen.getByLabelText('lighting')
  }

  get simulatedMetadata() {
    return screen.queryAllByLabelText('simulated metadata item');
  }

  async triggerOccupancyChange() {
    await this.clickButtonByLabel('trigger occupancy change');
  }

  async getIsOccupied() {
    return this.isSlideToggleCheckedByLabel('is occupied toggle');
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
