import {DeviceMetadataModel} from "./device-metadata.model";

export interface DeviceModel {
  id: number;
  externalId: string;
  metadata: Array<DeviceMetadataModel>;
}
