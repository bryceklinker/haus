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
    id: uuid(),
    topic: uuid(),
    payload: <any>uuid(),
    timestamp: new Date().toISOString(),
    replayError: null,
    isReplaying: false,
    ...model
  };
}

function createDeviceModel(model: Partial<DeviceModel> = {}): DeviceModel {
  return {
    id: model.id || ++id,
    name: uuid(),
    externalId: uuid(),
    metadata: [],
    deviceType: DeviceType.Unknown,
    lightType: LightType.None,
    roomId: undefined,
    lighting: createLighting(),
    ...model
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
    name: uuid(),
    lighting: createLighting(),
    occupancyTimeoutInSeconds: 300,
    ...model
  };
}

function createSimulatedDevice(model: Partial<SimulatedDeviceModel> = {}): SimulatedDeviceModel {
  return {
    deviceType: DeviceType.Unknown,
    id: uuid(),
    lighting: createLighting(),
    isOccupied: model.isOccupied !== undefined ? model.isOccupied : false,
    metadata: [],
    ...model,
  }
}

function createMetadata(key: string, value: string): MetadataModel {
  return { key, value };
}

function createColorLighting(model: Partial<ColorLightingModel> = {}): ColorLightingModel {
  return {
    red: 54,
    green: 12,
    blue: 89,
    ...model
  }
}

function createLevelLighting(model: Partial<LevelLightingModel> = {}): LevelLightingModel {
  return {
    value: 100,
    min: 0,
    max: 100,
    ...model
  }
}

function createTemperatureLighting(model: Partial<TemperatureLightingModel> = {}): TemperatureLightingModel {
  return {
    value: 3000,
    min: 2000,
    max: 6000,
    ...model
  }
}

function createLighting(model: Partial<LightingModel> = {}): LightingModel {
  return {
    state: LightingState.On,
    level: createLevelLighting(),
    ...model,
  }
}

function createLightingConstraints(model: Partial<LightingConstraintsModel> = {}): LightingConstraintsModel {
  return {
    minLevel: 0,
    maxLevel: 100,
    ...model
  }
}

function createDiscovery(model: Partial<DiscoveryModel> = {}): DiscoveryModel {
  return {
    state: DiscoveryState.Disabled,
    ...model
  };
}

function createHealthCheckModel(model: Partial<HausHealthCheckModel> = {}): HausHealthCheckModel {
  return {
    durationOfCheckInMilliseconds: 500,
    durationOfCheckInSeconds: 0.5,
    isError: model.isError !== undefined ? model.isError : false,
    isWarn: model.isWarn !== undefined ? model.isWarn : false,
    isOk: model.isOk !== undefined ? model.isOk : true,
    name: uuid(),
    status: HealthStatus.Healthy,
    tags: [],
    ...model
  }
}

function createHealthReportModel(model: Partial<HausHealthReportModel> = {}): HausHealthReportModel {
  return {
    durationOfCheckInMilliseconds: 1000,
    durationOfCheckInSeconds: 1,
    isError: model.isError !== undefined ? model.isError : false,
    isWarn: model.isWarn !== undefined ? model.isWarn : false,
    isOk: model.isOk !== undefined ? model.isOk : true,
    status: HealthStatus.Healthy,
    checks: [],
    ...model
  }
}

function createHausEvent<T = any>(model: Partial<HausEvent<T>> = {}): HausEvent<T> {
  return {
    type: '',
    timestamp: new Date().toISOString(),
    payload: null as any,
    isEvent: model.isEvent === undefined ? model.isEvent : true,
    ...model
  }
}

function createLogEntry(model: Partial<LogEntryModel> = {}): LogEntryModel {
  return {
    level: 'Information',
    timestamp: new Date().toISOString(),
    message: uuid(),
    value: {},
    ...model
  }
}

function createApplicationVersion(model: Partial<ApplicationVersionModel> = {}): ApplicationVersionModel {
  return {
    creationDate: new Date().toISOString(),
    isNewer: model.isNewer !== undefined ? model.isNewer : false,
    isOfficialRelease: model.isOfficialRelease !== undefined ? model.isOfficialRelease : false,
    version: '0.0.0',
    description: uuid(),
    ...model
  }
}

function createApplicationPackage(model: Partial<ApplicationPackageModel> = {}): ApplicationPackageModel {
  return {
    id: 76,
    name: uuid(),
    ...model
  }
}

function createUser(model: Partial<UserModel> = {}): UserModel {
  return {
    name: uuid(),
    email: `${uuid()}@haus.com`,
    picture: `https://${uuid()}.png`,
    ...model
  }
}

function createMany<T>(factory: () => T, count: number): Array<T> {
  return Array.from({length: count}).map(() => factory());
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
  createUser,
  createMany
};
