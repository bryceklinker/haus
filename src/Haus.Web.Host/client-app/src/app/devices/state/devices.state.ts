import {EntityState} from "@ngrx/entity";
import {DeviceModel} from "../models";

export interface DevicesState extends EntityState<DeviceModel> {
  allowDiscovery: boolean;
}
