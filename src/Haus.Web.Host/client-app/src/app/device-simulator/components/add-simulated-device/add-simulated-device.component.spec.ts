import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
  TestingServer,
  TestingSettingsService
} from "../../../../testing";
import {AddSimulatedDeviceComponent} from "./add-simulated-device.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

const DEVICE_SIMULATOR_BASE_URL = 'https://localhost:4444';
describe('AddSimulatedDeviceComponent', () => {
  beforeEach(() => {
    TestingSettingsService.updateSettings({
      deviceSimulator: {
        url: DEVICE_SIMULATOR_BASE_URL,
        isEnabled: true
      }
    })
  })

  it('should get device types from api', async () => {
    const types = ModelFactory.createListResult(
      'Light',
      'MotionSensor',
      'LightSensor',
      'TemperatureSensor'
    )
    TestingServer.setupGet(`${DEVICE_SIMULATOR_BASE_URL}/api/deviceTypes`, types);

    const {detectChanges} = await renderComponent();

    await eventually(async () => {
      detectChanges();
      expect(screen.queryAllByTestId('device-type-item')).toHaveLength(4);
    })
  })

  function renderComponent() {
    return renderFeatureComponent(AddSimulatedDeviceComponent, {
      imports: [DeviceSimulatorModule]
    })
  }
})
