import {createEntityAdapter} from "@ngrx/entity";

import {createComparer, SortDirection} from "../../shared/sort-array-by";
import {DiagnosticsState} from "./diagnostics.state";
import {Action, createReducer, createSelector, on} from "@ngrx/store";
import {AppState} from "../../app.state";
import {DiagnosticsActions} from "./actions";
import {UiMqttDiagnosticsMessageModel} from "../../shared/models";

const adapter = createEntityAdapter<UiMqttDiagnosticsMessageModel>({
  selectId: msg => msg.id,
  sortComparer: createComparer<UiMqttDiagnosticsMessageModel>(d => d.timestamp, SortDirection.Descending)
});

const initialState: DiagnosticsState = adapter.getInitialState({
  isConnected: false
});
const reducer = createReducer(initialState,
  on(DiagnosticsActions.messageReceived, (state, {payload}) => adapter.upsertOne(payload, state)),
  on(DiagnosticsActions.replayMessage.request, (state, {payload}) => adapter.upsertOne({...payload, isReplaying: true}, state)),
  on(DiagnosticsActions.replayMessage.success, (state, {payload}) => adapter.upsertOne({...payload, isReplaying: false}, state)),
  on(DiagnosticsActions.connected, (state) => ({...state, isConnected: true})),
  on(DiagnosticsActions.disconnected, (state) => ({...state, isConnected: false})),
);

export function diagnosticsReducer(state: DiagnosticsState | undefined, action: Action) {
  return reducer(state, action);
}

const {
  selectAll
} = adapter.getSelectors();

const selectDiagnosticsState = (state: AppState) => state.diagnostics;

export const selectAllDiagnosticMessages = createSelector(selectDiagnosticsState, selectAll);
export const selectIsDiagnosticsConnected = createSelector(selectDiagnosticsState, s => s.isConnected);
