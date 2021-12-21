import {generateStateFromActions} from "../../../testing/app-state-generator";
import {deviceSimulatorReducer} from "./device-simulator.reducer";
import {DeviceSimulatorActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('deviceSimulatorReducer', () => {
  test('should return state from action when state received', () => {
    const expected = {
      devices: [ModelFactory.createSimulatedDevice()]
    };
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.stateReceived(expected)
    );

    expect(state).toEqual({...expected, isConnected: false});
  })

  test('should return connected when connected received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected());

    expect(state.isConnected).toEqual(true);
  })

  test('should return disconnected when disconnected received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected(),
      DeviceSimulatorActions.disconnected()
    );

    expect(state.isConnected).toEqual(false);
  })

  test('should maintain connected when state received', () => {
    const state = generateStateFromActions(deviceSimulatorReducer,
      DeviceSimulatorActions.connected(),
      DeviceSimulatorActions.stateReceived({devices: []})
    );

    expect(state.isConnected).toEqual(true);
  })
})
