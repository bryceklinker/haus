import {screen} from "@testing-library/dom";
import {MatInputHarness} from "@angular/material/input/testing";
import {MatButtonHarness} from "@angular/material/button/testing";

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
    return await this.getInputValue(TEST_IDS.MIN_LEVEL);
  }

  async changeMinLevel(value: number) {
    await this.changeInput(value, TEST_IDS.MIN_LEVEL);
  }

  async isMinLevelDisabled() {
    return await this.isInputDisabled(TEST_IDS.MIN_LEVEL)
  }

  async maxLevelValue() {
    return await this.getInputValue(TEST_IDS.MAX_LEVEL);
  }

  async changeMaxLevel(value: number) {
    await this.changeInput(value, TEST_IDS.MAX_LEVEL);
  }

  async isMaxLevelDisabled() {
    return await this.isInputDisabled(TEST_IDS.MAX_LEVEL)
  }

  async minTemperatureValue() {
    return await this.getInputValue(TEST_IDS.MIN_TEMPERATURE);
  }

  async changeMinTemperature(value: number) {
    await this.changeInput(value, TEST_IDS.MIN_TEMPERATURE);
  }

  async isMinTemperatureDisabled() {
    return await this.isInputDisabled(TEST_IDS.MIN_TEMPERATURE)
  }

  async maxTemperatureValue() {
    return await this.getInputValue(TEST_IDS.MAX_TEMPERATURE);
  }

  async changeMaxTemperature(value: number) {
    await this.changeInput(value, TEST_IDS.MAX_TEMPERATURE);
  }

  async isMaxTemperatureDisabled() {
    return await this.isInputDisabled(TEST_IDS.MAX_TEMPERATURE)
  }

  async save() {
    await this.clickButton(TEST_IDS.SAVE);
  }

  async isSaveDisabled() {
    return await this.isButtonDisabled(TEST_IDS.SAVE);
  }

  async cancel() {
    await this.clickButton(TEST_IDS.CANCEL);
  }

  async isCancelDisabled() {
    return await this.isButtonDisabled(TEST_IDS.CANCEL);
  }

  private async getInputByTestId(testId: string) {
    return await this.getMatHarnessByTestId(MatInputHarness.with, testId);
  }

  private async getButtonByTestId(testId: string) {
    return await this.getMatHarnessByTestId(MatButtonHarness.with, testId);
  }

  private async changeInput(value: number, testId: string) {
    const input = await this.getInputByTestId(testId);
    await input.setValue(`${value}`);
  }

  private async isInputDisabled(testId: string) {
    const input = await this.getInputByTestId(testId);
    return await input.isDisabled();
  }

  private async getInputValue(testId: string) {
    const input = await this.getInputByTestId(testId);
    return await input.getValue();
  }

  private async clickButton(testId: string) {
    const button = await this.getButtonByTestId(testId);
    await button.click();
  }

  private async isButtonDisabled(testId: string) {
    const button = await this.getButtonByTestId(testId);
    return await button.isDisabled();
  }

  static async render(props: Partial<LightingConstraintsComponent>) {
    const result = await renderFeatureComponent(LightingConstraintsComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingConstraintsHarness(result);
  }
}
