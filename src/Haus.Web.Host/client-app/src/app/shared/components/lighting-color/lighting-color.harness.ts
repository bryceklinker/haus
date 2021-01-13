import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingColorComponent} from "./lighting-color.component";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {SharedModule} from "../../shared.module";
import {screen} from "@testing-library/dom";
import {findAttributeOnElementWithAttrs} from "@angular/cdk/schematics";

const TEST_IDS = {
  RGB_VALUE: 'rgb-value',
  HEX_VALUE: 'hex-value',
  RED: 'red-input',
  GREEN: 'green-input',
  BLUE: 'blue-input'
}

export class LightingColorHarness extends HausComponentHarness<LightingColorComponent> {
  get rgbValue() {
    return screen.getByTestId(TEST_IDS.RGB_VALUE);
  }

  get hexValue() {
    return screen.getByTestId(TEST_IDS.HEX_VALUE);
  }

  async redValue() {
    return await this.getSliderValueByTestId(TEST_IDS.RED);
  }

  async isRedDisabled() {
    return await this.isSliderDisabledByTestId(TEST_IDS.RED);
  }

  async greenValue() {
    return await this.getSliderValueByTestId(TEST_IDS.GREEN);
  }

  async isGreenDisabled() {
    return await this.isSliderDisabledByTestId(TEST_IDS.GREEN);
  }

  async blueValue() {
    return await this.getSliderValueByTestId(TEST_IDS.BLUE);
  }

  async isBlueDisabled() {
    return await this.isSliderDisabledByTestId(TEST_IDS.BLUE);
  }

  async changeRed(red: number) {
    await this.changeSliderValueByTestId(red, TEST_IDS.RED);
  }

  async changeGreen(green: number) {
    await this.changeSliderValueByTestId(green, TEST_IDS.GREEN);
  }

  async changeBlue(blue: number) {
    await this.changeSliderValueByTestId(blue, TEST_IDS.BLUE);
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
