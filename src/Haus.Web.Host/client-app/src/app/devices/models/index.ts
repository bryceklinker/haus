import {DeviceModel, LightingConstraintsModel} from "../../shared/models";

export interface DeviceLightingConstraintsModel {
  device: DeviceModel;
  constraints: LightingConstraintsModel;
}
