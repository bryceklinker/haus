import {ActionReducerMap} from "@ngrx/store";
import {AppState} from "./app.state";
import {signalrReducer} from "ngrx-signalr-core";
import {loadingReducer} from "./shared/loading/loading.reducer";
import {routerReducer} from "@ngrx/router-store";
import {DIAGNOSTICS_FEATURE_KEY, diagnosticsReducer} from "./diagnostics/reducers/diagnostics.reducer";
import {DEVICES_FEATURE_KEY, devicesReducer} from "./devices/reducers/devices.reducer";

export const appReducerMap: ActionReducerMap<AppState> =  {
  signalr: signalrReducer,
  loading: loadingReducer,
  router: routerReducer,
  [DIAGNOSTICS_FEATURE_KEY]: diagnosticsReducer,
  [DEVICES_FEATURE_KEY]: devicesReducer
};
