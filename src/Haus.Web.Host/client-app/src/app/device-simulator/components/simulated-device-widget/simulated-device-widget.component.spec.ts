import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {DeviceType, SimulatedDeviceModel} from "../../../shared/models";
import {SimulatedDeviceWidgetHarness} from "./simulated-device-widget.harness";

describe('SimulatedDeviceWidgetComponent', () => {
  it('should show device information', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.LightSensor});

    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice});

    expect(harness.container).toHaveTextContent(simulatedDevice.id);
    expect(harness.container).toHaveTextContent('Light Sensor');
  })

  it('should show device metadata', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({
      metadata: [
        ModelFactory.createMetadata('simulated', 'true'),
        ModelFactory.createMetadata('low', '100'),
        ModelFactory.createMetadata('high', '2000'),
      ]
    });

    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice});

    expect(harness.simulatedMetadata).toHaveLength(3);
    expect(harness.container).toHaveTextContent('simulated');
    expect(harness.container).toHaveTextContent('true');
    expect(harness.container).toHaveTextContent('low');
    expect(harness.container).toHaveTextContent('100');
    expect(harness.container).toHaveTextContent('high');
    expect(harness.container).toHaveTextContent('2000');
  })

  it('should show lighting when simulator is a light', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.Light});

    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice});

    expect(harness.lighting).toBeInTheDocument();
  })

  it('should show no lighting when simulator is missing lighting', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice();
    simulatedDevice.lighting = null as any;

    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice});

    expect(harness.lightingExists).toEqual(false);
  })

  it('should show empty data when simulated device is missing', async () => {
    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice: undefined});

    expect(harness.container).toHaveTextContent('N/A');
  })

  it('should notify to trigger occupancy change', async () => {
    const emitter = new TestingEventEmitter<SimulatedDeviceModel>();
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.MotionSensor});

    const harness = await SimulatedDeviceWidgetHarness.render({simulatedDevice, occupancyChange: emitter});
    await harness.triggerOccupancyChange();

    expect(emitter.emit).toHaveBeenCalledWith(simulatedDevice);
  })

  it('should show if device is occupied', async () => {
    const harness = await SimulatedDeviceWidgetHarness.render({
      simulatedDevice: ModelFactory.createSimulatedDevice({deviceType: DeviceType.MotionSensor, isOccupied: true})
    });

    expect(harness.getIsOccupied()).toEqual(true);
  })
})
