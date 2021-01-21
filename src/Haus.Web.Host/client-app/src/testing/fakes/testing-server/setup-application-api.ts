import {ApplicationVersionModel} from "../../../app/shared/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet} from "./setup-http";

const BASE_URL = '/api/application';

export function setupGetLatestVersion(model: ApplicationVersionModel, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/latest-version`, model, options);
}
