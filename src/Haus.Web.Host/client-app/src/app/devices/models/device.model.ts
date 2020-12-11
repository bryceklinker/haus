import {DeviceMetadataModel} from "./device-metadata.model";

export type DeviceType = 'Unknown' | 'Light' | 'LightSensor' | 'MotionSensor' | 'TemperatureSensor' | 'Switch';

export interface DeviceModel {
  id: number;
  externalId: string;
  name: string;
  metadata: Array<DeviceMetadataModel>;
  deviceType: Array<DeviceType>;
}
