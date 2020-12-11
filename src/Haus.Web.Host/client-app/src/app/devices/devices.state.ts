import {DeviceModel} from "./models";

export interface DevicesState {
  devices: {[id: string]: DeviceModel;}
  allowDiscovery: boolean;
}
