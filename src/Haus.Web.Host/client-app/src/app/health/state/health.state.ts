import {HausHealthReportModel, LogEntryModel} from "../../shared/models";

export interface HealthState {
  report: HausHealthReportModel | null;
  logs: Array<LogEntryModel>;
}
