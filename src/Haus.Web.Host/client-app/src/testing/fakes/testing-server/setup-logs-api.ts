import {LogEntryModel} from "../../../app/shared/models";
import {RequestOptions} from "./request-options";
import {setupHttpGet} from "./setup-http";
import {ModelFactory} from "../../model-factory";

export function setupGetLogs(logs: Array<LogEntryModel> = [], options?: RequestOptions) {
  setupHttpGet('/api/logs', ModelFactory.createListResult(...logs), options);
}
