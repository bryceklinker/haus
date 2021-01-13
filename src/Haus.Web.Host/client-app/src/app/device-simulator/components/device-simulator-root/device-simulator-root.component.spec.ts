import {ModelFactory} from "../../../../testing";
import {DeviceSimulatorRootComponent} from "./device-simulator-root.component";
import {DeviceSimulatorActions} from "../../state";
import {DeviceSimulatorRootHarness} from "./device-simulator-root.harness";

describe('DeviceSimulatorRootComponent', () => {
  it('should start connection to device simulator', async () => {
    const harness = await DeviceSimulatorRootHarness.render();

    expect(harness.dispatchedActions).toContainEqual(DeviceSimulatorActions.start());
  })

  it('should show all simulated devices', async () => {
    const harness = await DeviceSimulatorRootHarness.render(
      DeviceSimulatorActions.stateReceived({
        devices: [
          ModelFactory.createSimulatedDevice(),
          ModelFactory.createSimulatedDevice(),
          ModelFactory.createSimulatedDevice(),
        ]
      })
    )

    expect(harness.simulatedDevices).toHaveLength(3);
  })

  it('should show connection status of device simulator', async () => {
    const harness = await DeviceSimulatorRootHarness.render(DeviceSimulatorActions.connected());

    expect(harness.container).toHaveTextContent('connected');
  })

  it('should stop connection when destroyed', async () => {
    const harness = await DeviceSimulatorRootHarness.render();

    harness.destroy();

    expect(harness.dispatchedActions).toContainEqual(DeviceSimulatorActions.stop());
  })
})
