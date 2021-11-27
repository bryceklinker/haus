import {screen} from "@testing-library/dom";

import {DeviceDetailComponent} from "./device-detail.component";
import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {LightType} from "../../../shared/models";

export class DeviceDetailHarness extends HausComponentHarness<DeviceDetailComponent> {
  get nameField() {
    return screen.getByRole('textbox', {name: 'device name'});
  }

  get externalIdField() {
    return screen.getByRole('textbox', {name: 'device external id'});
  }

  get deviceTypeField() {
    return screen.getByRole('textbox', {name: 'device type'});
  }

  get metadata() {
    return screen.queryAllByLabelText('device metadata');
  }

  get lighting() {
    return screen.queryByLabelText('lighting');
  }

  get lightingConstraints() {
    return screen.queryByLabelText('lighting constraints');
  }

  get lightTypeSelect() {
    return screen.queryByRole('select', {name: 'select light type'});
  }

  async saveConstraints() {
    await this.clickButtonByLabel('save constraints');
  }

  async saveDevice() {
    await this.clickButtonByLabel('save device');
  }

  async selectLightType(lightType: LightType) {
    await this.changeSelectedOptionByLabel(lightType, 'select light type');
  }

  async getLightTypesOptions() {
    return await this.getSelectOptionsByLabel('select light type');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DeviceDetailHarness(result);
  }

  static async render(props: Partial<DeviceDetailComponent> = {}) {
    const result = await renderFeatureComponent(DeviceDetailComponent, {
      imports: [DevicesModule],
      componentProperties: props
    });
    result.detectChanges();
    await result.fixture.whenRenderingDone();

    return new DeviceDetailHarness(result);
  }
}
