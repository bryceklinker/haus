import {ActionReducerMap} from "@ngrx/store";
import {AppState} from "./app.state";
import {devicesReducer, deviceTypesReducer, lightTypesReducer} from "./devices/state";
import {diagnosticsReducer} from "./diagnostics/state";
import {roomsReducer} from "./rooms/state";
import {loadingReducer} from "./shared/loading";
import {deviceSimulatorReducer} from "./device-simulator/state";
import {discoveryReducer} from "./shared/discovery";
import {healthReducer} from "./health/state";
import {eventsReducer} from "./shared/events";
import {shellReducer} from "./shell/state";

export const appReducerMap: ActionReducerMap<AppState> = {
  devices: devicesReducer,
  diagnostics: diagnosticsReducer,
  rooms: roomsReducer,
  loading: loadingReducer,
  deviceSimulator: deviceSimulatorReducer,
  deviceTypes: deviceTypesReducer,
  discovery: discoveryReducer,
  lightTypes: lightTypesReducer,
  health: healthReducer,
  events: eventsReducer,
  shell: shellReducer
};
