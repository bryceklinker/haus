import {createAction} from "@ngrx/store";
import {HubConnectionState} from "@microsoft/signalr";
import {MqttDiagnosticsMessageModel} from "../models/mqtt-diagnostics-message.model";

export const DiagnosticsActions = {
  connectionStateChanged: createAction('[Diagnostics] State', (state: HubConnectionState) => ({payload: state})),
  messageReceived: createAction('[Diagnostics] Message Received', (message: MqttDiagnosticsMessageModel) => ({payload: message}))
}
