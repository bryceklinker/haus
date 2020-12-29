import {Action, ActionReducer, combineReducers} from "@ngrx/store";
import {AppState} from "../app/app.state";
import {sharedReducerMap} from "../app/shared/shared-reducer-map";

export function generateAppStateFromActions(...actions: Action[]): AppState {
  return generateStateFromActions(combineReducers(sharedReducerMap), ...actions);
}

export function generateStateFromActions<TState>(reducer: ActionReducer<TState>, ...actions: Action[]): TState {
  const initialState = reducer(undefined, {type: '@@init/store'});
  return actions.reduce((state, action) => reducer(state, action), initialState);
}
