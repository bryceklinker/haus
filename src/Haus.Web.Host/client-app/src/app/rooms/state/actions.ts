import {createAsyncActionSet} from "../../shared/actions";
import {ListResult, RoomLightingChangedEvent, RoomModel} from "../../shared/models";
import {AssignDevicesToRoomModel} from "../models";

export const RoomsActions = {
  loadRooms: createAsyncActionSet(
    '[Rooms] Load Rooms',
    () => ({payload: 'ignore'}),
    (result: ListResult<RoomModel>) => ({payload: result.items}),
    (error: any) => ({payload: error})
  ),

  addRoom: createAsyncActionSet(
    '[Rooms] Add Room',
    (room: Partial<RoomModel>) => ({payload: room}),
    (room: RoomModel) => ({payload: room}),
    (error: any) => ({payload: error})
  ),

  changeRoomLighting: createAsyncActionSet(
    '[Rooms] Change Room Lighting',
    (change: RoomLightingChangedEvent) => ({payload: change}),
    (change: RoomLightingChangedEvent) => ({payload: change}),
    (roomId: number, error: any) => ({payload: {roomId, error}})
  ),

  assignDevicesToRoom: createAsyncActionSet(
    '[Rooms] Assign Devices To Room',
    (assignment: AssignDevicesToRoomModel) => ({payload: assignment}),
    (assignment: AssignDevicesToRoomModel) =>({payload: assignment}),
    (roomId: number, error: any) => ({payload: {roomId, error}})
  )
}
