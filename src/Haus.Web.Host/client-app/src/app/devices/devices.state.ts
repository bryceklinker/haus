import {DeviceModel} from "./models";

export interface DevicesState {
  devices: {[id: number]: DeviceModel;}
  allowDiscovery: boolean;
}
