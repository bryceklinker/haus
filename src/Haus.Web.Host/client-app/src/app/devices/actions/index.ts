import {ListResult} from "../../shared/models";
import {DeviceModel} from "../models";
import {createAsyncAction} from "../../shared/actions";

export const DevicesActions = {
  load: createAsyncAction(
    '[Devices] Load Devices',
    () => ({id: 'ignore'}),
    (devices: ListResult<DeviceModel>) => ({payload: devices}),
    (err: any) => ({payload: err})
  ),

  startDiscovery: createAsyncAction(
    '[Devices] Start Discovery',
    () => ({id: 'ignore'}),
    () => ({id: 'ignore'}),
    (err: any) => ({payload: err})
  ),

  stopDiscovery: createAsyncAction(
    '[Devices] Stop Discovery',
    () => ({id: 'ignore'}),
    () => ({id: 'ignore'}),
    (err: any) => ({payload: err})
  ),

  syncDiscovery: createAsyncAction(
    '[Devices] Sync Discovery',
    () => ({id: 'ignore'}),
    () => ({id: 'ignore'}),
    (err: any) => ({payload: err})
  ),

  turnOn: createAsyncAction(
    '[Devices] Turn Device On',
    (deviceId: number) => ({payload: deviceId}),
    (deviceId: number) => ({payload: deviceId}),
    (deviceId: number, err: any) => ({payload: {deviceId, err}})
  ),

  turnOff: createAsyncAction(
    '[Devices] Turn Device Off',
    (deviceId: number) => ({payload: deviceId}),
    (deviceId: number) => ({payload: deviceId}),
    (deviceId: number, err: any) => ({payload: {deviceId, err}})
  )
}
