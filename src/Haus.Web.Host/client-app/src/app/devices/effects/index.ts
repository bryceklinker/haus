import {DiscoveryEffects} from "./discovery.effects";
import {LoadDevicesEffects} from "./load-devices.effects";
import {TurnDeviceOffEffects} from "./turn-device-off.effects";
import {TurnDeviceOnEffects} from "./turn-device-on.effects";

export const DEVICES_EFFECTS = [
  DiscoveryEffects,
  LoadDevicesEffects,
  TurnDeviceOnEffects,
  TurnDeviceOffEffects
]
