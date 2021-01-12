import {createAsyncActionSet} from "../../shared/actions";
import {DeviceModel, DeviceType, LightType, ListResult} from "../../shared/models";
import {DeviceLightingConstraintsModel} from "../models";

export const DevicesActions = {
  loadDevices: createAsyncActionSet(
    '[Devices] Load Devices',
    () => ({id: 'ignore'}),
    (devices: ListResult<DeviceModel>) => ({payload: devices}),
    (err: any) => ({payload: err})
  ),
  updateDevice: createAsyncActionSet(
    '[Devices] Update Device',
    (device: DeviceModel) => ({payload: device}),
    (device: DeviceModel) => ({payload: device}),
    (id: number, error: any) => ({id, error})
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
    '[Devices] Change Lighting Constraints',
    (payload: DeviceLightingConstraintsModel) => ({payload}),
    (payload: DeviceLightingConstraintsModel) => ({payload}),
    (id: number, error: any) => ({payload: {id, error}})
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

export const LightTypesActions = {
  loadLightTypes: createAsyncActionSet(
    '[Light Types] Load Light Types',
    () => ({payload: 'ignore'}),
    (result: ListResult<LightType>) => ({payload: result.items}),
    (error: any) => ({payload: error})
  )
}
