import {DeviceType} from './device-type';
import {MetadataModel} from './metadata-model';

export interface SimulatedDeviceModel {
	id: string;
	deviceType: DeviceType;
	metadata: Array<MetadataModel>;
}
