import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingComponent} from "./lighting.component";
import {SharedModule} from "../../shared.module";
import {screen} from "@testing-library/dom";
import {LightingColorHarness} from "../lighting-color/lighting-color.harness";

const TEST_IDS = {
  LEVEL_INPUT: 'level-input',
  LEVEL_VALUE: 'level-value',
  STATE_INPUT: 'state-input',
  STATE_VALUE: 'state-value',
  TEMPERATURE_INPUT: 'temperature-input',
  TEMPERATURE_VALUE: 'temperature-value',
  LIGHTING_COLOR: 'lighting-color'
};

export class LightingHarness extends HausComponentHarness<LightingComponent> {
  private _colorHarness: LightingColorHarness;

  get exists() {
    return screen.queryAllByTestId('lighting').length > 0
  }

  get levelDisplay() {
    return screen.getByTestId(TEST_IDS.LEVEL_VALUE);
  }

  get stateDisplay() {
    return screen.getByTestId(TEST_IDS.STATE_VALUE);
  }

  get temperatureDisplay() {
    return screen.getByTestId(TEST_IDS.TEMPERATURE_VALUE);
  }

  get colorElement() {
    return screen.queryByTestId(TEST_IDS.LIGHTING_COLOR);
  }

  get temperatureElement() {
    return screen.queryByTestId(TEST_IDS.TEMPERATURE_INPUT);
  }

  constructor(result: RenderComponentResult<LightingComponent>) {
    super(result);

    this._colorHarness = LightingColorHarness.fromResult(result);
  }

  async isStateOn() {
    return await this.isSlideToggleCheckedByTestId(TEST_IDS.STATE_INPUT);
  }

  async isLevelDisabled() {
    return await this.isSliderDisabledByTestId(TEST_IDS.LEVEL_INPUT);
  }

  async isTemperatureDisabled() {
    return await this.isSliderDisabledByTestId(TEST_IDS.TEMPERATURE_INPUT);
  }

  async isStateDisabled() {
    return await this.isSlideToggleDisabledByTestId(TEST_IDS.STATE_INPUT)
  }

  async levelValue() {
    return await this.getSliderValueByTestId(TEST_IDS.LEVEL_INPUT);
  }

  async levelMin() {
    return await this.getSliderMinByTestId(TEST_IDS.LEVEL_INPUT);
  }

  async levelMax() {
    return await this.getSliderMaxByTestId(TEST_IDS.LEVEL_INPUT);
  }

  async temperatureValue() {
    return await this.getSliderValueByTestId(TEST_IDS.TEMPERATURE_INPUT);
  }

  async temperatureMin() {
    return await this.getSliderMinByTestId(TEST_IDS.TEMPERATURE_INPUT);
  }

  async temperatureMax() {
    return await this.getSliderMaxByTestId(TEST_IDS.TEMPERATURE_INPUT);
  }

  async turnLightingOn() {
    await this.checkSlideToggleByTestId(TEST_IDS.STATE_INPUT);
  }

  async changeLevel(level: number) {
    await this.changeSliderValueByTestId(level, TEST_IDS.LEVEL_INPUT);
  }

  async changeTemperature(temperature: number) {
    await this.changeSliderValueByTestId(temperature, TEST_IDS.TEMPERATURE_INPUT);
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
