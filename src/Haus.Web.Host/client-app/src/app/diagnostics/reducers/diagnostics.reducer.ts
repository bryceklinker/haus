import {DiagnosticsState} from "../diagnostics.state";
import {Action, createFeatureSelector, createReducer, createSelector, on} from "@ngrx/store";
import {DiagnosticsActions} from "../actions";
import {AppState} from "../../app.state";
import {selectHubStatus} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "../effects/diagnostics-hub";
import {DiagnosticsMessageModel} from "../models";

export const DIAGNOSTICS_FEATURE_KEY = 'diagnostics';

const INITIAL_STATE: DiagnosticsState = { messages: [] };
const reducer = createReducer(INITIAL_STATE,
  on(DiagnosticsActions.messageReceived, (state, {payload}) => ({...state, messages: [...state.messages, payload]})),
  on(DiagnosticsActions.replay.request, (state, {payload}) => ({
    ...state,
    messages: [
      ...state.messages.filter(m => m.id !== payload.id),
      {
        ...payload,
        isReplaying: true
      }
    ]
  })),
  on(DiagnosticsActions.replay.failed, (state, {payload}) => ({
    ...state,
    messages: [
      ...state.messages.filter(m => m.id !== payload.message.id),
      {
        ...payload.message,
        isReplaying: false,
        replayError: payload.error
      }
    ]
  })),
  on(DiagnosticsActions.replay.success, (state, {payload}) => ({
    ...state,
    messages: [
      ...state.messages.filter(m => m.id !== payload.id),
      {
        ...payload,
        isReplaying: false
      }
    ]
  }))
);

export function diagnosticsReducer(state: DiagnosticsState | undefined, action: Action): DiagnosticsState {
  return reducer(state, action);
}

const selectDiagnosticsState = createFeatureSelector<DiagnosticsState>(DIAGNOSTICS_FEATURE_KEY);
export const selectDiagnosticsMessages = createSelector(selectDiagnosticsState, s => [...s.messages].sort(sortMessagesDescending));
export const selectDiagnosticsHubState = (state:AppState) => selectHubStatus(state, DIAGNOSTICS_HUB);
export const selectIsDiagnosticHubConnected = createSelector(selectDiagnosticsHubState, s => s.state === 'connected');

function sortMessagesDescending(one: DiagnosticsMessageModel, two: DiagnosticsMessageModel): number {
  return Date.parse(two.timestamp) - Date.parse(one.timestamp);
}
