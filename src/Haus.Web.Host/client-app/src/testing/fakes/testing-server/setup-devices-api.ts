import {DeviceModel} from "../../../app/devices/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet, setupHttpPost} from "./setup-http";
import {ModelFactory} from "../../model-factory";

const BASE_URL = '/api/devices';

export function setupGetAllDevices(devices: Array<DeviceModel> = [], options?: RequestOptions) {
  setupHttpGet(BASE_URL, ModelFactory.createListResult(...devices), options);
}

export function setupDevicesStartDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/start-discovery`, null, options);
}

export function setupDevicesStopDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/stop-discovery`, null, options);
}

export function setupDevicesSyncDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/sync-discovery`, null, options);
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
