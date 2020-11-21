import {v4 as uuid} from 'uuid';
import {DiagnosticsMessageModel} from "../app/diagnostics/models";

function createMqttDiagnosticsMessage(model: Partial<DiagnosticsMessageModel> = {}): DiagnosticsMessageModel {
  return {
    id: model.id || uuid(),
    topic: model.topic || uuid(),
    payload: model.payload || uuid(),
    timestamp: model.timestamp || new Date().toISOString(),
    replayError: model.replayError,
    isReplaying: model.isReplaying
  };
}

export const ModelFactory = {
  createMqttDiagnosticsMessage
};
