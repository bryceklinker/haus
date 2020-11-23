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
}
