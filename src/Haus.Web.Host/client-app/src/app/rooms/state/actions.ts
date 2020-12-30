import {createAsyncActionSet} from "../../shared/actions/create-async-action-set";
import {LightingModel, ListResult} from "../../shared/models";
import {CreateRoomModel, RoomModel} from "../models";
import {RoomLightingChangeModel} from "../models/room-lighting-change.model";

export const RoomsActions = {
  loadRooms: createAsyncActionSet(
    '[Rooms] Load Rooms',
    () => ({payload: 'ignore'}),
    (result: ListResult<RoomModel>) => ({payload: result.items}),
    (err: any) => ({payload: err})
  ),

  addRoom: createAsyncActionSet(
    '[Rooms] Add Room',
    (room: CreateRoomModel) => ({payload: room}),
    (room: RoomModel) => ({payload: room}),
    (err: any) => ({payload: err})
  ),

  changeRoomLighting: createAsyncActionSet(
    '[Rooms] Change Room Lighting',
    (change: RoomLightingChangeModel) => ({payload: change}),
    (change: RoomLightingChangeModel) => ({payload: change}),
    (roomId: number, err: any) => ({payload: {roomId, error: err}})
  )
}
