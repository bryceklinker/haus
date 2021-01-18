import {createAction} from "@ngrx/store";
import {HausHealthReportModel, ListResult, LogEntryModel} from "../../shared/models";
import {createAsyncActionSet} from "../../shared/actions";

export const HealthActions = {
  start: createAction('[Health] Start'),
  stop: createAction('[Health] Stop'),
  connected: createAction('[Health] Connected'),
  disconnected: createAction('[Health] Disconnected'),
  healthReceived: createAction('[Health] Health Received', (model: HausHealthReportModel) => ({payload: model})),
  loadRecentLogs: createAsyncActionSet(
    '[Health] Load Recent Logs',
    () => ({payload: 'ignore'}),
    (result: ListResult<LogEntryModel>) => ({payload: result.items}),
    (error: any) => ({payload: error})
  )
};
