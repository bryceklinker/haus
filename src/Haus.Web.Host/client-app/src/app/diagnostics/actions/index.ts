import {createAction} from "@ngrx/store";
import {MqttDiagnosticsMessageModel} from "../models/mqtt-diagnostics-message.model";

export const DiagnosticsActions = {
  initEffects: createAction('[Diagnostics] Init Effects'),
  messageReceived: createAction('[Diagnostics] Message Received', (message: MqttDiagnosticsMessageModel) => ({payload: message}))
}
