import {v4 as uuid} from 'uuid';
import {MqttDiagnosticsMessageModel} from "../app/diagnostics/models/mqtt-diagnostics-message.model";

function createMqttDiagnosticsMessage(model: Partial<MqttDiagnosticsMessageModel> = {}): MqttDiagnosticsMessageModel {
  return {
    id: model.id || uuid(),
    topic: model.topic || uuid(),
    payload: model.payload || uuid(),
    replayError: model.replayError,
    isReplaying: model.isReplaying
  };
}

export const ModelFactory = {
  createMqttDiagnosticsMessage
};
