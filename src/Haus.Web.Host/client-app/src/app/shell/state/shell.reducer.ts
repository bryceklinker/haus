import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {ShellState} from "./shell.state";
import {ShellActions} from "./actions";
import {AppState} from "../../app.state";

const initialState: ShellState = {
  latestVersion: null
};

const reducer = createReducer(initialState,
  on(ShellActions.loadLatestVersion.success, (state, {payload}) => ({...state, latestVersion: payload}))
);

export function shellReducer(state: ShellState | undefined, action: Action): ShellState {
  return reducer(state, action);
}

const selectShellState = (state: AppState) => state.shell;
export const selectLatestVersion = createSelector(selectShellState, s => s.latestVersion);
export const selectIsUpdateAvailable = createSelector(selectLatestVersion, s => !!s && s.isNewer);
