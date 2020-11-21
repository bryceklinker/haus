import {createAction} from "@ngrx/store";
import {DiagnosticsMessageModel} from "../models";

export const DiagnosticsActions = {
  initEffects: createAction('[Diagnostics] Init Effects'),
  messageReceived: createAction('[Diagnostics] Message Received', (message: DiagnosticsMessageModel) => ({payload: message})),
  replayMessageRequest: createAction('[Diagnostics] Replay Message Request', (message: DiagnosticsMessageModel) => ({payload: message})),
  replayMessageSuccess: createAction('[Diagnostics] Replay Message Success'),
  replayMessageFailed: createAction('[Diagnostics] Replay Message Failed', (message: DiagnosticsMessageModel, error: any) => ({payload: {message, error}}))
}
