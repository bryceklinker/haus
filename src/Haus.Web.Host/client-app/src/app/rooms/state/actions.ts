import {createAsyncActionSet} from "../../shared/actions/create-async-action-set";
import {ListResult} from "../../shared/models";
import {CreateRoomModel, RoomModel} from "../models";
import {createAction} from "@ngrx/store";

export const RoomsActions = {
  loadRooms: createAsyncActionSet(
    '[Rooms] Load Rooms',
    () => ({payload: 'ignore'}),
    (result: ListResult<RoomModel>) => ({payload: result.items}),
    (err: any) => ({payload: err})
  ),

  addRoom: {
    begin: createAction('[Rooms] Add Room Begin'),
    cancel: createAction('[Rooms] Add Room Cancel'),
    ...createAsyncActionSet(
      '[Rooms] Add Room',
      (room: CreateRoomModel) => ({payload: room}),
      (room: RoomModel) => ({payload: room}),
      (err: any) => ({payload: err})
    )
  }
}
