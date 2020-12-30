import {SimulatedDeviceModel} from "../models";

export interface DeviceSimulatorState {
  devices: Array<SimulatedDeviceModel>;
  isConnected?: boolean;
}
