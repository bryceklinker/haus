import {screen} from "@testing-library/dom";

import {DeviceDetailComponent} from "./device-detail.component";
import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {LightType} from "../../../shared/models";

export class DeviceDetailHarness extends HausComponentHarness<DeviceDetailComponent> {
  get nameField() {
    return screen.getByTestId('device-name-field');
  }

  get externalIdField() {
    return screen.getByTestId('device-external-id-field');
  }

  get deviceTypeField() {
    return screen.getByTestId('device-type-field');
  }

  get metadata() {
    return screen.queryAllByTestId('device-metadata');
  }

  get lighting() {
    return screen.queryByTestId('lighting');
  }

  get lightingConstraints() {
    return screen.queryByTestId('lighting-constraints');
  }

  get lightTypeSelect() {
    return screen.queryByTestId('light-type-select');
  }

  async saveConstraints() {
    await this.clickButtonByTestId('save-constraints-btn');
  }

  async saveDevice() {
    await this.clickButtonByTestId('save-device-btn');
  }

  async selectLightType(lightType: LightType) {
    await this.changeSelectedOptionByTestId(lightType, 'light-type-select');
  }

  async getLightTypesOptions() {
    return await this.getSelectOptionsByTestId('light-type-select');
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
