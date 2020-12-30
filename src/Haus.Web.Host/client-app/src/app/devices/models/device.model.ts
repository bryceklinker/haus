import {MetadataModel} from "./metadata.model";

export interface DeviceModel {
  id: number;
  roomId?: number;
  externalId: string;
  name: string;
  metadata: Array<MetadataModel>;
  deviceType: string;
}
