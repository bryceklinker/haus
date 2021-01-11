import {screen} from "@testing-library/dom";

import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {SimulatedDeviceWidgetComponent} from "./simulated-device-widget.component";
import {DeviceType} from "../../../shared/models";

describe('SimulatedDeviceWidgetComponent', () => {

  it('should show device information', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.LightSensor});

    const {container} = await renderWidget({simulatedDevice});

    expect(container).toHaveTextContent(simulatedDevice.id);
    expect(container).toHaveTextContent('Light Sensor');
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

  it('should show lighting when simulator is a light', async () => {
    const simulatedDevice = ModelFactory.createSimulatedDevice({deviceType: DeviceType.Light});

    await renderWidget({simulatedDevice});

    expect(screen.getByTestId('lighting')).toBeInTheDocument();
  })

  function renderWidget(props: Partial<SimulatedDeviceWidgetComponent>) {
    return renderFeatureComponent(SimulatedDeviceWidgetComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    })
  }
})
