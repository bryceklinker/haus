import {LightingColorComponent} from "./lighting-color.component";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {ColorLightingModel} from "../../models";
import {LightingColorHarness} from "./lighting-color.harness";

describe('LightingColorComponent', () => {
  test('should show color values when rendered', async () => {
    const color = ModelFactory.createColorLighting({red: 7, green: 9, blue: 19});

    const harness = await LightingColorHarness.render({color});

    expect(await harness.redValue()).toEqual(color.red);
    expect(await harness.greenValue()).toEqual(color.green);
    expect(await harness.blueValue()).toEqual(color.blue);
  })

  test('should show color values when rendered', async () => {
    const color = ModelFactory.createColorLighting({red: 6, green: 9, blue: 8});

    const harness = await LightingColorHarness.render({color});

    expect(harness.rgbValue).toHaveTextContent('6');
    expect(harness.rgbValue).toHaveTextContent('9');
    expect(harness.rgbValue).toHaveTextContent('8');
    expect(harness.hexValue).toHaveTextContent('#060908');
  })

  test('should notify of color change when red changes', async () => {
    const color = ModelFactory.createColorLighting();
    const emitter = new TestingEventEmitter<ColorLightingModel>();

    const harness = await LightingColorHarness.render({color, colorChange: emitter});
    await harness.changeRed(55);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      red: 55
    })
  })

  test('should notify of color change when green changes', async () => {
    const color = ModelFactory.createColorLighting();
    const emitter = new TestingEventEmitter<ColorLightingModel>();

    const harness = await LightingColorHarness.render({color, colorChange: emitter});
    await harness.changeGreen(77);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      green: 77
    })
  })

  test('should notify of color change when blue changes', async () => {
    const color = ModelFactory.createColorLighting();
    const emitter = new TestingEventEmitter<ColorLightingModel>();

    const harness = await LightingColorHarness.render({color, colorChange: emitter});
    await harness.changeBlue(99);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      blue: 99
    })
  })

  test('should disable controls when readonly', async () => {
    const color = ModelFactory.createColorLighting();

    const harness = await LightingColorHarness.render({color, readonly: true});

    expect(await harness.isRedDisabled()).toEqual(true);
    expect(await harness.isGreenDisabled()).toEqual(true);
    expect(await harness.isBlueDisabled()).toEqual(true);
  })
})
