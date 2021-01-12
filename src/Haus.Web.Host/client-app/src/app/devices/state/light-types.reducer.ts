import {LightTypesState} from "./light-types.state";
import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {AppState} from "../../app.state";
import {LightTypesActions} from "./actions";
import {sortArrayBy, SortDirection} from "../../shared/sort-array-by";

const initialState: LightTypesState = {
  lightTypes: []
};

const reducer = createReducer(initialState,
  on(LightTypesActions.loadLightTypes.success, (state, {payload}) => ({...state, lightTypes: sortArrayBy(payload, s => s, SortDirection.Ascending)}))
);

export function lightTypesReducer(state: LightTypesState | undefined, action: Action): LightTypesState {
  return reducer(state, action);
}

const selectLightTypesState = (state: AppState) => state.lightTypes;

export const selectAllLightTypes = createSelector(selectLightTypesState, s => s.lightTypes)
