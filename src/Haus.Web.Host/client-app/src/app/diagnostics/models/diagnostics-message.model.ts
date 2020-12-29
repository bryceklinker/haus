export interface DiagnosticsMessageModel {
  id: string,
  topic: string;
  payload: any;
  timestamp: string;
  replayError?: any;
  isReplaying?: boolean;
}
