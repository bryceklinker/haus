import {v4 as uuid} from 'uuid';
import {LightingColorModel, LightingModel, LightingState, ListResult, MetadataModel} from "../app/shared/models";
import {DiagnosticsMessageModel} from "../app/diagnostics/models";
import {DeviceModel} from "../app/devices/models";
import {RoomModel} from "../app/rooms/models";
import {SimulatedDeviceModel} from "../app/device-simulator/models";
import {LightingConstraintsModel} from "../app/shared/models/lighting-constraints.model";

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
    name: model.name || uuid(),
    lighting: model.lighting || createLighting()
  };
}

function createSimulatedDevice(model: Partial<SimulatedDeviceModel> = {}): SimulatedDeviceModel {
  return {
    deviceType: model.deviceType || 'Unknown',
    id: model.id || uuid(),
    metadata: [
      ...(model.metadata || []),
    ]
  }
}

function createMetadata(key: string, value: string): MetadataModel {
  return { key, value };
}

function createLightingColor(model: Partial<LightingColorModel> = {}): LightingColorModel {
  return {
    red: model.red || 54,
    green: model.green || 12,
    blue: model.blue || 89
  }
}

function createLightingConstraints(model: Partial<LightingConstraintsModel> = {}): LightingConstraintsModel {
  return {
    minLevel: model.minLevel || 0,
    maxLevel: model.maxLevel || 100,
    minTemperature: model.minTemperature || 2000,
    maxTemperature: model.maxTemperature || 4000
  }
}

function createLighting(model: Partial<LightingModel> = {}): LightingModel {
  return {
    level: model.level || 23,
    color: model.color || createLightingColor(),
    state: model.state || LightingState.On,
    temperature: model.temperature || 2900,
    constraints: model.constraints || createLightingConstraints()
  }
}

export const ModelFactory = {
  createMqttDiagnosticsMessage,
  createDeviceModel,
  createRoomModel,
  createListResult,
  createSimulatedDevice,
  createMetadata,
  createLightingColor,
  createLightingConstraints,
  createLighting
};
