import {ApplicationPackageModel, ApplicationVersionModel} from "../../../app/shared/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet} from "./setup-http";
import {ModelFactory} from "../../model-factory";

const BASE_URL = '/api/application';

export function setupGetLatestVersion(model: ApplicationVersionModel, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/latest-version`, model, options);
}

export function setupGetLatestPackages(packages: Array<ApplicationPackageModel>, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/latest-version/packages`, ModelFactory.createListResult(...packages), options);
}
