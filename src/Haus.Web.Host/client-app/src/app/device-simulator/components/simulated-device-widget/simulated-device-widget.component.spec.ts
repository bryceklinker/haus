import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {DeviceType} from "../../../shared/models";
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

  function renderWidget(props: Partial<SimulatedDeviceWidgetComponent>) {
    return renderFeatureComponent(SimulatedDeviceWidgetComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    })
  }
})
