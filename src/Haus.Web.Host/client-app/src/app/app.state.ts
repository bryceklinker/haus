import {DevicesState} from "./devices/state";
import {DiagnosticsState} from "./diagnostics/state";
import {RoomsState} from "./rooms/state";
import {LoadingState} from "./shared/loading";
import {DeviceSimulatorState} from "./device-simulator/state";
import {DeviceTypesState} from "./devices/state/device-types.state";

export interface AppState {
  devices: DevicesState;
  diagnostics: DiagnosticsState;
  rooms: RoomsState;
  loading: LoadingState;
  deviceSimulator: DeviceSimulatorState;
  deviceTypes: DeviceTypesState;
}
