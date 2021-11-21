import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {AddSimulatedDeviceComponent} from "./add-simulated-device.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {DeviceSimulatorActions} from "../../state";
import {DeviceType} from "../../../shared/models";
import {DeviceSimulatorDashboardComponent} from "../device-simulator-dashboard/device-simulator-dashboard.component";

export class AddSimulatedDeviceHarness extends HausComponentHarness<AddSimulatedDeviceComponent> {

  get saveButton() {
    return screen.getByTestId('save-simulated-device-btn');
  }

  constructor(result: RenderComponentResult<AddSimulatedDeviceComponent>) {
    super(result);

    jest.spyOn(this.router, 'navigateByUrl');
  }

  async enterMetadataKey(key: string) {
    await this.changeInputByTestId('something', 'metadata-key-input');
  }

  async enterMetadataValue(value: string) {
    await this.changeInputByTestId('else', 'metadata-value-input');
  }

  async addMetadata() {
    await this.clickButtonByTestId('add-metadata-btn');
  }

  async selectDeviceType(deviceType: DeviceType) {
    await this.changeSelectedOptionByTestId(deviceType, 'device-type-select');
  }

  async save() {
    await this.clickButtonByTestId('save-simulated-device-btn')
  }

  async cancel() {
    await this.clickButtonByTestId('cancel-simulated-device-btn');
  }

  async simulateAddSuccess() {
    this.actionsSubject.next(DeviceSimulatorActions.addSimulatedDevice.success());
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(AddSimulatedDeviceComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions,
      routes: [
        {path: 'device-simulator', component: DeviceSimulatorDashboardComponent}
      ]
    });

    return new AddSimulatedDeviceHarness(result);
  }
}
