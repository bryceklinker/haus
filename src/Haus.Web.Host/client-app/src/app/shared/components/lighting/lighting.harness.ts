import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingComponent} from "./lighting.component";
import {SharedModule} from "../../shared.module";
import {screen} from "@testing-library/dom";
import {LightingColorHarness} from "../lighting-color/lighting-color.harness";

const TEST_IDS = {
  LEVEL_INPUT: 'level',
  LEVEL_VALUE: 'level value',
  STATE_INPUT: 'state',
  STATE_VALUE: 'state value',
  TEMPERATURE_INPUT: 'temperature',
  TEMPERATURE_VALUE: 'temperature value',
  LIGHTING_COLOR: 'lighting color'
};

export class LightingHarness extends HausComponentHarness<LightingComponent> {
  private _colorHarness: LightingColorHarness;

  get exists() {
    return screen.queryAllByLabelText('lighting').length > 0
  }

  get levelDisplay() {
    return screen.getByLabelText(TEST_IDS.LEVEL_VALUE);
  }

  get stateDisplay() {
    return screen.getByLabelText(TEST_IDS.STATE_VALUE);
  }

  get temperatureDisplay() {
    return screen.getByLabelText(TEST_IDS.TEMPERATURE_VALUE);
  }

  get colorElement() {
    return screen.queryByLabelText(TEST_IDS.LIGHTING_COLOR);
  }

  get temperatureElement() {
    return screen.queryByRole('textbox', {name: TEST_IDS.TEMPERATURE_INPUT});
  }

  constructor(result: RenderComponentResult<LightingComponent>) {
    super(result);

    this._colorHarness = LightingColorHarness.fromResult(result);
  }

  isStateOn() {
    return this.isSlideToggleCheckedByLabel(TEST_IDS.STATE_INPUT);
  }

  async isLevelDisabled() {
    return await this.isSliderDisabledByLabel(TEST_IDS.LEVEL_INPUT);
  }

  async isTemperatureDisabled() {
    return await this.isSliderDisabledByLabel(TEST_IDS.TEMPERATURE_INPUT);
  }

  isStateDisabled() {
    return this.isSlideToggleDisabledByLabel(TEST_IDS.STATE_INPUT)
  }

  async levelValue() {
    return await this.getSliderValueByLabel(TEST_IDS.LEVEL_INPUT);
  }

  async levelMin() {
    return await this.getSliderMinByLabel(TEST_IDS.LEVEL_INPUT);
  }

  async levelMax() {
    return await this.getSliderMaxByLabel(TEST_IDS.LEVEL_INPUT);
  }

  async temperatureValue() {
    return await this.getSliderValueByLabel(TEST_IDS.TEMPERATURE_INPUT);
  }

  async temperatureMin() {
    return await this.getSliderMinByLabel(TEST_IDS.TEMPERATURE_INPUT);
  }

  async temperatureMax() {
    return await this.getSliderMaxByLabel(TEST_IDS.TEMPERATURE_INPUT);
  }

  async turnLightingOn() {
    await this.toggleSlideByLabel(TEST_IDS.STATE_INPUT);
  }

  async changeLevel(level: number) {
    await this.changeSliderValueByLabel(level, TEST_IDS.LEVEL_INPUT);
  }

  async changeTemperature(temperature: number) {
    await this.changeSliderValueByLabel(temperature, TEST_IDS.TEMPERATURE_INPUT);
  }

  async changeRed(red: number) {
    await this._colorHarness.changeRed(red);
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LightingHarness(result);
  }

  static async render(props: Partial<LightingComponent> = {}) {
    const result = await renderFeatureComponent(LightingComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingHarness(result);
  }
}
