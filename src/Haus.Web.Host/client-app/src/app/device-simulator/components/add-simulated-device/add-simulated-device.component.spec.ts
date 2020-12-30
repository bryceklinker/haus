import {eventually, ModelFactory, renderFeatureComponent} from "../../../../testing";
import {AddSimulatedDeviceComponent} from "./add-simulated-device.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {DeviceSimulatorActions} from "../../state";
import {DeviceTypesActions} from "../../../devices/state";
import {Action} from "@ngrx/store";
import {MatSelectHarness} from "@angular/material/select/testing";

describe('AddSimulatedDeviceComponent', () => {
  it('should request device types to be loaded', async () => {
    const {store} = await renderAdd();

    expect(store.dispatchedActions).toContainEqual(DeviceTypesActions.loadDeviceTypes.request());
  })

  it('should disable save when form is not valid', async () => {
    await renderAdd();

    expect(screen.getByTestId('save-simulated-device-btn')).toBeDisabled();
  })

  it('should save simulated device using values from form', async () => {
    const deviceTypes = ModelFactory.createListResult('one', 'two');
    const {store, matHarness, detectChanges} = await renderAdd(DeviceTypesActions.loadDeviceTypes.success(deviceTypes));

    const select = await matHarness.getHarness(MatSelectHarness)
    await select.open();
    await select.clickOptions({text: 'Two'});

    userEvent.click(screen.getByTestId('add-metadata-btn'));
    detectChanges();

    userEvent.type(screen.getByTestId('metadata-key-input'), 'something');
    userEvent.type(screen.getByTestId('metadata-value-input'), 'else');
    detectChanges();

    userEvent.click(screen.getByTestId('save-simulated-device-btn'));

    expect(store.dispatchedActions).toContainEqual(DeviceSimulatorActions.addSimulatedDevice.request({
      deviceType: 'two',
      metadata: [
        {key: 'something', value: 'else'}
      ]
    }))
  })

  it('should navigate to device simulator dashboard when add succeeds', async () => {
    const {actionsSubject, router} = await renderAdd();
    spyOn(router, 'navigateByUrl');

    actionsSubject.next(DeviceSimulatorActions.addSimulatedDevice.success());
    await eventually(() => {
      expect(router.navigateByUrl).toHaveBeenCalledWith('/device-simulator');
    })
  })

  it('should navigate to device simulator dashboard when cancelled', async () => {
    const {router} = await renderAdd();
    spyOn(router, 'navigateByUrl');

    userEvent.click(screen.getByTestId('cancel-simulated-device-btn'));

    expect(router.navigateByUrl).toHaveBeenCalledWith('/device-simulator');
  })

  function renderAdd(...actions: Array<Action>) {
    return renderFeatureComponent(AddSimulatedDeviceComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions
    })
  }
})
