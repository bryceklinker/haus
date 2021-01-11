import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {HarnessLoader} from "@angular/cdk/testing";
import {By} from "@angular/platform-browser";
import {ComponentFixture} from "@angular/core/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {screen} from "@testing-library/dom";

import {LightingComponent} from "./lighting.component";
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {LightingModel, LightingState} from "../../models";

describe('LightingComponent', () => {
  it('should show lighting values when rendered', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.On,
      level: ModelFactory.createLevelLighting({value: 45, min: 5, max: 95}),
      temperature: ModelFactory.createTemperatureLighting({value: 77, min: 0, max: 1000}),
      color: ModelFactory.createColorLighting()
    });
    const {level, state, temperature} = await renderLightingWithHarnesses({lighting});

    expect(await state.isChecked()).toEqual(true);
    expect(await level.getValue()).toEqual(45);
    expect(await level.getMinValue()).toEqual(5);
    expect(await level.getMaxValue()).toEqual(95);
    expect(await temperature.getValue()).toEqual(77);
    expect(await temperature.getMinValue()).toEqual(0);
    expect(await temperature.getMaxValue()).toEqual(1000);
  })

  it('should show lighting values', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.Off,
      level: ModelFactory.createLevelLighting({value: 45}),
      temperature: ModelFactory.createTemperatureLighting({value: 2700}),
      color: ModelFactory.createColorLighting()
    });

    await renderLightingWithHarnesses({lighting});

    expect(screen.getByTestId('state-value')).toHaveTextContent('Off');
    expect(screen.getByTestId('level-value')).toHaveTextContent('45');
    expect(screen.getByTestId('temperature-value')).toHaveTextContent('2700');
    expect(screen.getByTestId('lighting-color')).toBeInTheDocument();
  })

  it('should render show lighting not available when lighting is not provided', async () => {
    const {container} = await renderLighting();

    expect(container).toHaveTextContent('No lighting available');
  })

  it('should notify of change when brightness changes', async () => {
    const lighting = ModelFactory.createLighting({
      level: ModelFactory.createLevelLighting(),
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'level-input', 78);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      level: {...lighting.level, value: 78}
    })
  })

  it('should notify of change when temperature changes', async () => {
    const lighting = ModelFactory.createLighting({temperature: ModelFactory.createTemperatureLighting({value: 4500})});
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'temperature-input', 6000);

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

  it('should notify of change when state changes', async () => {
    const lighting = ModelFactory.createLighting({
      state: LightingState.Off,
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });
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
    const lighting = ModelFactory.createLighting({
      temperature: ModelFactory.createTemperatureLighting(),
      color: ModelFactory.createColorLighting()
    });

    const {level, state, temperature} = await renderLightingWithHarnesses({lighting, readonly: true});

    expect(await level.isDisabled()).toEqual(true);
    expect(await state.isDisabled()).toEqual(true);
    expect(await temperature.isDisabled()).toEqual(true);
  })

  it('should hide color when lighting does not have color', async () => {
    const lighting = ModelFactory.createLighting();

    await renderLighting({lighting});

    expect(screen.queryByTestId('lighting-color')).not.toBeInTheDocument();
  })

  it('should hide temperature when lighting does not have temperature', async () => {
    const lighting = ModelFactory.createLighting();

    await renderLighting({lighting});

    expect(screen.queryByTestId('temperature-input')).not.toBeInTheDocument();
  })

  async function renderLightingWithHarnesses(props: Partial<LightingComponent> = {}) {
    const result = await renderLighting(props);
    return {
      ...result,
      level: await getSliderHarness(result.matHarness, 'level-input'),
      state: await result.matHarness.getHarness(MatSlideToggleHarness.with({selector: '[data-testid="state-input"]'})),
      temperature: await getSliderHarness(result.matHarness, 'temperature-input')
    }
  }

  function renderLighting(props: Partial<LightingComponent> = {}) {
    return renderFeatureComponent(LightingComponent, {
      imports: [SharedModule],
      componentProperties: props
    });
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
