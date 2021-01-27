import {LogEntryModel} from "../../../app/shared/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet} from "./setup-http";
import {ModelFactory} from "../../model-factory";
import {HttpStatusCodes} from "../../../app/shared/rest-api";

export function setupGetLogs(logs: Array<LogEntryModel> = [], options?: RequestOptions) {
  setupHttpGet('/api/logs', ModelFactory.createListResult(...logs), options);
}

export function setupGetLogsFailure(options?: RequestOptions) {
  setupHttpGet('/api/logs', null, {...options, status: HttpStatusCodes.InternalServerError});
}
