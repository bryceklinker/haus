import {MatInputHarness} from "@angular/material/input/testing";
import {MatButtonHarness} from "@angular/material/button/testing";
import {screen} from "@testing-library/dom";

import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {LightingConstraintsComponent} from "./lighting-constraints.component";
import {SharedModule} from "../../shared.module";
import {LightingConstraintsModel} from "../../models";

describe('LightingConstraintsComponent', () => {
  it('should show level values', async () => {
    const level = ModelFactory.createLevelLighting({min: 25, max: 90});
    const {minLevel, maxLevel} = await renderConstraints({level});

    expect(await minLevel.getValue()).toEqual('25');
    expect(await maxLevel.getValue()).toEqual('90');
  })

  it('should show temperature when temperature is provided', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting({min: 0, max: 255});

    const {minTemperature, maxTemperature} = await renderConstraints({level, temperature});

    expect(await minTemperature?.getValue()).toEqual('0');
    expect(await maxTemperature?.getValue()).toEqual('255');
  })

  it('should hide temperature controls when temperature is missing', async () => {
    const level = ModelFactory.createLevelLighting();

    await renderConstraints({level});

    expect(screen.queryByTestId('min-temperature-input')).not.toBeInTheDocument();
    expect(screen.queryByTestId('max-temperature-input')).not.toBeInTheDocument();
  })

  it('should notify when saved', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting();
    const emitter = new TestingEventEmitter<LightingConstraintsModel>();

    const {minLevel, maxLevel, minTemperature, maxTemperature, save} = await renderConstraints({level, temperature, save: emitter});
    await minLevel.setValue('54');
    await maxLevel.setValue('65');
    await minTemperature?.setValue('3000');
    await maxTemperature?.setValue('5000');
    await save.click();

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

    const {minLevel, maxLevel, save} = await renderConstraints({level, save: emitter});
    await minLevel.setValue('54');
    await maxLevel.setValue('65');
    await save.click();

    expect(emitter.emit).toHaveBeenCalledWith({
      minLevel: 54,
      maxLevel: 65
    })
  })

  it('should notify when cancelled', async () => {
    const level = ModelFactory.createLevelLighting();
    const emitter = new TestingEventEmitter();

    const {cancel} = await renderConstraints({level, cancel: emitter});

    await cancel.click();

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable save when constraints are invalid', async () => {
    const level = ModelFactory.createLevelLighting();

    const {save, minLevel} = await renderConstraints({level});
    await minLevel.setValue('');

    expect(await save.isDisabled()).toEqual(true);
  })

  it('should be invalid when min level is greater or equal to max level', async () => {
    const level = ModelFactory.createLevelLighting();

    const {minLevel, maxLevel, form} = await renderConstraints({level});
    await minLevel.setValue('40');
    await maxLevel.setValue('30');

    expect(form?.invalid).toEqual(true);
  })

  it('should be invalid when max level is less than or equal to min level', async () => {
    const level = ModelFactory.createLevelLighting();

    const {minLevel, maxLevel, form} = await renderConstraints({level});
    await minLevel.setValue('90');
    await maxLevel.setValue('45');

    expect(form?.invalid).toEqual(true);
  })

  it('should be invalid when min temperature is greater than or equal to max temperature', async () => {
    const temperature = ModelFactory.createTemperatureLighting();

    const {minTemperature, maxTemperature, form} = await renderConstraints({temperature});
    await maxTemperature?.setValue('45');
    await minTemperature?.setValue('90');

    expect(form?.invalid).toEqual(true);
  })

  it('should be invalid when max temperature is less than or equal to min temperature', async () => {
    const temperature = ModelFactory.createTemperatureLighting();

    const {minTemperature, maxTemperature, form} = await renderConstraints({temperature});
    await minTemperature?.setValue('90');
    await maxTemperature?.setValue('45');

    expect(form?.invalid).toEqual(true);
  })

  it('should disable all controls when readonly', async () => {
    const level = ModelFactory.createLevelLighting();
    const temperature = ModelFactory.createTemperatureLighting();

    const {minLevel, maxLevel, minTemperature, maxTemperature, save, cancel} = await renderConstraints({temperature, level, readonly: true});

    expect(await minLevel.isDisabled()).toEqual(true);
    expect(await maxLevel.isDisabled()).toEqual(true);
    expect(await minTemperature?.isDisabled()).toEqual(true);
    expect(await maxTemperature?.isDisabled()).toEqual(true);
    expect(await save.isDisabled()).toEqual(true);
    expect(await cancel.isDisabled()).toEqual(true);
  })

  it('should hide cancel when cancel is hidden', async () => {
    const level = ModelFactory.createLevelLighting();

    await renderWithoutGettingHarnesses({level, hideCancel: true});

    expect(screen.queryByTestId('cancel-constraints-btn')).not.toBeInTheDocument();
  })

  function renderWithoutGettingHarnesses(props: Partial<LightingConstraintsComponent>) {
    return renderFeatureComponent(LightingConstraintsComponent, {
      imports: [SharedModule],
      componentProperties: props
    });
  }

  async function renderConstraints(props: Partial<LightingConstraintsComponent> = {}) {
    const result = await renderWithoutGettingHarnesses(props);

    return {
      ...result,
      minLevel: await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="min-level-input"]'})),
      maxLevel: await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="max-level-input"]'})),
      minTemperature: screen.queryAllByTestId('min-temperature-input').length > 0
        ? await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="min-temperature-input"]'}))
        : undefined,
      maxTemperature: screen.queryAllByTestId('max-temperature-input').length > 0
        ? await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="max-temperature-input"]'}))
        : undefined,
      save: await result.matHarness.getHarness(MatButtonHarness.with({selector: '[data-testid="save-constraints-btn"'})),
      cancel: await result.matHarness.getHarness(MatButtonHarness.with({selector: '[data-testid="cancel-constraints-btn"'})),
      form: result.fixture.componentInstance.constraintsForm
    };
  }
})
