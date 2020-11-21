export interface MqttDiagnosticsMessageModel {
  id: string,
  topic: string;
  payload: any;
  replayError?: any;
  isReplaying?: boolean;
}
