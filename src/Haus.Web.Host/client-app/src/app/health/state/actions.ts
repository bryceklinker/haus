import {createAction} from "@ngrx/store";
import {HausHealthReportModel} from "../../shared/models";

export const HealthActions = {
  start: createAction('[Health] Start'),
  stop: createAction('[Health] Stop'),
  connected: createAction('[Health] Connected'),
  disconnected: createAction('[Health] Disconnected'),
  healthReceived: createAction('[Health] Health Received', (model: HausHealthReportModel) => ({payload: model}))
};