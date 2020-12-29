import {EntityState} from "@ngrx/entity";
import {DeviceModel} from "../../shared/devices";

export interface DevicesState extends EntityState<DeviceModel> {
  allowDiscovery: boolean;
}
