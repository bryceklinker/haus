import {MqttDiagnosticsMessageModel} from "./models/mqtt-diagnostics-message.model";

export interface DiagnosticsState {
  messages: Array<MqttDiagnosticsMessageModel>;
}
