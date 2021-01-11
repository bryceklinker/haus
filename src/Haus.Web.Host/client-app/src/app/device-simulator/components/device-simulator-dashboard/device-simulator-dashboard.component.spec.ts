import {screen} from "@testing-library/dom";

import {DeviceSimulatorDashboardComponent} from "./device-simulator-dashboard.component";
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorModule} from "../../device-simulator.module";

describe('DeviceSimulatorDashboardComponent', () => {
  it('should show connected when is connected', async () => {
    const {container} = await renderDashboard({isConnected: true});

    expect(container).toHaveTextContent('connected');
    expect(container).not.toHaveTextContent('disconnected');
  })

  it('should show disconnected when is not connected', async () => {
    const {container} = await renderDashboard({isConnected: false});

    expect(container).toHaveTextContent('disconnected');
  })

  it('should show each simulated device', async () => {
    await renderDashboard({
      simulatedDevices: [
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
      ]
    });

    expect(screen.queryAllByTestId('simulated-device-item')).toHaveLength(3);
  })

  function renderDashboard(props: Partial<DeviceSimulatorDashboardComponent>) {
    return renderFeatureComponent(DeviceSimulatorDashboardComponent, {
      imports: [DeviceSimulatorModule],
      componentProperties: props
    })
  }
})
