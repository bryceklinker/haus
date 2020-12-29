import {RequestOptions} from "./request-options";
import {setupHttpPost} from "./setup-http";

const BASE_URL = '/api/diagnostics';

export function setupDiagnosticsReplay(options?: RequestOptions) {
  setupHttpPost(`${BASE_URL}/replay`, null, options);
}
