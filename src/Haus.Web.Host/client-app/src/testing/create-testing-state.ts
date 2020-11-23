import {Action, ActionReducer, combineReducers} from "@ngrx/store";
import {AppState} from "../app/app.state";
import {signalrReducer} from "ngrx-signalr-core";
import {initAction} from "./testing-actions";
import {DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer} from "../app/diagnostics/reducers/diagnostics.reducer";
import {loadingReducer} from "../app/shared/loading/loading.reducer";
import {devicesReducer} from "../app/devices/reducers/devices.reducer";

export function createTestingState(...actions: Action[]): AppState {
  const reducer = combineReducers<AppState>({
    signalr: signalrReducer,
    [DIAGNOSTICS_FEATURE_KEY]: diagnosticsReducer,
    loading: loadingReducer,
    devices: devicesReducer
  });
  return runActionsThroughReducer(reducer, ...actions);
}

export function runActionsThroughReducer<TState>(reducer: ActionReducer<TState>, ...actions: Action[]): TState {
  const initialState = reducer(undefined, initAction());
  return actions.reduce((state, action) => reducer(state, action), initialState);
}
