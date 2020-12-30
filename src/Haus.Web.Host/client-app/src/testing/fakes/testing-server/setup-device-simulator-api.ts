import {RequestOptions} from "./request-options";
import {setupHttpPost} from "./setup-http";

export function setupAddSimulatedDevice(options?: RequestOptions) {
  setupHttpPost('/api/device-simulator/devices', null, options);
}
