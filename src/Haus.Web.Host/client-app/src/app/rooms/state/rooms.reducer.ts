import {createEntityAdapter} from "@ngrx/entity";
import {Action, createReducer, createSelector, on} from "@ngrx/store";

import {createComparer} from "../../shared/sort-array-by";
import {RoomsState} from "./rooms.state";
import {RoomsActions} from "./actions";
import {AppState} from "../../app.state";
import {RoomModel} from "../../shared/models";
import {EventsActions} from "../../shared/events";

const adapter = createEntityAdapter<RoomModel>({
  selectId: r => r.id,
  sortComparer: createComparer(r => r.name)
})

const initialState: RoomsState = adapter.getInitialState();
const reducer = createReducer(initialState,
  on(RoomsActions.loadRooms.success, (state, {payload}) => adapter.upsertMany(payload, state)),
  on(RoomsActions.addRoom.success, RoomsActions.updateRoom.request, (state, {payload}) => ({...adapter.upsertOne(payload, state), isAdding: false})),
  on(RoomsActions.changeRoomLighting.request, EventsActions.roomLightingChanged, (state, {payload}) =>
    adapter.updateOne({
      id: payload.room.id,
      changes: {lighting: payload.lighting},
    }, state)
  ),
  on(EventsActions.roomCreated, EventsActions.roomUpdated, (state, {payload}) => ({...adapter.upsertOne(payload.room, state)}))
);

export function roomsReducer(state: RoomsState | undefined, action: Action) {
  return reducer(state, action);
}

const {
  selectAll,
  selectEntities
} = adapter.getSelectors();
const selectRoomsState = (state: AppState) => state.rooms;
export const selectAllRooms = createSelector(selectRoomsState, selectAll);
export const selectRoomById = (id: string | null) => createSelector(
  selectRoomsState,
  state => id ? selectEntities(state)[id] : null)
