import {eventually, ModelFactory} from "../../../../testing";
import {AddSimulatedDeviceComponent} from "./add-simulated-device.component";
import {DeviceSimulatorActions} from "../../state";
import {DeviceTypesActions} from "../../../devices/state";
import {DeviceType} from "../../../shared/models";
import {AddSimulatedDeviceHarness} from "./add-simulated-device.harness";

describe('AddSimulatedDeviceComponent', () => {
  it('should request device types to be loaded', async () => {
    const harness = await AddSimulatedDeviceHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DeviceTypesActions.loadDeviceTypes.request());
  })

  it('should disable save when form is not valid', async () => {
    const harness = await AddSimulatedDeviceHarness.render();

    expect(harness.saveButton).toBeDisabled();
  })

  it('should save simulated device using values from form', async () => {
    const deviceTypes = ModelFactory.createListResult(DeviceType.MotionSensor, DeviceType.Light);
    const harness = await AddSimulatedDeviceHarness.render(DeviceTypesActions.loadDeviceTypes.success(deviceTypes));

    await harness.selectDeviceType(DeviceType.Light);
    await harness.addMetadata();
    await harness.enterMetadataKey('something');
    await harness.enterMetadataValue('else');
    await harness.save();

    expect(harness.dispatchedActions).toContainEqual(DeviceSimulatorActions.addSimulatedDevice.request({
      deviceType: DeviceType.Light,
      metadata: [
        {key: 'something', value: 'else'}
      ]
    }))
  })

  it('should navigate to device simulator dashboard when add succeeds', async () => {
    const harness = await AddSimulatedDeviceHarness.render();

    await harness.simulateAddSuccess();

    await eventually(() => {
      expect(harness.router.navigateByUrl).toHaveBeenCalledWith('/device-simulator');
    })
  })

  it('should navigate to device simulator dashboard when cancelled', async () => {
    const harness = await AddSimulatedDeviceHarness.render();

    await harness.cancel();

    expect(harness.router.navigateByUrl).toHaveBeenCalledWith('/device-simulator');
  })
})
