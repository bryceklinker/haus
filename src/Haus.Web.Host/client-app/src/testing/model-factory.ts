import {v4 as uuid} from 'uuid';
import {DiagnosticsMessageModel} from "../app/diagnostics/models";
import {DeviceModel} from "../app/devices/models";
import {ListResult} from "../app/shared/models";

let id = 0;

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

function createDeviceModel(model: Partial<DeviceModel> = {}): DeviceModel {
  return {
    id: model.id || ++id,
    externalId: model.externalId || uuid(),
    metadata: model.metadata || []
  }
}

function createListResult<T>(...items: Array<T>): ListResult<T> {
  return {
    items: items,
    count: items.length
  }
}

export const ModelFactory = {
  createMqttDiagnosticsMessage,
  createDeviceModel,
  createListResult,
};
