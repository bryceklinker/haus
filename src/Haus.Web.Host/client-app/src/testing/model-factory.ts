import {v4 as uuid} from 'uuid';
import {
  ColorLightingModel,
  DeviceModel,
  DeviceType,
  DiscoveryModel,
  DiscoveryState,
  LevelLightingModel,
  LightingModel,
  LightingState,
  LightType,
  ListResult,
  MetadataModel,
  RoomModel,
  SimulatedDeviceModel,
  TemperatureLightingModel,
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
    lightType: model.lightType || LightType.None,
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

function createColorLighting(model: Partial<ColorLightingModel> = {}): ColorLightingModel {
  return {
    red: model.red === undefined ? 54 : model.red,
    green: model.green === undefined ? 12 : model.green,
    blue: model.blue === undefined ? 89 : model.blue
  }
}

function createLevelLighting(model: Partial<LevelLightingModel> = {}): LevelLightingModel {
  return {
    value: model.value === undefined ? 100 : model.value,
    min: model.min === undefined ? 100 : model.min,
    max: model.max === undefined ? 0 : model.max
  }
}

function createTemperatureLighting(model: Partial<TemperatureLightingModel> = {}): TemperatureLightingModel {
  return {
    value: model.value === undefined ? 3000 : model.value,
    min: model.min === undefined ? 2000 : model.min,
    max: model.max === undefined ? 6000 : model.max
  }
}

function createLighting(model: Partial<LightingModel> = {}): LightingModel {
  return {
    state: model.state === undefined ? LightingState.On : model.state,
    level: model.level || createLevelLighting(),
    temperature: model.temperature,
    color: model.color
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
  createColorLighting,
  createLevelLighting,
  createTemperatureLighting,
  createLighting,
  createDiscovery
};
