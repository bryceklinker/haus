import {createAction} from "@ngrx/store";
import {createAsyncActionSet} from "../../shared/actions";
import {MqttDiagnosticsMessageModel, UiMqttDiagnosticsMessageModel} from "../../shared/models";

export const DiagnosticsActions = {
  start: createAction('[Diagnostics] Start'),
  stop: createAction('[Diagnostics] Stop'),
  connected: createAction('[Diagnostics] Connected'),
  disconnected: createAction('[Diagnostics] Disconnected'),
  messageReceived: createAction('[Diagnostics] Message Received', (msg: MqttDiagnosticsMessageModel) => ({payload: msg})),

  replayMessage: createAsyncActionSet(
    '[Diagnostics] Replay Message',
    (msg: UiMqttDiagnosticsMessageModel) => ({payload: msg}),
    (msg: UiMqttDiagnosticsMessageModel) => ({payload: msg}),
    (msg: UiMqttDiagnosticsMessageModel, err: any) => ({payload: {message: msg, error: err}})
  )
}
