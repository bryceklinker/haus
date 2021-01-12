import {RequestOptions} from "./request-options";
import {setupHttpGet, setupHttpPost, setupHttpPut} from "./setup-http";
import {ModelFactory} from "../../model-factory";
import {DeviceModel} from "../../../app/shared/models";

const BASE_URL = '/api/devices';

export function setupGetAllDevices(devices: Array<DeviceModel> = [], options?: RequestOptions) {
  setupHttpGet(BASE_URL, ModelFactory.createListResult(...devices), options);
}

export function setupUpdateDevice(deviceId: number, options?: RequestOptions) {
  setupHttpPut(`${BASE_URL}/${deviceId}`, options);
}

export function setupDeviceTurnOff(deviceId: string | number, options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/${deviceId}/turn-off`, null, options);
}

export function setupDeviceTurnOn(deviceId: string | number, options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/${deviceId}/turn-on`, null, options);
}

export function setupGetAllDeviceTypes(deviceTypes: Array<string> = [], options?: RequestOptions) {
  setupHttpGet('/api/device-types', ModelFactory.createListResult(...deviceTypes), options);
}

export function setupGetAllLightTypes(lightTypes: Array<string> = [], options?: RequestOptions) {
  setupHttpGet('/api/light-types', ModelFactory.createListResult(...lightTypes), options);
}

export function setupChangeDeviceLightingConstraints(deviceId: number, options?: RequestOptions) {
  setupHttpPut(`${BASE_URL}/${deviceId}/lighting-constraints`, null, options);
}
