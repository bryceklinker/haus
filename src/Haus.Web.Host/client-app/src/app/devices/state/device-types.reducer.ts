import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {DeviceTypesState} from "./device-types.state";
import {AppState} from "../../app.state";
import {DeviceTypesActions} from "./actions";
import {sortArrayBy, SortDirection} from "../../shared/sort-array-by";

const initialState: DeviceTypesState = {
  deviceTypes: []
};
const reducer = createReducer(initialState,
  on(DeviceTypesActions.loadDeviceTypes.success, (state, {payload}) => ({...state, deviceTypes: sortArrayBy(payload, s => s, SortDirection.Ascending)})));

export function deviceTypesReducer(state: DeviceTypesState | undefined, action: Action): DeviceTypesState {
  return reducer(state, action);
}

const selectDeviceTypesState = (state: AppState) => state.deviceTypes;

export const selectAllDeviceTypes = createSelector(selectDeviceTypesState, s => s.deviceTypes);
