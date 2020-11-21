import {createAction} from "@ngrx/store";
import {MqttDiagnosticsMessageModel} from "../models/mqtt-diagnostics-message.model";

export const DiagnosticsActions = {
  initEffects: createAction('[Diagnostics] Init Effects'),
  messageReceived: createAction('[Diagnostics] Message Received', (message: MqttDiagnosticsMessageModel) => ({payload: message})),
  replayMessageRequest: createAction('[Diagnostics] Replay Message Request', (message: MqttDiagnosticsMessageModel) => ({payload: message})),
  replayMessageSuccess: createAction('[Diagnostics] Replay Message Success'),
  replayMessageFailed: createAction('[Diagnostics] Replay Message Failed', (message: MqttDiagnosticsMessageModel, error: any) => ({payload: {message, error}}))
}
