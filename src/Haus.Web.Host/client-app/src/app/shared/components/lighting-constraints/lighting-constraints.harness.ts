import {screen} from '@testing-library/dom';

import {HausComponentHarness, renderFeatureComponent} from '../../../../testing';
import {LightingConstraintsComponent} from './lighting-constraints.component';
import {SharedModule} from '../../shared.module';

const LABELS = {
  MIN_LEVEL: 'min level',
  MAX_LEVEL: 'max level',
  MIN_TEMPERATURE: 'min temperature',
  MAX_TEMPERATURE: 'max temperature',
  SAVE: 'save constraints',
  CANCEL: 'cancel constraints'
};

export class LightingConstraintsHarness extends HausComponentHarness<LightingConstraintsComponent> {
  get minTemperatureElement() {
    return screen.queryByRole('textbox', {name: LABELS.MIN_TEMPERATURE});
  }

  get maxTemperatureElement() {
    return screen.queryByRole('textbox', {name: LABELS.MAX_TEMPERATURE});
  }

  get invalid() {
    return this.fixture.componentInstance.constraintsForm.invalid;
  }

  get cancelElement() {
    return screen.queryByRole('button', {name: LABELS.CANCEL});
  }

  async minLevelValue() {
    return await this.getInputValueByLabel(LABELS.MIN_LEVEL);
  }

  async changeMinLevel(value: number) {
    await this.changeInputByLabel(`${value}`, LABELS.MIN_LEVEL);
  }

  async isMinLevelDisabled() {
    return await this.isInputDisabledByLabel(LABELS.MIN_LEVEL);
  }

  async maxLevelValue() {
    return await this.getInputValueByLabel(LABELS.MAX_LEVEL);
  }

  async changeMaxLevel(value: number) {
    await this.changeInputByLabel(`${value}`, LABELS.MAX_LEVEL);
  }

  async isMaxLevelDisabled() {
    return await this.isInputDisabledByLabel(LABELS.MAX_LEVEL);
  }

  async minTemperatureValue() {
    return await this.getInputValueByLabel(LABELS.MIN_TEMPERATURE);
  }

  async changeMinTemperature(value: number) {
    await this.changeInputByLabel(`${value}`, LABELS.MIN_TEMPERATURE);
  }

  async isMinTemperatureDisabled() {
    return await this.isInputDisabledByLabel(LABELS.MIN_TEMPERATURE);
  }

  async maxTemperatureValue() {
    return await this.getInputValueByLabel(LABELS.MAX_TEMPERATURE);
  }

  async changeMaxTemperature(value: number) {
    await this.changeInputByLabel(`${value}`, LABELS.MAX_TEMPERATURE);
  }

  async isMaxTemperatureDisabled() {
    return await this.isInputDisabledByLabel(LABELS.MAX_TEMPERATURE);
  }

  save() {
    this.clickButtonByLabel(LABELS.SAVE);
  }

  isSaveDisabled() {
    return this.isButtonDisabledByLabel(LABELS.SAVE);
  }

  cancel() {
    this.clickButtonByLabel(LABELS.CANCEL);
  }

  isCancelDisabled() {
    return this.isButtonDisabledByLabel(LABELS.CANCEL);
  }

  static async render(props: Partial<LightingConstraintsComponent>) {
    const result = await renderFeatureComponent(LightingConstraintsComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingConstraintsHarness(result);
  }
}
