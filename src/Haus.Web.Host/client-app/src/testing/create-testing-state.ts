import {Action, ActionReducer, combineReducers} from "@ngrx/store";
import {routerReducer} from "@ngrx/router-store";
import {signalrReducer} from "ngrx-signalr-core";

import {AppState} from "../app/app.state";
import {initAction, setRouterState} from "./testing-actions";
import {DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer} from "../app/diagnostics/reducers/diagnostics.reducer";
import {loadingReducer} from "../app/shared/loading/loading.reducer";
import {DEVICES_FEATURE_KEY, devicesReducer} from "../app/devices/reducers/devices.reducer";

export function createTestingState(...actions: Action[]): AppState {
  const reducer = combineReducers<AppState>({
    signalr: signalrReducer,
    loading: loadingReducer,
    router: routerReducer,
    [DIAGNOSTICS_FEATURE_KEY]: diagnosticsReducer,
    [DEVICES_FEATURE_KEY]: devicesReducer
  });
  return runActionsThroughReducer(reducer, ...actions);
}

export function runActionsThroughReducer<TState>(reducer: ActionReducer<TState>, ...actions: Action[]): TState {
  const initialState = reducer(undefined, initAction());
  return actions.reduce((state, action) => {
    if (action.type === setRouterState.type) {
      return {
        ...state,
        router: {
          ...(<ReturnType<typeof setRouterState>>action).payload
        }
      }
    }
    return reducer(state, action)
  }, initialState);
}
