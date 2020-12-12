import {Action, ActionReducer, combineReducers} from "@ngrx/store";
import {routerReducer} from "@ngrx/router-store";
import {signalrReducer} from "ngrx-signalr-core";

import {AppState} from "../app/app.state";
import {TestingActions} from "./testing-actions";
import {DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer} from "../app/diagnostics/reducers/diagnostics.reducer";
import {loadingReducer} from "../app/shared/loading/loading.reducer";
import {DEVICES_FEATURE_KEY, devicesReducer} from "../app/devices/reducers/devices.reducer";

export function createAppState(...actions: Action[]): AppState {
  const reducer = combineReducers<AppState>({
    signalr: signalrReducer,
    loading: loadingReducer,
    router: routerReducer,
    [DIAGNOSTICS_FEATURE_KEY]: diagnosticsReducer,
    [DEVICES_FEATURE_KEY]: devicesReducer
  });

  const nonRouterActions = actions.filter(a => a.type !== TestingActions.setRouterState.type);
  const routerActions = actions.filter(a => a.type === TestingActions.setRouterState.type);
  const state = runActionsThroughReducer(reducer, ...nonRouterActions);
  return routerActions.reduce((state, action) => generateRouterState(state, <any>action), state)
}

export function runActionsThroughReducer<TState>(reducer: ActionReducer<TState>, ...actions: Action[]): TState {
  const initialState = reducer(undefined, TestingActions.initAction());
  return actions.reduce((state, action) => {
    if (action.type === TestingActions.setRouterState.type) {
      return {
        ...state,
        router: {
          ...(<any>state).router,
          ...(<ReturnType<typeof TestingActions.setRouterState>>action).payload
        }
      }
    }
    return reducer(state, action)
  }, initialState);
}

function generateRouterState(appState: AppState, action: ReturnType<typeof TestingActions.setRouterState>) {
  return {
    ...appState,
    router: {
      ...appState.router,
      state: {
        url: action.payload.url || '',
        queryParams: action.payload.queryParams || {},
        params: action.payload.params || {}
      }
    }
  };
}
