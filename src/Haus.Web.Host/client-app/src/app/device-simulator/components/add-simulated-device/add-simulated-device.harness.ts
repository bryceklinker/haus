import {screen} from '@testing-library/dom';
import {Action} from '@ngrx/store';

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from '../../../../testing';
import {AddSimulatedDeviceComponent} from './add-simulated-device.component';
import {DeviceSimulatorModule} from '../../device-simulator.module';
import {DeviceSimulatorActions} from '../../state';
import {DeviceType} from '../../../shared/models';
import {DeviceSimulatorDashboardComponent} from '../device-simulator-dashboard/device-simulator-dashboard.component';

export class AddSimulatedDeviceHarness extends HausComponentHarness<AddSimulatedDeviceComponent> {

  get saveButton() {
    return screen.getByRole('button', {name: 'save simulated device'});
  }

  constructor(result: RenderComponentResult<AddSimulatedDeviceComponent>) {
    super(result);

    jest.spyOn(this.router, 'navigateByUrl');
  }

  async enterMetadataKey(key: string) {
    await this.changeInputByLabel('something', 'metadata key');
  }

  async enterMetadataValue(value: string) {
    await this.changeInputByLabel('else', 'metadata value');
  }

  async addMetadata() {
    await this.clickButtonByLabel('add metadata');
  }

  async selectDeviceType(deviceType: DeviceType) {
    await this.changeSelectedOptionByLabel(deviceType, 'select device type');
  }

  async save() {
    await this.clickButtonByLabel('save simulated device');
  }

  async cancel() {
    await this.clickButtonByLabel('cancel simulated device');
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
