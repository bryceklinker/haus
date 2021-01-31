import {v4 as uuid} from 'uuid';
import {
  ApplicationPackageModel,
  ApplicationVersionModel,
  ColorLightingModel,
  DeviceModel,
  DeviceType,
  DiscoveryModel,
  DiscoveryState, HausEvent,
  HausHealthCheckModel,
  HausHealthReportModel,
  HealthStatus,
  LevelLightingModel,
  LightingConstraintsModel,
  LightingModel,
  LightingState,
  LightType,
  ListResult, LogEntryModel,
  MetadataModel,
  RoomModel,
  SimulatedDeviceModel,
  TemperatureLightingModel,
  UiMqttDiagnosticsMessageModel
} from "../app/shared/models";
import {UserModel} from "../app/shared/auth";

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
    isOccupied: model.isOccupied !== undefined ? model.isOccupied : false,
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

function createLightingConstraints(model: Partial<LightingConstraintsModel> = {}): LightingConstraintsModel {
  return {
    minLevel: model.minLevel || 0,
    maxLevel: model.maxLevel || 100
  }
}

function createDiscovery(model: Partial<DiscoveryModel> = {}): DiscoveryModel {
  return {
    state: model.state || DiscoveryState.Disabled
  };
}

function createHealthCheckModel(model: Partial<HausHealthCheckModel> = {}): HausHealthCheckModel {
  return {
    description: model.description,
    durationOfCheckInMilliseconds: model.durationOfCheckInMilliseconds || 500,
    durationOfCheckInSeconds: model.durationOfCheckInSeconds || 0.5,
    isError: model.isError !== undefined ? model.isError : false,
    isWarn: model.isWarn !== undefined ? model.isWarn : false,
    isOk: model.isOk !== undefined ? model.isOk : true,
    exceptionMessage: model.exceptionMessage,
    name: model.name || uuid(),
    status: model.status || HealthStatus.Healthy,
    tags: model.tags || []
  }
}

function createHealthReportModel(model: Partial<HausHealthReportModel> = {}): HausHealthReportModel {
  return {
    durationOfCheckInMilliseconds: model.durationOfCheckInMilliseconds || 1000,
    durationOfCheckInSeconds: model.durationOfCheckInSeconds || 1,
    isError: model.isError !== undefined ? model.isError : false,
    isWarn: model.isWarn !== undefined ? model.isWarn : false,
    isOk: model.isOk !== undefined ? model.isOk : true,
    status: model.status || HealthStatus.Healthy,
    checks: model.checks || []
  }
}

function createHausEvent<T = any>(model: Partial<HausEvent<T>> = {}): HausEvent<T> {
  return {
    type: model.type || '',
    timestamp: model.timestamp || new Date().toISOString(),
    payload: model.payload || null as any,
    isEvent: model.isEvent === undefined ? model.isEvent : true
  }
}

function createLogEntry(model: Partial<LogEntryModel> = {}): LogEntryModel {
  return {
    level: model.level || 'Information',
    timestamp: model.timestamp || new Date().toISOString(),
    message: model.message || uuid(),
    value: model.value || {}
  }
}

function createApplicationVersion(model: Partial<ApplicationVersionModel> = {}): ApplicationVersionModel {
  return {
    creationDate: model.creationDate || new Date().toISOString(),
    isNewer: model.isNewer !== undefined ? model.isNewer : false,
    isOfficialRelease: model.isOfficialRelease !== undefined ? model.isOfficialRelease : false,
    version: model.version || '0.0.0',
    description: model.description || uuid()
  }
}

function createApplicationPackage(model: Partial<ApplicationPackageModel> = {}): ApplicationPackageModel {
  return {
    id: model.id || 76,
    name: model.name || uuid()
  }
}

function createUser(model: Partial<UserModel> = {}): UserModel {
  return {
    name: model.name || uuid(),
    email: model.email || `${uuid()}@haus.com`,
    picture: model.picture || `https://${uuid()}.png`
  }
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
  createLightingConstraints,
  createLighting,
  createDiscovery,
  createHealthCheckModel,
  createHealthReportModel,
  createHausEvent,
  createLogEntry,
  createApplicationVersion,
  createApplicationPackage,
  createUser
};
