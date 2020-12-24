import {DeviceModel} from "../../shared/devices";

export interface DeviceSimulatorStateModel {
  devicesById: {[id: string]: DeviceModel};
  devices: Array<DeviceModel>;
}
