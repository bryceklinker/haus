import {createAction} from "@ngrx/store";
import {DiagnosticsMessageModel} from "../models";
import {createAsyncAction} from "../../shared/actions";
import {DevicesActions} from "../../devices/actions";

export const DiagnosticsActions = {
  initHub: createAction('[Diagnostics] Init Effects'),
  messageReceived: createAction('[Diagnostics] Message Received', (message: DiagnosticsMessageModel) => ({payload: message})),
  replay: createAsyncAction('[Diagnostics] Replay Message',
    (message: DiagnosticsMessageModel) => ({payload: message}),
    (message: DiagnosticsMessageModel) => ({payload: message}),
    (message: DiagnosticsMessageModel, error: any) => ({payload: {message, error}})
  ),
  startDiscovery: DevicesActions.startDiscovery,
  stopDiscovery: DevicesActions.stopDiscovery,
  syncDiscovery: DevicesActions.syncDiscovery,
}
