import {MetadataModel} from "../../shared/models/metadata.model";

export interface CreateSimulatedDeviceModel {
  deviceType: string;
  metadata: Array<MetadataModel>;
}

export interface SimulatedDeviceModel extends CreateSimulatedDeviceModel {
  id: string;
}
