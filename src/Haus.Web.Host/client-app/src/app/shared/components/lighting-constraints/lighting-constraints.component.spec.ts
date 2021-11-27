import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {LightingConstraintsComponent} from "./lighting-constraints.component";
import {LightingConstraintsModel} from "../../models";
import {LightingConstraintsHarness} from "./lighting-constraints.harness";

describe('LightingConstraintsComponent', () => {
  it('should show level values', async () => {
    const level = ModelFactory.createLevelLighting({min: 25, max: 90});

    const harness = await LightingConstraintsHarness.render({level});

    expect(await harness.minLevelValue()).toEqual('25');
    expect(await harness.maxLevelValue()).toEqual('90');
  })

  it('should show temperature when temperature is provided', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting({min: 0, max: 255});

    const harness = await LightingConstraintsHarness.render({level, temperature});

    expect(await harness.minTemperatureValue()).toEqual('0');
    expect(await harness.maxTemperatureValue()).toEqual('255');
  })

  it('should hide temperature controls when temperature is missing', async () => {
    const level = ModelFactory.createLevelLighting();

    const harness = await LightingConstraintsHarness.render({level});

    expect(harness.minTemperatureElement).not.toBeInTheDocument();
    expect(harness.maxTemperatureElement).not.toBeInTheDocument();
  })

  it('should notify when saved', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting();
    const emitter = new TestingEventEmitter<LightingConstraintsModel>();

    const harness = await LightingConstraintsHarness.render({level, temperature, save: emitter});
    await harness.changeMinLevel(54);
    await harness.changeMaxLevel(65);
    await harness.changeMinTemperature(3000)
    await harness.changeMaxTemperature(5000)
    harness.save();

    expect(emitter.emit).toHaveBeenCalledWith({
      minLevel: 54,
      maxLevel: 65,
      minTemperature: 3000,
      maxTemperature: 5000
    });
  })

  it('should only notify with level changes when saved', async () => {
    const level = ModelFactory.createLevelLighting();
    const emitter = new TestingEventEmitter<LightingConstraintsModel>();

    const harness = await LightingConstraintsHarness.render({level, save: emitter});
    await harness.changeMinLevel(54);
    await harness.changeMaxLevel(65);
    harness.save();

    expect(emitter.emit).toHaveBeenCalledWith({
      minLevel: 54,
      maxLevel: 65
    })
  })

  it('should notify when cancelled', async () => {
    const level = ModelFactory.createLevelLighting();
    const emitter = new TestingEventEmitter();

    const harness = await LightingConstraintsHarness.render({level, cancel: emitter});
    await harness.cancel();

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable save when constraints are invalid', async () => {
    const level = ModelFactory.createLevelLighting();

    const harness = await LightingConstraintsHarness.render({level});
    await harness.changeMinLevel(1000);

    expect(harness.isSaveDisabled()).toEqual(true);
  })

  it('should be invalid when min level is greater or equal to max level', async () => {
    const level = ModelFactory.createLevelLighting();

    const harness = await LightingConstraintsHarness.render({level});
    await harness.changeMaxLevel(30);
    await harness.changeMinLevel(40);

    expect(harness.invalid).toEqual(true);
  })

  it('should be invalid when max level is less than or equal to min level', async () => {
    const level = ModelFactory.createLevelLighting();

    const harness = await LightingConstraintsHarness.render({level});
    await harness.changeMinLevel(90);
    await harness.changeMaxLevel(45);

    expect(harness.invalid).toEqual(true);
  })

  it('should be invalid when min temperature is greater than or equal to max temperature', async () => {
    const temperature = ModelFactory.createTemperatureLighting();

    const harness = await LightingConstraintsHarness.render({temperature});
    await harness.changeMaxTemperature(45);
    await harness.changeMinTemperature(90);

    expect(harness.invalid).toEqual(true);
  })

  it('should be invalid when max temperature is less than or equal to min temperature', async () => {
    const temperature = ModelFactory.createTemperatureLighting();

    const harness = await LightingConstraintsHarness.render({temperature});
    await harness.changeMinTemperature(90);
    await harness.changeMaxTemperature(45);

    expect(harness.invalid).toEqual(true);
  })

  it('should disable all controls when readonly', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting();

    const harness = await LightingConstraintsHarness.render({temperature, level, readonly: true});

    expect(await harness.isMinLevelDisabled()).toEqual(true);
    expect(await harness.isMaxLevelDisabled()).toEqual(true);
    expect(await harness.isMinTemperatureDisabled()).toEqual(true);
    expect(await harness.isMaxTemperatureDisabled()).toEqual(true);
    expect(harness.isSaveDisabled()).toEqual(true);
    expect(harness.isCancelDisabled()).toEqual(true);
  })

  it('should hide cancel when cancel is hidden', async () => {
    const level = ModelFactory.createLevelLighting();

    const harness = await LightingConstraintsHarness.render({level, hideCancel: true});

    expect(harness.cancelElement).not.toBeInTheDocument();
  })
})
