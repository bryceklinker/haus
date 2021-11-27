import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {LightingColorComponent} from "./lighting-color.component";
import {SharedModule} from "../../shared.module";
import {screen} from "@testing-library/dom";

const TEST_IDS = {
  RGB_VALUE: 'rgb value',
  HEX_VALUE: 'hex value',
  RED: 'red',
  GREEN: 'green',
  BLUE: 'blue'
}

export class LightingColorHarness extends HausComponentHarness<LightingColorComponent> {
  get rgbValue() {
    return screen.getByLabelText(TEST_IDS.RGB_VALUE);
  }

  get hexValue() {
    return screen.getByLabelText(TEST_IDS.HEX_VALUE);
  }

  async redValue() {
    return await this.getSliderValueByLabel(TEST_IDS.RED);
  }

  async isRedDisabled() {
    return await this.isSliderDisabledByLabel(TEST_IDS.RED);
  }

  async greenValue() {
    return await this.getSliderValueByLabel(TEST_IDS.GREEN);
  }

  async isGreenDisabled() {
    return await this.isSliderDisabledByLabel(TEST_IDS.GREEN);
  }

  async blueValue() {
    return await this.getSliderValueByLabel(TEST_IDS.BLUE);
  }

  async isBlueDisabled() {
    return await this.isSliderDisabledByLabel(TEST_IDS.BLUE);
  }

  async changeRed(red: number) {
    await this.changeSliderValueByLabel(red, TEST_IDS.RED);
  }

  async changeGreen(green: number) {
    await this.changeSliderValueByLabel(green, TEST_IDS.GREEN);
  }

  async changeBlue(blue: number) {
    await this.changeSliderValueByLabel(blue, TEST_IDS.BLUE);
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
