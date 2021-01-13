import {LightingComponent} from "./lighting.component";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {LightingModel, LightingState} from "../../models";
import {LightingHarness} from "./lighting.harness";

describe('LightingComponent', () => {
  it('should show lighting values when rendered', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.On,
      level: ModelFactory.createLevelLighting({value: 45, min: 5, max: 95}),
      temperature: ModelFactory.createTemperatureLighting({value: 77, min: 0, max: 1000}),
      color: ModelFactory.createColorLighting()
    });
    const harness = await LightingHarness.render({lighting});

    expect(await harness.isStateOn()).toEqual(true);
    expect(await harness.levelValue()).toEqual(45);
    expect(await harness.levelMin()).toEqual(5);
    expect(await harness.levelMax()).toEqual(95);
    expect(await harness.temperatureValue()).toEqual(77);
    expect(await harness.temperatureMin()).toEqual(0);
    expect(await harness.temperatureMax()).toEqual(1000);
  })

  it('should show lighting values', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.Off,
      level: ModelFactory.createLevelLighting({value: 45}),
      temperature: ModelFactory.createTemperatureLighting({value: 2700}),
      color: ModelFactory.createColorLighting()
    });

    const harness = await LightingHarness.render({lighting});

    expect(harness.stateDisplay).toHaveTextContent('Off');
    expect(harness.levelDisplay).toHaveTextContent('45');
    expect(harness.temperatureDisplay).toHaveTextContent('2700');
    expect(harness.colorElement).toBeInTheDocument();
  })

  it('should render show lighting not available when lighting is not provided', async () => {
    const harness = await LightingHarness.render();

    expect(harness.container).toHaveTextContent('No lighting available');
  })

  it('should notify of change when brightness changes', async () => {
    const lighting = ModelFactory.createLighting({
      level: ModelFactory.createLevelLighting(),
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const harness = await LightingHarness.render({lighting, change: changeEmitter});

    await harness.changeLevel(78);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      level: {...lighting.level, value: 78}
    })
  })

  it('should notify of change when temperature changes', async () => {
    const lighting = ModelFactory.createLighting({temperature: ModelFactory.createTemperatureLighting({value: 4500})});
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const harness = await LightingHarness.render({lighting, change: changeEmitter});

    await harness.changeTemperature(6000);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      temperature: {...lighting.temperature, value: 6000}
    })
  })

  it('should notify of change when color changes', async () => {
    const lighting = ModelFactory.createLighting({
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting({red: 9})
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();

    const harness = await LightingHarness.render({lighting, change: changeEmitter});
    await harness.changeRed(200);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      color: {
        ...lighting.color,
        red: 200
      }
    })
  })

  it('should notify of change when state changes', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.Off,
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();

    const harness = await LightingHarness.render({lighting, change: changeEmitter});
    await harness.turnLightingOn();

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      state: LightingState.On
    })
  })

  it('should disable controls when lighting is readonly', async () => {
    const lighting = ModelFactory.createLighting({
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });

    const harness = await LightingHarness.render({lighting, readonly: true});

    expect(await harness.isLevelDisabled()).toEqual(true);
    expect(await harness.isStateDisabled()).toEqual(true);
    expect(await harness.isTemperatureDisabled()).toEqual(true);
  })

  it('should hide color when lighting does not have color', async () => {
    const lighting = ModelFactory.createLighting();

    const harness = await LightingHarness.render({lighting});

    expect(harness.colorElement).not.toBeInTheDocument();
  })

  it('should hide temperature when lighting does not have temperature', async () => {
    const lighting = ModelFactory.createLighting();

    const harness = await LightingHarness.render({lighting});

    expect(harness.temperatureElement).not.toBeInTheDocument();
  })
})
