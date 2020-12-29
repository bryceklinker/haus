import {ActionReducerMap} from "@ngrx/store";
import {AppState} from "../app.state";
import {devicesReducer} from "../devices/state";
import {diagnosticsReducer} from "../diagnostics/state";
import {roomsReducer} from "../rooms/state";
import {loadingReducer} from "./loading/loading.reducer";

export const sharedReducerMap: ActionReducerMap<AppState> = {
  devices: devicesReducer,
  diagnostics: diagnosticsReducer,
  rooms: roomsReducer,
  loading: loadingReducer
};
