import {RequestOptions} from "./request-options";
import {setupHttpGet, setupHttpPost} from "./setup-http";
import {DiscoveryModel} from "../../../app/shared/models";

const BASE_URL = '/api/discovery';

export function setupStartDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/start`, null, options);
}

export function setupStopDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/stop`, null, options);
}

export function setupSyncDiscovery(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/sync`, null, options);
}

export function setupGetDiscovery(model: Partial<DiscoveryModel> = {}, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/state`, model, options);
}
