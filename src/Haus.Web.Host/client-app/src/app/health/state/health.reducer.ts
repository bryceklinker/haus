import {Action, createReducer} from "@ngrx/store";
import {HealthState} from "./health.state";

const initialState: HealthState = {};
const reducer = createReducer(initialState);

export function healthReducer(state: HealthState | undefined, action: Action): HealthState {
  return reducer(state, action);
}
