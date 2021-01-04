import {createAsyncActionSet} from "../../shared/actions/create-async-action-set";
import {DeviceModel, DeviceType, ListResult} from "../../shared/models";

export const DevicesActions = {
  loadDevices: createAsyncActionSet(
    '[Devices] Load Devices',
    () => ({id: 'ignore'}),
    (devices: ListResult<DeviceModel>) => ({payload: devices}),
    (err: any) => ({payload: err})
  ),
  turnOnDevice: createAsyncActionSet(
    '[Devices] Turn On Device',
    (id: number) => ({payload: id}),
    (id: number) => ({payload: id}),
    (id: number, err: any) => ({payload: {error: err, id}})
  ),
  turnOffDevice: createAsyncActionSet(
    '[Devices] Turn Off Device',
    (id: number) => ({payload: id}),
    (id: number) => ({payload: id}),
    (id: number, err: any) => ({payload: {error: err, id}})
  ),
  startDiscovery: createAsyncActionSet(
    '[Devices] Start Discovery',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),
  stopDiscovery: createAsyncActionSet(
    '[Devices] Stop Discovery',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  ),
  syncDiscovery: createAsyncActionSet(
    '[Devices] Sync Discovery',
    () => ({payload: 'ignore'}),
    () => ({payload: 'ignore'}),
    (err: any) => ({payload: err})
  )
};

export const DeviceTypesActions = {
  loadDeviceTypes: createAsyncActionSet(
    '[Device Types] Load Device Types',
    () => ({payload: 'ignore'}),
    (result: ListResult<DeviceType>) => ({payload: result.items}),
    (err: any) => ({payload: err})
  )
}
