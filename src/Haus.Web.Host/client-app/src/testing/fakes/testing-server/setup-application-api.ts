import {ApplicationPackageModel, ApplicationVersionModel} from "../../../app/shared/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet} from "./setup-http";
import {ModelFactory} from "../../model-factory";
import {HttpStatusCodes} from "../../../app/shared/rest-api";

const BASE_URL = '/api/application/latest-version';

export function setupGetLatestVersion(model: ApplicationVersionModel, options?: RequestOptions) {
  setupHttpGet(BASE_URL, model, options);
}

export function setupFailedLatestVersion(options?: RequestOptions) {
  setupHttpGet(BASE_URL, null, {...(options || {}), status: HttpStatusCodes.InternalServerError});
}

export function setupGetLatestPackages(packages: Array<ApplicationPackageModel>, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/packages`, ModelFactory.createListResult(...packages), options);
}

export function setupFailedLatestPackages(options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/packages`, null, {...(options || {}), status: HttpStatusCodes.InternalServerError});
}

export function setupDownloadPackage(packageId: number, blob: Blob, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/packages/${packageId}/download`, blob, options);
}

export function setupFailedDownloadPackage(packageId: number, options?: RequestOptions) {
  setupHttpGet(`${BASE_URL}/packages/${packageId}/download`, null, {...(options || {}), status: HttpStatusCodes.InternalServerError});
}
