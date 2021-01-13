import {screen} from "@testing-library/dom";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {LightingConstraintsComponent} from "./lighting-constraints.component";
import {SharedModule} from "../../shared.module";

const TEST_IDS = {
  MIN_LEVEL: 'min-level-input',
  MAX_LEVEL: 'max-level-input',
  MIN_TEMPERATURE: 'min-temperature-input',
  MAX_TEMPERATURE: 'max-temperature-input',
  SAVE: 'save-constraints-btn',
  CANCEL: 'cancel-constraints-btn'
}

export class LightingConstraintsHarness extends HausComponentHarness<LightingConstraintsComponent> {
  get minTemperatureElement() {
    return screen.queryByTestId(TEST_IDS.MIN_TEMPERATURE);
  }

  get maxTemperatureElement() {
    return screen.queryByTestId(TEST_IDS.MAX_TEMPERATURE);
  }

  get invalid() {
    return this.fixture.componentInstance.constraintsForm.invalid;
  }

  get cancelElement() {
    return screen.queryByTestId(TEST_IDS.CANCEL);
  }

  async minLevelValue() {
    return await this.getInputValueByTestId(TEST_IDS.MIN_LEVEL);
  }

  async changeMinLevel(value: number) {
    await this.changeInputByTestId(`${value}`, TEST_IDS.MIN_LEVEL);
  }

  async isMinLevelDisabled() {
    return await this.isInputDisabledByTestId(TEST_IDS.MIN_LEVEL)
  }

  async maxLevelValue() {
    return await this.getInputValueByTestId(TEST_IDS.MAX_LEVEL);
  }

  async changeMaxLevel(value: number) {
    await this.changeInputByTestId(`${value}`, TEST_IDS.MAX_LEVEL);
  }

  async isMaxLevelDisabled() {
    return await this.isInputDisabledByTestId(TEST_IDS.MAX_LEVEL)
  }

  async minTemperatureValue() {
    return await this.getInputValueByTestId(TEST_IDS.MIN_TEMPERATURE);
  }

  async changeMinTemperature(value: number) {
    await this.changeInputByTestId(`${value}`, TEST_IDS.MIN_TEMPERATURE);
  }

  async isMinTemperatureDisabled() {
    return await this.isInputDisabledByTestId(TEST_IDS.MIN_TEMPERATURE)
  }

  async maxTemperatureValue() {
    return await this.getInputValueByTestId(TEST_IDS.MAX_TEMPERATURE);
  }

  async changeMaxTemperature(value: number) {
    await this.changeInputByTestId(`${value}`, TEST_IDS.MAX_TEMPERATURE);
  }

  async isMaxTemperatureDisabled() {
    return await this.isInputDisabledByTestId(TEST_IDS.MAX_TEMPERATURE)
  }

  async save() {
    await this.clickButtonByTestId(TEST_IDS.SAVE);
  }

  async isSaveDisabled() {
    return await this.isButtonDisabledByTestId(TEST_IDS.SAVE);
  }

  async cancel() {
    await this.clickButtonByTestId(TEST_IDS.CANCEL);
  }

  async isCancelDisabled() {
    return await this.isButtonDisabledByTestId(TEST_IDS.CANCEL);
  }

  static async render(props: Partial<LightingConstraintsComponent>) {
    const result = await renderFeatureComponent(LightingConstraintsComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingConstraintsHarness(result);
  }
}
