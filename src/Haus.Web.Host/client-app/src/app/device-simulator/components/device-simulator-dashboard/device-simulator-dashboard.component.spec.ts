import {DeviceSimulatorDashboardComponent} from "./device-simulator-dashboard.component";
import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {DeviceSimulatorDashboardHarness} from "./device-simulator-dashboard.harness";
import {DeviceType, SimulatedDeviceModel} from "../../../shared/models";

describe('DeviceSimulatorDashboardComponent', () => {
  it('should show connected when is connected', async () => {
    const harness = await DeviceSimulatorDashboardHarness.render({isConnected: true});

    expect(harness.container).toHaveTextContent('connected');
    expect(harness.container).not.toHaveTextContent('disconnected');
  })

  it('should show disconnected when is not connected', async () => {
    const harness = await DeviceSimulatorDashboardHarness.render({isConnected: false});

    expect(harness.container).toHaveTextContent('disconnected');
  })

  it('should show each simulated device', async () => {
    const harness = await DeviceSimulatorDashboardHarness.render({
      simulatedDevices: [
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
      ]
    });

    expect(harness.simulatedDevices).toHaveLength(3);
  })

  it('should notify to change occupancy', async () => {
    const emitter = new TestingEventEmitter<SimulatedDeviceModel>();
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.MotionSensor});
    const harness = await DeviceSimulatorDashboardHarness.render({
      simulatedDevices: [simulatedDevice],
      occupancyChange: emitter
    });

    await harness.triggerOccupancyChange();

    expect(emitter.emit).toHaveBeenCalledWith(simulatedDevice);
  })
})
