import {DiagnosticsEffects} from "./diagnostics/effects/diagnostics.effects";
import {DevicesEffects} from "./devices/effects/devices.effects";
import {RoomsEffects} from "./rooms/effects/rooms.effects";
import {DeviceSimulatorEffects} from "./device-simulator/effects/device-simulator.effects";
import {DeviceTypesEffects} from "./devices/effects/device-types.effects";
import {EventsEffects} from "./shared/events";
import {DiscoveryEffects} from "./shared/discovery";
import {LightTypesEffects} from "./devices/effects/light-types.effects";
import {HealthEffects} from "./health/effects/health.effects";
import {ShellEffects} from "./shell/effects/shell.effects";
import {AuthEffects} from "./shared/auth/effects/auth.effects";

export const APP_EFFECTS = [
  DevicesEffects,
  DiagnosticsEffects,
  RoomsEffects,
  DeviceSimulatorEffects,
  DeviceTypesEffects,
  EventsEffects,
  DiscoveryEffects,
  LightTypesEffects,
  HealthEffects,
  AuthEffects,
  ShellEffects
]
