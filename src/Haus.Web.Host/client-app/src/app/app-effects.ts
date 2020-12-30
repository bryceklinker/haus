import {DiagnosticsEffects} from "./diagnostics/effects/diagnostics.effects";
import {DevicesEffects} from "./devices/effects/devices.effects";
import {RoomsEffects} from "./rooms/effects/rooms.effects";
import {DeviceSimulatorEffects} from "./device-simulator/effects/device-simulator.effects";
import {DeviceTypesEffects} from "./devices/effects/device-types.effects";

export const APP_EFFECTS = [
  DevicesEffects,
  DiagnosticsEffects,
  RoomsEffects,
  DeviceSimulatorEffects,
  DeviceTypesEffects
]
