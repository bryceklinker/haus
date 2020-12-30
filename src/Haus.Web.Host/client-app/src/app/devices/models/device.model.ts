import {MetadataModel} from "../../shared/models";

export interface DeviceModel {
  id: number;
  roomId?: number;
  externalId: string;
  name: string;
  metadata: Array<MetadataModel>;
  deviceType: string;
}
