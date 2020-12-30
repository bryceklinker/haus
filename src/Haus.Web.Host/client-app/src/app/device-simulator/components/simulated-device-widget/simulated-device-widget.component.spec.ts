import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {screen} from "@testing-library/dom";

describe('SimulatedDeviceWidgetComponent', () => {

  it('should show device information', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice();

    const {container} = await renderWidget({simulatedDevice});

    expect(container).toHaveTextContent(simulatedDevice.id);
    expect(container).toHaveTextContent(simulatedDevice.deviceType);
  })

  it('should show device metadata', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({
      metadata: [
        ModelFactory.createMetadata('simulated', 'true'),
        ModelFactory.createMetadata('low', '100'),
        ModelFactory.createMetadata('high', '2000'),
      ]
    });

    const {container} = await renderWidget({simulatedDevice});

    expect(screen.queryAllByTestId('simulated-metadata-item')).toHaveLength(3);
    expect(container).toHaveTextContent('simulated');
    expect(container).toHaveTextContent('true');
    expect(container).toHaveTextContent('low');
    expect(container).toHaveTextContent('100');
    expect(container).toHaveTextContent('high');
    expect(container).toHaveTextContent('2000');
  })

  function renderWidget(props: Partial<SimulatedDeviceWidgetComponent>) {
    return renderFeatureComponent(SimulatedDeviceWidgetComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    })
  }
})
