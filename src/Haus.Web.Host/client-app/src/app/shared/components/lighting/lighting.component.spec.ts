import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {HarnessLoader} from "@angular/cdk/testing";
import {By} from "@angular/platform-browser";
import {ComponentFixture} from "@angular/core/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";

import {LightingComponent} from "./lighting.component";
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {LightingModel, LightingState} from "../../models";
import {screen} from "@testing-library/dom";

describe('LightingComponent', () => {
  it('should show lighting values when rendered', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.On,
      constraints: ModelFactory.createLightingConstraints({minLevel: 5, maxLevel: 95, minTemperature: 1000, maxTemperature: 6000})
    });
    const {level, state, temperature, red, green, blue} = await renderLightingWithHarnesses({lighting});

    expect(await level.getValue()).toEqual(lighting.level);
    expect(await level.getMinValue()).toEqual(5);
    expect(await level.getMaxValue()).toEqual(95);
    expect(await state.isChecked()).toEqual(true);
    expect(await temperature.getValue()).toEqual(lighting.temperature);
    expect(await temperature.getMinValue()).toEqual(1000);
    expect(await temperature.getMaxValue()).toEqual(6000);
    expect(await red.getValue()).toEqual(lighting.color.red);
    expect(await green.getValue()).toEqual(lighting.color.green);
    expect(await blue.getValue()).toEqual(lighting.color.blue);
  })

  it('should show lighting values', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.Off,
      level: 45,
      temperature: 2700,
      color: ModelFactory.createLightingColor({red: 6, green: 9, blue: 8})
    });

    await renderLightingWithHarnesses({lighting});

    expect(screen.getByTestId('state-value')).toHaveTextContent('Off');
    expect(screen.getByTestId('level-value')).toHaveTextContent('45');
    expect(screen.getByTestId('temperature-value')).toHaveTextContent('2700');
    expect(screen.getByTestId('rgb-value')).toHaveTextContent('6');
    expect(screen.getByTestId('rgb-value')).toHaveTextContent('9');
    expect(screen.getByTestId('rgb-value')).toHaveTextContent('8');
    expect(screen.getByTestId('hex-value')).toHaveTextContent('#060908');
  })

  it('should render show lighting not available when lighting is not provided', async () => {
    const {container} = await renderLighting();

    expect(container).toHaveTextContent('No lighting available');
  })

  it('should notify of change when brightness changes', async () => {
    const lighting = ModelFactory.createLighting({level: 0});
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'level-input', 78);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      level: 78
    })
  })

  it('should notify of change when temperature changes', async () => {
    const lighting = ModelFactory.createLighting({temperature: 4500});
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'temperature-input', 6000);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      temperature: 6000
    })
  })

  it('should notify of change when red changes', async () => {
    const lighting = ModelFactory.createLighting({
      color: ModelFactory.createLightingColor({red: 9})
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'red-input', 200);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      color: {
        ...lighting.color,
        red: 200
      }
    })
  })

  it('should notify of change when green changes', async () => {
    const lighting = ModelFactory.createLighting({
      color: ModelFactory.createLightingColor({green: 98})
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'green-input', 67);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      color: {
        ...lighting.color,
        green: 67
      }
    })
  })

  it('should notify of change when blue changes', async () => {
    const lighting = ModelFactory.createLighting({
      color: ModelFactory.createLightingColor({blue: 123})
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'blue-input', 98);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      color: {
        ...lighting.color,
        blue: 98
      }
    })
  })

  it('should notify of change when state changes', async () => {
    const lighting = ModelFactory.createLighting({state: LightingState.Off});
    const changeEmitter = new TestingEventEmitter<LightingModel>();

    const {state, detectChanges} = await renderLightingWithHarnesses({lighting, change: changeEmitter});
    await state.check();
    detectChanges();

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      state: LightingState.On
    })
  })

  it('should disable controls when lighting is readonly', async () => {
    const lighting = ModelFactory.createLighting();

    const {level, state, temperature, red, green, blue} = await renderLightingWithHarnesses({lighting, readonly: true});

    expect(await level.isDisabled()).toEqual(true);
    expect(await state.isDisabled()).toEqual(true);
    expect(await temperature.isDisabled()).toEqual(true);
    expect(await red.isDisabled()).toEqual(true);
    expect(await green.isDisabled()).toEqual(true);
    expect(await blue.isDisabled()).toEqual(true);
  })

  function renderLighting(props: Partial<LightingComponent> = {}) {
    return renderFeatureComponent(LightingComponent, {
      imports: [SharedModule],
      componentProperties: props
    });
  }

  async function renderLightingWithHarnesses(props: Partial<LightingComponent> = {}) {
    const result = await renderLighting(props);
    return {
      ...result,
      level: await getSliderHarness(result.matHarness, 'level-input'),
      state: await result.matHarness.getHarness(MatSlideToggleHarness.with({selector: '[data-testid="state-input"]'})),
      temperature: await getSliderHarness(result.matHarness, 'temperature-input'),
      red: await getSliderHarness(result.matHarness, 'red-input'),
      green: await getSliderHarness(result.matHarness, 'green-input'),
      blue: await getSliderHarness(result.matHarness, 'blue-input')
    }
  }

  function getSliderHarness(matHarness: HarnessLoader, testId: string): Promise<MatSliderHarness> {
    return matHarness.getHarness(MatSliderHarness.with({selector: `[data-testid="${testId}"]`}));
  }

  function triggerSliderInput(fixture: ComponentFixture<LightingComponent>, testId: string, value: number) {
    fixture.debugElement.query(By.css(`[data-testid="${testId}"]`))
      .triggerEventHandler('input', {value});
    fixture.detectChanges();
  }
})
