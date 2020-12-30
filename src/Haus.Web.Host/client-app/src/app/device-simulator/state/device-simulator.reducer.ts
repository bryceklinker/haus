import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {DeviceSimulatorState} from "./device-simulator.state";
import {AppState} from "../../app.state";
import {DeviceSimulatorActions} from "./actions";

const initialState: DeviceSimulatorState = {
  devices: [],
  isConnected: false
};

const reducer = createReducer(initialState,
  on(DeviceSimulatorActions.stateReceived, (state, {payload}) => ({...state, ...payload})),
  on(DeviceSimulatorActions.connected, state => ({...state, isConnected: true})),
  on(DeviceSimulatorActions.disconnected, state => ({...state, isConnected: false}))
);

export function deviceSimulatorReducer(state: DeviceSimulatorState | undefined, action: Action): DeviceSimulatorState {
  return reducer(state, action);
}

export const selectDeviceSimulatorState = (s: AppState) => s.deviceSimulator;
export const selectAllSimulatedDevices = createSelector(selectDeviceSimulatorState, s => s.devices);
export const selectIsDeviceSimulatorConnected = createSelector(selectDeviceSimulatorState, s => s.isConnected);
