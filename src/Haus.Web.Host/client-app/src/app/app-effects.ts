import {DiagnosticsEffects} from "./diagnostics/effects/diagnostics.effects";
import {DevicesEffects} from "./devices/effects/devices.effects";
import {RoomsEffects} from "./rooms/effects/rooms.effects";

export const APP_EFFECTS = [
  DevicesEffects,
  DiagnosticsEffects,
  RoomsEffects
]
