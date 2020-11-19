import {Action, combineReducers} from "@ngrx/store";
import {AppState} from "../app/app.state";
import {signalrReducer} from "ngrx-signalr-core";
import {initAction} from "./testing-actions";

export function createTestingState(...actions: Action[]): AppState {
  const reducer = combineReducers<AppState>({
    signalr: signalrReducer
  });

  const initialState = reducer(undefined, initAction());
  return actions.reduce((state, action) => reducer(state, action), initialState);
}
