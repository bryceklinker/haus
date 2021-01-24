import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {ShellState} from "./shell.state";
import {ShellActions} from "./actions";
import {AppState} from "../../app.state";
import {selectIsLoading} from "../../shared/loading";

const initialState: ShellState = {
  latestVersion: null,
  latestPackages: [],
  loadPackagesError: null,
  loadVersionError: null,
  downloadPackageError: null
};

const reducer = createReducer(initialState,
  on(ShellActions.loadLatestVersion.success, (state, {payload}) => ({...state, latestVersion: payload})),
  on(ShellActions.loadLatestPackages.success, (state, {payload}) => ({...state, latestPackages: payload})),
  on(ShellActions.loadLatestVersion.failed, (state, {payload}) => ({...state, loadVersionError: payload})),
  on(ShellActions.loadLatestPackages.failed, (state, {payload}) => ({...state, loadPackagesError: payload})),
  on(ShellActions.downloadPackage.failed, (state, {payload}) => ({...state, downloadPackageError: payload})),
);

export function shellReducer(state: ShellState | undefined, action: Action): ShellState {
  return reducer(state, action);
}

const selectShellState = (state: AppState) => state.shell;
export const selectLatestVersion = createSelector(selectShellState, s => s.latestVersion);
export const selectIsUpdateAvailable = createSelector(selectLatestVersion, s => !!s && s.isNewer);
export const selectHasLatestVersionError = createSelector(selectShellState, s => s.loadVersionError !== null);
export const selectHasLatestVersion = createSelector(selectShellState, s => s.latestVersion !== null && s.loadVersionError === null);
export const selectLatestPackages = createSelector(selectShellState, s => s.latestPackages || []);
export const selectLatestVersionError = createSelector(selectShellState, s => s.loadVersionError);
export const selectLatestPackagesError = createSelector(selectShellState, s => s.loadPackagesError);
export const selectDownloadPackageError = createSelector(selectShellState, s => s.downloadPackageError);
export const selectIsDownloadingPackage = selectIsLoading(ShellActions.downloadPackage.request);
