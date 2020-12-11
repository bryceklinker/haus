import {signalrReducer} from "ngrx-signalr-core";
import {DiagnosticsState} from "./diagnostics/diagnostics.state";
import {LoadingState} from "./shared/loading/loading.state";
import {DevicesState} from "./devices/devices.state";
import {RouterReducerState} from "@ngrx/router-store";
import {Params} from "@angular/router";

export interface RouterState {
  url: string;
  queryParams: Params;
  params: Params;
}

export interface AppState {
  signalr: ReturnType<typeof signalrReducer>;
  diagnostics: DiagnosticsState;
  loading: LoadingState;
  devices: DevicesState,
  router: RouterReducerState<RouterState>
}
