import {SimulatedDeviceModel} from "../../shared/models";

export interface DeviceSimulatorState {
  devices: Array<SimulatedDeviceModel>;
  isConnected?: boolean;
}
