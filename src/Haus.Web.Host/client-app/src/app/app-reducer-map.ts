import {ActionReducerMap} from "@ngrx/store";
import {AppState} from "./app.state";
import {devicesReducer, deviceTypesReducer} from "./devices/state";
import {diagnosticsReducer} from "./diagnostics/state";
import {roomsReducer} from "./rooms/state";
import {loadingReducer} from "./shared/loading";
import {deviceSimulatorReducer} from "./device-simulator/state";
import {discoveryReducer} from "./shared/discovery";

export const appReducerMap: ActionReducerMap<AppState> = {
  devices: devicesReducer,
  diagnostics: diagnosticsReducer,
  rooms: roomsReducer,
  loading: loadingReducer,
  deviceSimulator: deviceSimulatorReducer,
  deviceTypes: deviceTypesReducer,
  discovery: discoveryReducer
};
