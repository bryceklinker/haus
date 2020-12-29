import {v4 as uuid} from 'uuid';
import {ListResult} from "../app/shared/models";
import {DiagnosticsMessageModel} from "../app/diagnostics/models";
import {DeviceModel} from "../app/devices/models";
import {RoomModel} from "../app/rooms/models";

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
    name: model.name || uuid(),
    externalId: model.externalId || uuid(),
    metadata: model.metadata || [],
    deviceType: model.deviceType || 'Unknown',
    roomId: model.roomId || undefined
  }
}

function createListResult<T>(...items: Array<T>): ListResult<T> {
  return {
    items: items,
    count: items.length
  }
}

function createRoomModel(model: Partial<RoomModel> = {}): RoomModel {
  return {
    id: model.id || ++id,
    name: model.name || uuid()
  };
}

export const ModelFactory = {
  createMqttDiagnosticsMessage,
  createDeviceModel,
  createRoomModel,
  createListResult
};
