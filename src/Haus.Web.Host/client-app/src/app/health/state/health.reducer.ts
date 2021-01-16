import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {HealthState} from "./health.state";
import {AppState} from "../../app.state";
import {HealthActions} from "./actions";

const initialState: HealthState = {
  report: null
};
const reducer = createReducer(initialState,
  on(HealthActions.healthReceived, (state, {payload}) => ({
    ...state,
    report: payload
  }))
);

export function healthReducer(state: HealthState | undefined, action: Action): HealthState {
  return reducer(state, action);
}

const selectHealthState = (state: AppState) => state.health;
export const selectHealthReport = createSelector(selectHealthState, s => s.report);
