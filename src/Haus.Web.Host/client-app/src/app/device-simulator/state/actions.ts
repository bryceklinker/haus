import {DeviceSimulatorState} from "./device-simulator.state";
import {createAction} from "@ngrx/store";

import {createAsyncActionSet} from "../../shared/actions";
import {SimulatedDeviceModel} from "../../shared/models";

export const DeviceSimulatorActions = {
  start: createAction('[Device Simulator] Start'),
  stop: createAction('[Device Simulator] Stop'),
  connected: createAction('[Device Simulator] Connected'),
  disconnected: createAction('[Device Simulator] Disconnected'),
  stateReceived: createAction('[Device Simulator] State Received', (state: DeviceSimulatorState) => ({payload: state})),

  addSimulatedDevice: createAsyncActionSet(
    '[Device Simulator] Add Simulated Device',
    (model: Partial<SimulatedDeviceModel>) => ({payload: model}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),

  triggerOccupancyChange: createAsyncActionSet(
    '[Device Simulator] Trigger Occupancy Change',
    (model: SimulatedDeviceModel) => ({payload: model}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  )
}
