import {createAsyncActionSet} from "../../shared/actions";
import {DeviceLightingConstraintsChangedEvent, DeviceModel, DeviceType, ListResult} from "../../shared/models";

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
  changeDeviceLightingConstraints: createAsyncActionSet(
    '[Devices] Update Device Lighting Constraints',
    (model: DeviceLightingConstraintsChangedEvent) => ({payload: model}),
    (model: DeviceLightingConstraintsChangedEvent) => ({payload: model}),
    (id: number, error: any) => ({payload: {error, id}})
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
