import userEvent from "@testing-library/user-event";
import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";
import {MatSelectHarness} from "@angular/material/select/testing";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {AddSimulatedDeviceComponent} from "./add-simulated-device.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {DeviceSimulatorActions} from "../../state";
import {DeviceType} from "../../../shared/models";

export class AddSimulatedDeviceHarness extends HausComponentHarness<AddSimulatedDeviceComponent> {

  get saveButton() {
    return screen.getByTestId('save-simulated-device-btn');
  }

  constructor(result: RenderComponentResult<AddSimulatedDeviceComponent>) {
    super(result);

    spyOn(this.router, 'navigateByUrl');
  }

  async enterMetadataKey(key: string) {
    userEvent.type(screen.getByTestId('metadata-key-input'), 'something');
    this.detectChanges();
    await this.whenRenderingDone();
  }

  async enterMetadataValue(value: string) {
    userEvent.type(screen.getByTestId('metadata-value-input'), 'else');
    this.detectChanges();
    await this.whenRenderingDone();
  }

  async addMetadata() {
    userEvent.click(screen.getByTestId('add-metadata-btn'));
    this.detectChanges();
    await this.whenRenderingDone();
  }

  async selectDeviceType(deviceType: DeviceType) {
    const select = await this.getMatHarnessByTestId(MatSelectHarness.with, 'device-type-select')
    await select.open();
    await select.clickOptions({text: deviceType});
  }

  async save() {
    userEvent.click(screen.getByTestId('save-simulated-device-btn'));
    this.detectChanges();
    await this.whenRenderingDone();
  }

  async cancel() {
    userEvent.click(screen.getByTestId('cancel-simulated-device-btn'));
  }

  async simulateAddSuccess() {
    this.actionsSubject.next(DeviceSimulatorActions.addSimulatedDevice.success());
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(AddSimulatedDeviceComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions
    });

    return new AddSimulatedDeviceHarness(result);
  }
}
