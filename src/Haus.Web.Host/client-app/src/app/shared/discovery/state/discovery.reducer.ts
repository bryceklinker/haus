import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {AppDiscoveryState} from "./app-discovery.state";
import {AppState} from "../../../app.state";
import {DiscoveryActions} from "./actions";
import {EventsActions} from "../../events";
import {DiscoveryState} from "../../models";

const initialState: AppDiscoveryState = {
  isDiscoveryAllowed: false
};

const reducer = createReducer(initialState,
  on(DiscoveryActions.startDiscovery.success, EventsActions.discoveryStarted, (state) => ({...state, isDiscoveryAllowed: true})),
  on(DiscoveryActions.stopDiscovery.success, EventsActions.discoveryStopped, (state) => ({...state, isDiscoveryAllowed: false})),
  on(DiscoveryActions.getDiscovery.success, (state, {payload}) => ({...state, isDiscoveryAllowed: payload.state === DiscoveryState.Enabled}))
);

export function discoveryReducer(state: AppDiscoveryState | undefined, action: Action): AppDiscoveryState {
  return reducer(state, action);
}

const selectDiscoveryState = (state: AppState) => state.discovery;
export const selectIsDiscoveryAllowed = createSelector(selectDiscoveryState, s => s.isDiscoveryAllowed);
