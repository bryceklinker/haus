import {ActionReducerMap} from "@ngrx/store";
import {AppState} from "./app.state";
import {devicesReducer} from "./devices/state";
import {diagnosticsReducer} from "./diagnostics/state";
import {roomsReducer} from "./rooms/state";
import {loadingReducer} from "./shared/loading";

export const appReducerMap: ActionReducerMap<AppState> = {
  devices: devicesReducer,
  diagnostics: diagnosticsReducer,
  rooms: roomsReducer,
  loading: loadingReducer
};
