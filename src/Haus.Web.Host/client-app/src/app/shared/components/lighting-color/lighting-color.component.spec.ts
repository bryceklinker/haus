import {ComponentFixture} from "@angular/core/testing";
import {By} from "@angular/platform-browser";
import {HarnessLoader} from "@angular/cdk/testing";
import {MatSliderHarness} from "@angular/material/slider/testing";
import {screen} from "@testing-library/dom";

import {LightingColorComponent} from "./lighting-color.component";
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {LightingColorModel} from "../../models";

describe('LightingColorComponent', () => {
  it('should show color values when rendered', async () => {
    const color = ModelFactory.createLightingColor({red: 7, green: 9, blue: 19});

    const {red, green, blue} = await renderColorWithHarnesses({color});

    expect(await red.getValue()).toEqual(color.red);
    expect(await green.getValue()).toEqual(color.green);
    expect(await blue.getValue()).toEqual(color.blue);
  })

  it('should show color values when rendered', async () => {
    const color = ModelFactory.createLightingColor({red: 6, green: 9, blue: 8});

    await renderColor({color});

    expect(screen.getByTestId('rgb-value')).toHaveTextContent('6');
    expect(screen.getByTestId('rgb-value')).toHaveTextContent('9');
    expect(screen.getByTestId('rgb-value')).toHaveTextContent('8');
    expect(screen.getByTestId('hex-value')).toHaveTextContent('#060908');
  })

  it('should notify of color change when red changes', async () => {
    const color = ModelFactory.createLightingColor();
    const emitter = new TestingEventEmitter<LightingColorModel>();

    const {fixture} = await renderColorWithHarnesses({color, colorChange: emitter});

    triggerSliderInput(fixture, 'red-input', 55);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      red: 55
    })
  })

  it('should notify of color change when green changes', async () => {
    const color = ModelFactory.createLightingColor();
    const emitter = new TestingEventEmitter<LightingColorModel>();

    const {fixture} = await renderColorWithHarnesses({color, colorChange: emitter});

    triggerSliderInput(fixture, 'green-input', 77);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      green: 77
    })
  })

  it('should notify of color change when blue changes', async () => {
    const color = ModelFactory.createLightingColor();
    const emitter = new TestingEventEmitter<LightingColorModel>();

    const {fixture} = await renderColorWithHarnesses({color, colorChange: emitter});

    triggerSliderInput(fixture, 'blue-input', 99);

    expect(emitter.emit).toHaveBeenCalledWith({
      ...color,
      blue: 99
    })
  })

  it('should disable controls when readonly', async () => {
    const color = ModelFactory.createLightingColor();

    const {red, green, blue} = await renderColorWithHarnesses({color, readonly: true});

    expect(await red.isDisabled()).toEqual(true);
    expect(await green.isDisabled()).toEqual(true);
    expect(await blue.isDisabled()).toEqual(true);
  })

  async function renderColorWithHarnesses(props: Partial<LightingColorComponent>) {
    const result = await renderColor(props);
    return {
      ...result,
      red: await getSliderHarness(result.matHarness, 'red-input'),
      green: await getSliderHarness(result.matHarness, 'green-input'),
      blue: await getSliderHarness(result.matHarness, 'blue-input')
    }
  }

  function renderColor(props: Partial<LightingColorComponent>) {
    return renderFeatureComponent(LightingColorComponent, {
      imports: [SharedModule],
      componentProperties: props
    })
  }

  function getSliderHarness(matHarness: HarnessLoader, testId: string): Promise<MatSliderHarness> {
    return matHarness.getHarness(MatSliderHarness.with({selector: `[data-testid="${testId}"]`}));
  }

  function triggerSliderInput(fixture: ComponentFixture<LightingColorComponent>, testId: string, value: number) {
    fixture.debugElement.query(By.css(`[data-testid="${testId}"]`))
      .triggerEventHandler('input', {value});
    fixture.detectChanges();
  }
})
