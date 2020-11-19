import {MqttDiagnosticsMessageModel} from "./models/mqtt-diagnostics-message.model";
import {HubConnectionState} from "@microsoft/signalr";

export interface DiagnosticsState {
  connectionStatus: HubConnectionState;
  messages: Array<MqttDiagnosticsMessageModel>;
}
