import {DevicesState} from "./devices/state";
import {DiagnosticsState} from "./diagnostics/state";
import {RoomsState} from "./rooms/state";
import {LoadingState} from "./shared/loading/loading.state";

export interface AppState {
  devices: DevicesState;
  diagnostics: DiagnosticsState;
  rooms: RoomsState;
  loading: LoadingState;
}
