import {LoadingState} from "./loading.state";
import {Action, createSelector} from "@ngrx/store";
import {AppState} from "../../app.state";

const ASYNC_ACTION_REGEX = /(.*)\s(Request|Success|Failed)/i;

const INITIAL_STATE: LoadingState = {};

export function loadingReducer(state: LoadingState | undefined = INITIAL_STATE, action: Action): LoadingState {
  const matches = ASYNC_ACTION_REGEX.exec(action.type);
  if (!matches) {
    return state;
  }

  const [, asyncName, asyncState] = matches;
  return {
    ...state,
    [asyncName]: asyncState === 'Request'
  }
}

const selectLoadingState = (state: AppState) => state.loading;
