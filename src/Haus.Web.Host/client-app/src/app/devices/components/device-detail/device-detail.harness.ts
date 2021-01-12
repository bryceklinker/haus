import {screen} from "@testing-library/dom";
import {MatButtonHarness} from "@angular/material/button/testing";
import {MatSelectHarness} from "@angular/material/select/testing";

import {DeviceDetailComponent} from "./device-detail.component";
import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
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
    const button = await this.getMatHarnessByTestId(MatButtonHarness.with, 'save-constraints-btn');
    await button.click();
  }

  async saveDevice() {
    const button = await this.getMatHarnessByTestId(MatButtonHarness.with, 'save-device-btn');
    await button.click();
  }

  async selectLightType(lightType: LightType) {
    const select = await this.getLightTypeSelect();
    await select.open();
    await select.clickOptions({text: lightType});
  }

  async getLightTypesOptions() {
    const select = await this.getLightTypeSelect();
    await select.open();
    return await select.getOptions();
  }

  private async getLightTypeSelect() {
    return await this.getMatHarnessByTestId(MatSelectHarness.with, 'light-type-select');
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
