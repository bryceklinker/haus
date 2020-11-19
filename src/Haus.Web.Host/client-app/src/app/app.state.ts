import {signalrReducer} from "ngrx-signalr-core";
import {DiagnosticsState} from "./diagnostics/diagnostics.state";

export interface AppState {
  signalr: ReturnType<typeof signalrReducer>;
  diagnostics: DiagnosticsState
}
