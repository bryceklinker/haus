import {v4 as uuid} from 'uuid';
import {ListResult} from "../app/shared/models";
import {DiagnosticsMessageModel} from "../app/shared/diagnostics";
import {DeviceModel} from "../app/shared/devices";
import {RoomModel} from "../app/shared/rooms";
import {DeviceSimulatorStateModel} from "../app/device-simulator/models";

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
    deviceType: model.deviceType || 'Unknown'
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

function createDeviceSimulatorState(model: Partial<DeviceSimulatorStateModel> = {}): DeviceSimulatorStateModel {
  const deviceOne = createDeviceModel();
  const deviceTwo = createDeviceModel();
  const deviceThree = createDeviceModel();

  return {
    devices: model.devices ?? [deviceOne, deviceTwo, deviceThree],
    devicesById: model.devicesById ?? {
      [deviceOne.externalId]: deviceOne,
      [deviceTwo.externalId]: deviceTwo,
      [deviceThree.externalId]: deviceThree
    }
  }
}

export const ModelFactory = {
  createMqttDiagnosticsMessage,
  createDeviceModel,
  createRoomModel,
  createListResult,
  createDeviceSimulatorState
};
