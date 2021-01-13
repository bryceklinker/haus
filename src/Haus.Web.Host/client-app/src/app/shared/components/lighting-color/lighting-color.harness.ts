import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingColorComponent} from "./lighting-color.component";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {SharedModule} from "../../shared.module";
import {screen} from "@testing-library/dom";
import {findAttributeOnElementWithAttrs} from "@angular/cdk/schematics";

export class LightingColorHarness extends HausComponentHarness<LightingColorComponent> {
  get rgbValue() {
    return screen.getByTestId('rgb-value');
  }

  get hexValue() {
    return screen.getByTestId('hex-value');
  }

  async redValue() {
    const red = await this.getRedSlider();
    return await red.getValue();
  }

  async isRedDisabled() {
    const red = await this.getRedSlider();
    return await red.isDisabled();
  }

  async greenValue() {
    const green = await this.getGreenSlider();
    return await green.getValue();
  }

  async isGreenDisabled() {
    const green = await this.getGreenSlider();
    return await green.isDisabled();
  }

  async blueValue() {
    const blue = await this.getBlueSlider();
    return await blue.getValue();
  }

  async isBlueDisabled() {
    const blue = await this.getBlueSlider();
    return await blue.isDisabled();
  }

  async changeRed(red: number) {
    const slider = await this.getRedSlider();
    await slider.setValue(red);
  }

  async changeGreen(green: number) {
    const slider = await this.getGreenSlider();
    await slider.setValue(green);
  }

  async changeBlue(blue: number) {
    const slider = await this.getBlueSlider();
    await slider.setValue(blue);
  }

  private async getRedSlider() {
    return await this.getMatHarnessByTestId(MatSliderHarness.with, 'red-input');
  }

  private async getGreenSlider() {
    return await this.getMatHarnessByTestId(MatSliderHarness.with, 'green-input');
  }

  private async getBlueSlider() {
    return await this.getMatHarnessByTestId(MatSliderHarness.with, 'blue-input');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new LightingColorHarness(result);
  }

  static async render(props: Partial<LightingColorComponent>) {
    const result = await renderFeatureComponent(LightingColorComponent, {
      imports: [SharedModule],
      componentProperties: props
    });

    return new LightingColorHarness(result);
  }
}
