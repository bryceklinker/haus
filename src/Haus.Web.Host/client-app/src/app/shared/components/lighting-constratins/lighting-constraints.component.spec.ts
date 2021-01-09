import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {LightingConstraintsComponent} from "./lighting-constraints.component";
import {SharedModule} from "../../shared.module";
import {MatInputHarness} from "@angular/material/input/testing";
import {MatButtonHarness} from "@angular/material/button/testing";
import {LightingConstraintsModel} from "../../models";
import {screen} from "@testing-library/dom";

describe('LightingConstraintsComponent', () => {
  it('should show constraints values', async () => {
    const constraints = ModelFactory.createLightingConstraints({minLevel: 9, maxLevel: 98, minTemperature: 90, maxTemperature: 100});
    const {minLevel, maxLevel, minTemperature, maxTemperature} = await renderConstraints({constraints});

    expect(await minLevel.getValue()).toEqual('9');
    expect(await maxLevel.getValue()).toEqual('98');
    expect(await minTemperature.getValue()).toEqual('90');
    expect(await maxTemperature.getValue()).toEqual('100');
  })

  it('should notify when constraints are saved', async () => {
    const constraints = ModelFactory.createLightingConstraints();
    const emitter = new TestingEventEmitter<LightingConstraintsModel>();

    const {minLevel, maxLevel, minTemperature, maxTemperature, save} = await renderConstraints({constraints, save: emitter});
    await minLevel.setValue('54');
    await maxLevel.setValue('65');
    await minTemperature.setValue('3000');
    await maxTemperature.setValue('5000');
    await save.click();

    expect(emitter.emit).toHaveBeenCalledWith({
      minLevel: 54,
      maxLevel: 65,
      minTemperature: 3000,
      maxTemperature: 5000
    });
  })

  it('should notify when cancelled', async () => {
    const constraints = ModelFactory.createLightingConstraints();
    const emitter = new TestingEventEmitter();

    const {cancel} = await renderConstraints({constraints, cancel: emitter});

    await cancel.click();

    expect(emitter.emit).toHaveBeenCalled();
  })

  it('should disable save when constraints are invalid', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {save, minLevel} = await renderConstraints({constraints});
    await minLevel.setValue('');

    expect(await save.isDisabled()).toEqual(true);
  })

  it('should be invalid when min level is greater or equal to max level', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {minLevel, maxLevel, form} = await renderConstraints({constraints});
    await minLevel.setValue('40');
    await maxLevel.setValue('30');

    expect(form.get('minLevel')?.invalid).toEqual(true);
  })

  it('should be invalid when max level is less than or equal to min level', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {minLevel, maxLevel, form} = await renderConstraints({constraints});
    await minLevel.setValue('90');
    await maxLevel.setValue('45');

    expect(form.get('maxLevel')?.invalid).toEqual(true);
  })

  it('should be invalid when min temperature is greater than or equal to max temperature', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {minTemperature, maxTemperature, form} = await renderConstraints({constraints});
    await maxTemperature.setValue('45');
    await minTemperature.setValue('90');

    expect(form.get('minTemperature')?.invalid).toEqual(true);
  })

  it('should be invalid when max temperature is less than or equal to min temperature', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {minTemperature, maxTemperature, form} = await renderConstraints({constraints});
    await minTemperature.setValue('90');
    await maxTemperature.setValue('45');

    expect(form.get('minTemperature')?.invalid).toEqual(true);
  })

  it('should disable all controls when readonly', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    const {minLevel, maxLevel, minTemperature, maxTemperature, save, cancel} = await renderConstraints({constraints, readonly: true});

    expect(await minLevel.isDisabled()).toEqual(true);
    expect(await maxLevel.isDisabled()).toEqual(true);
    expect(await minTemperature.isDisabled()).toEqual(true);
    expect(await maxTemperature.isDisabled()).toEqual(true);
    expect(await save.isDisabled()).toEqual(true);
    expect(await cancel.isDisabled()).toEqual(true);
  })

  it('should hide cancel when cancel is hidden', async () => {
    const constraints = ModelFactory.createLightingConstraints();

    await renderWithoutGettingHarnesses({constraints, hideCancel: true});

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
      minTemperature: await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="min-temperature-input"]'})),
      maxTemperature: await result.matHarness.getHarness(MatInputHarness.with({selector: '[data-testid="max-temperature-input"]'})),
      save: await result.matHarness.getHarness(MatButtonHarness.with({selector: '[data-testid="save-constraints-btn"'})),
      cancel: await result.matHarness.getHarness(MatButtonHarness.with({selector: '[data-testid="cancel-constraints-btn"'})),
      form: result.fixture.componentInstance.constraintsForm
    };
  }
})
