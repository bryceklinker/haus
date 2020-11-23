import {signalrReducer} from "ngrx-signalr-core";
import {DiagnosticsState} from "./diagnostics/diagnostics.state";
import {LoadingState} from "./shared/loading/loading.state";
import {DevicesState} from "./devices/devices.state";

export interface AppState {
  signalr: ReturnType<typeof signalrReducer>;
  diagnostics: DiagnosticsState;
  loading: LoadingState;
  devices: DevicesState
}
