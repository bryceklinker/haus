import {DiagnosticsState} from "../diagnostics.state";
import {Action, createFeatureSelector, createReducer, createSelector, on} from "@ngrx/store";
import {DiagnosticsActions} from "../actions";
import {AppState} from "../../app.state";
import {selectHubStatus} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "../effects/diagnostics-hub";

export const DIAGNOSTICS_FEATURE_KEY = 'diagnostics';

const INITIAL_STATE: DiagnosticsState = { messages: [] };
const reducer = createReducer(INITIAL_STATE,
  on(DiagnosticsActions.messageReceived, (state, {payload}) => ({...state, messages: [...state.messages, payload]}))
);

export function diagnosticsReducer(state: DiagnosticsState | undefined, action: Action): DiagnosticsState {
  return reducer(state, action);
}


const selectDiagnosticsState = createFeatureSelector<DiagnosticsState>(DIAGNOSTICS_FEATURE_KEY);
export const selectDiagnosticsMessages = createSelector(selectDiagnosticsState, s => s.messages);
export const selectDiagnosticsHubState = (state:AppState) => selectHubStatus(state, DIAGNOSTICS_HUB);
export const selectIsDiagnosticHubConnected = createSelector(selectDiagnosticsHubState, s => s.state === 'connected');
