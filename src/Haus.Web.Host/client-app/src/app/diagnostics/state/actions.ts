import {createAction} from "@ngrx/store";
import {DiagnosticsMessageModel} from "../models";
import {createAsyncActionSet} from "../../shared/actions/create-async-action-set";
import {DevicesActions} from "../../devices/state";

export const DiagnosticsActions = {
  start: createAction('[Diagnostics] Start'),
  stop: createAction('[Diagnostics] Stop'),
  connected: createAction('[Diagnostics] Connected'),
  disconnected: createAction('[Diagnostics] Disconnected'),
  messageReceived: createAction('[Diagnostics] Message Received', (msg: DiagnosticsMessageModel) => ({payload: msg})),

  replayMessage: createAsyncActionSet(
    '[Diagnostics] Replay Message',
    (msg: DiagnosticsMessageModel) => ({payload: msg}),
    (msg: DiagnosticsMessageModel) => ({payload: msg}),
    (msg: DiagnosticsMessageModel, err: any) => ({payload: {message: msg, error: err}})
  ),

  startDiscovery: DevicesActions.startDiscovery,
  stopDiscovery: DevicesActions.stopDiscovery,
  syncDiscovery: DevicesActions.syncDiscovery,
}
