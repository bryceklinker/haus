import {RequestOptions} from "./request-options";
import {setupHttpPost} from "./setup-http";

const BASE_URL = '/api/device-simulator/devices';

export function setupAddSimulatedDevice(options?: RequestOptions) {
  setupHttpPost(BASE_URL, null, options);
}

export function setupTriggerSimulatedDeviceOccupancyChange(deviceId: string, options?: RequestOptions){
  setupHttpPost(`${BASE_URL}/${deviceId}/trigger-occupancy-change`, null, options);
}
