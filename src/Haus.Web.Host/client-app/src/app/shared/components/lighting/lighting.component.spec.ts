import {LightingComponent} from "./lighting.component";
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {LightingModel, LightingState} from "../../models";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {HarnessLoader} from "@angular/cdk/testing";
import {MatSlider} from "@angular/material/slider";
import {By} from "@angular/platform-browser";
import {ComponentFixture} from "@angular/core/testing";

describe('LightingComponent', () => {
  it('should show lighting values when rendered', async () => {
    const lighting = ModelFactory.createLighting({state: LightingState.On});
    const {brightness, state, temperature, red, green, blue} = await renderLightingWithHarnesses({lighting});

    expect(await brightness.getValue()).toEqual(lighting.brightnessPercent);
    expect(await state.isChecked()).toEqual(true);
    expect(await temperature.getValue()).toEqual(lighting.temperature);
    expect(await red.getValue()).toEqual(lighting.color.red);
    expect(await green.getValue()).toEqual(lighting.color.green);
    expect(await blue.getValue()).toEqual(lighting.color.blue);
  })

  it('should render show lighting not available when lighting is not provided', async () => {
    const {container} = await renderLighting();

    expect(container).toHaveTextContent('No lighting available');
  })

  it('should notify of change when brightness changes', async () => {
    const lighting = ModelFactory.createLighting({brightnessPercent: 0});
    const changeEmitter = new TestingEventEmitter<LightingModel>();
    const {fixture} = await renderLightingWithHarnesses({lighting, change: changeEmitter});

    triggerSliderInput(fixture, 'brightness-input', 78);

    expect(changeEmitter.emit).toHaveBeenCalledWith({
      ...lighting,
      brightnessPercent: 78
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
      brightness: await getSliderHarness(result.matHarness, 'brightness-input'),
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
