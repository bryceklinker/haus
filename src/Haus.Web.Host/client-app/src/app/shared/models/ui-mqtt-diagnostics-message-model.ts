import {MqttDiagnosticsMessageModel} from "./generated";

export interface UiMqttDiagnosticsMessageModel extends MqttDiagnosticsMessageModel {
  payload: any;
  replayError?: any;
  isReplaying?: boolean;
}
