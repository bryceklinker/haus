import {DeviceType} from './device-type';
import {MetadataModel} from './metadata-model';

export interface CreateSimulatedDeviceModel {
	deviceType: DeviceType;
	metadata: Array<MetadataModel>;
}
