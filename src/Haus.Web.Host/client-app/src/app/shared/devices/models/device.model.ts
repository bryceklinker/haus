import {DeviceMetadataModel} from "./device-metadata.model";

export interface DeviceModel {
  id: number;
  roomId?: number;
  externalId: string;
  name: string;
  metadata: Array<DeviceMetadataModel>;
  deviceType: string;
}
