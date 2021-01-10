import {v4 as uuid} from 'uuid';
import {
  DeviceModel,
  DeviceType,
  DiscoveryModel,
  DiscoveryState,
  LightingColorModel,
  LightingConstraintsModel,
  LightingModel,
  LightingState,
  ListResult,
  MetadataModel,
  RoomModel,
  SimulatedDeviceModel,
  UiMqttDiagnosticsMessageModel
} from "../app/shared/models";

let id = 0;

function createMqttDiagnosticsMessage(model: Partial<UiMqttDiagnosticsMessageModel> = {}): UiMqttDiagnosticsMessageModel {
  return {
    id: model.id || uuid(),
    topic: model.topic || uuid(),
    payload: model.payload || <any>uuid(),
    timestamp: model.timestamp || new Date().toISOString(),
    replayError: model.replayError || null,
    isReplaying: model.isReplaying || false
  };
}

function createDeviceModel(model: Partial<DeviceModel> = {}): DeviceModel {
  return {
    id: model.id || ++id,
    name: model.name || uuid(),
    externalId: model.externalId || uuid(),
    metadata: model.metadata || [],
    deviceType: model.deviceType || DeviceType.Unknown,
    roomId: model.roomId || undefined,
    lighting: model.lighting || createLighting()
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
    deviceType: model.deviceType || DeviceType.Unknown,
    id: model.id || uuid(),
    lighting: model.lighting || createLighting(),
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

function createDiscovery(model: Partial<DiscoveryModel> = {}): DiscoveryModel {
  return {
    state: model.state || DiscoveryState.Disabled
  };
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
  createLighting,
  createDiscovery
};
