import {generateStateFromActions} from "../../../testing/app-state-generator";
import {deviceSimulatorReducer} from "./device-simulator.reducer";
import {DeviceSimulatorActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('deviceSimulatorReducer', () => {
  it('should return state from action when state received', () => {
    const expected = {
      devices: [ModelFactory.createSimulatedDevice()]
    };
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.stateReceived(expected)
    );

    expect(state).toEqual({...expected, isConnected: false});
  })

  it('should return connected when connected received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected());

    expect(state.isConnected).toEqual(true);
  })

  it('should return disconnected when disconnected received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected(),
      DeviceSimulatorActions.disconnected()
    );

    expect(state.isConnected).toEqual(false);
  })

  it('should maintain connected when state received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected(),
      DeviceSimulatorActions.stateReceived({devices: []})
    );

    expect(state.isConnected).toEqual(true);
  })
})
