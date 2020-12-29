import {createEntityAdapter} from "@ngrx/entity";
import {RoomModel} from "../models";
import {createComparer} from "../../shared/sort-array-by";
import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {RoomsState} from "./rooms.state";
import {RoomsActions} from "./actions";
import {AppState} from "../../app.state";

const adapter = createEntityAdapter<RoomModel>({
  selectId: r => r.id,
  sortComparer: createComparer(r => r.name)
})

const initialState: RoomsState = adapter.getInitialState({
  isAdding: false
});
const reducer = createReducer(initialState,
  on(RoomsActions.loadRooms.success, (state, {payload}) => adapter.upsertMany(payload, state)),
  on(RoomsActions.addRoom.begin, (state) => ({...state, isAdding: true})),
  on(RoomsActions.addRoom.cancel, (state) => ({...state, isAdding: false})),
  on(RoomsActions.addRoom.success, (state, {payload}) => ({...adapter.upsertOne(payload, state), isAdding: false})),
);

export function roomsReducer(state: RoomsState | undefined, action: Action){
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
