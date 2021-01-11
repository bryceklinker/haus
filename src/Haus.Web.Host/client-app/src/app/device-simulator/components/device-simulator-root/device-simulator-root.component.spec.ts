import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DeviceSimulatorRootComponent} from "./device-simulator-root.component";
import {DeviceSimulatorModule} from "../../device-simulator.module";
import {DeviceSimulatorActions} from "../../state";

describe('DeviceSimulatorRootComponent', () => {
  it('should start connection to device simulator', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(DeviceSimulatorActions.start());
  })

  it('should show all simulated devices', async () => {
    await renderRoot(DeviceSimulatorActions.stateReceived({
      devices: [
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
      ]
    }));

    expect(screen.queryAllByTestId('simulated-device-item')).toHaveLength(3);
  })

  it('should show connection status of device simulator', async () => {
    const {container} = await renderRoot(DeviceSimulatorActions.connected());

    expect(container).toHaveTextContent('connected');
  })

  it('should stop connection when destroyed', async () => {
    const {store, fixture} = await renderRoot();

    fixture.destroy();

    expect(store.dispatchedActions).toContainEqual(DeviceSimulatorActions.stop());
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(DeviceSimulatorRootComponent, {
      imports: [DeviceSimulatorModule],
      actions: actions
    })
  }
})
