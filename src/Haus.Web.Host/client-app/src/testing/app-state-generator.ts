import {Action, ActionReducer, combineReducers} from "@ngrx/store";
import {AppState} from "../app/app.state";
import {appReducerMap} from "../app/app-reducer-map";

export function generateAppStateFromActions(...actions: Action[]): AppState {
  return generateStateFromActions(combineReducers(appReducerMap), ...actions);
}

export function generateStateFromActions<TState>(reducer: ActionReducer<TState>, ...actions: Action[]): TState {
  const initialState = reducer(undefined, {type: '@@init/store'});
  return actions.reduce((state, action) => reducer(state, action), initialState);
}
