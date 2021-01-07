import {DeviceModel} from "../models";
import {toTitleCase} from "./to-title-case";

export function getDeviceDisplayText(device: DeviceModel): string {
  return `(${toTitleCase(device.deviceType)}) ${device.name}`;
}
