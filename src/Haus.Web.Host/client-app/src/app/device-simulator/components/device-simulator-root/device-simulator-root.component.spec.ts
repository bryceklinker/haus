import {eventually, ModelFactory, renderFeatureComponent, TestingSettingsService} from "../../../../testing";
import {DeviceSimulatorRootComponent} from "./device-simulator-root.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {screen} from "@testing-library/dom";

const DEVICE_SIMULATOR_BASE_URL = 'https://localhost:5001';
describe('DeviceSimulatorRootComponent', () => {
  beforeEach(() => {
      TestingSettingsService.updateSettings({
        deviceSimulator: {
          url: DEVICE_SIMULATOR_BASE_URL,
          isEnabled: true
        }
      })
  })

  it('should show devices from state', async () => {
    const model = ModelFactory.createDeviceSimulatorState({
      devices: [
        ModelFactory.createDeviceModel(),
        ModelFactory.createDeviceModel(),
        ModelFactory.createDeviceModel(),
      ]
    })
    const {signalrConnectionFactory, detectChanges} = await renderRoot();
    const hub = signalrConnectionFactory.getTestingHub(`${DEVICE_SIMULATOR_BASE_URL}/hubs/devices`);
    hub.triggerMessage('OnStateChange', model);

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('simulated-device')).toHaveLength(3);
    })
  })

  it('should stop device simulator connection when destroyed', async () => {
    const {signalrConnectionFactory, fixture} = await renderRoot();
    const hub = signalrConnectionFactory.getTestingHub(`${DEVICE_SIMULATOR_BASE_URL}/hubs/devices`);

    fixture.destroy();
    expect(hub.stop).toHaveBeenCalled();
  })

  function renderRoot() {
    return renderFeatureComponent(DeviceSimulatorRootComponent, {
      imports: [DeviceSimulatorModule]
    })
  }
})
