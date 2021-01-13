import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingComponent} from "./lighting.component";
import {SharedModule} from "../../shared.module";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {screen} from "@testing-library/dom";
import {LightingColorHarness} from "../lighting-color/lighting-color.harness";

export class LightingHarness extends HausComponentHarness<LightingComponent> {
  private _colorHarness: LightingColorHarness;
  get levelDisplay() {
    return screen.getByTestId('level-value');
  }

  get stateDisplay() {
    return screen.getByTestId('state-value');
  }

  get temperatureDisplay() {
    return screen.getByTestId('temperature-value');
  }

  get colorElement() {
    return screen.getByTestId('lighting-color');
  }

  get temperatureElement() {
    return screen.queryByTestId('temperature-input');
  }

  constructor(result: RenderComponentResult<LightingComponent>) {
    super(result);

    this._colorHarness = LightingColorHarness.fromResult(result);
  }

  async isStateOn() {
    const state = await this.getStateSlide();
    return await state.isChecked();
  }

  async isLevelDisabled() {
    const level = await this.getLevelSlider();
    return await level.isDisabled();
  }

  async isTemperatureDisabled() {
    const temperature = await this.getTemperatureSlider();
    return await temperature.isDisabled();
  }

  async isStateDisabled() {
    const state = await this.getStateSlide();
    return await state.isDisabled();
  }

  async levelValue() {
    const level = await this.getLevelSlider();
    return await level.getValue();
  }

  async levelMin() {
    const level = await this.getLevelSlider();
    return await level.getMinValue();
  }

  async levelMax() {
    const level = await this.getLevelSlider();
    return await level.getMaxValue();
  }

  async temperatureValue() {
    const temperature = await this.getTemperatureSlider();
    return await temperature.getValue();
  }

  async temperatureMin() {
    const temperature = await this.getTemperatureSlider();
    return await temperature.getMinValue();
  }

  async temperatureMax() {
    const temperature = await this.getTemperatureSlider();
    return await temperature.getMaxValue();
  }

  async turnLightingOn() {
    const state = await this.getStateSlide();
    await state.check();
  }

  async changeLevel(level: number) {
    const slider = await this.getLevelSlider();
    await slider.setValue(level);
  }

  async changeTemperature(temperature: number) {
    const slider = await this.getTemperatureSlider();
    await slider.setValue(temperature);
  }

  async changeRed(red: number) {
    await this._colorHarness.changeRed(red);
  }

  private async getLevelSlider() {
    return await this.getMatHarnessByTestId(MatSliderHarness.with, 'level-input')
  }

  private async getTemperatureSlider() {
    return await this.getMatHarnessByTestId(MatSliderHarness.with, 'temperature-input')
  }

  private async getStateSlide() {
    return await this.getMatHarnessByTestId(MatSlideToggleHarness.with, 'state-input');
  }

  static async render(props: Partial<LightingComponent> = {}) {
    const result = await renderFeatureComponent(LightingComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingHarness(result);
  }
}
