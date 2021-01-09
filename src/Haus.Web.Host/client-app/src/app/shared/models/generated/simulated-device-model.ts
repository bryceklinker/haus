import {DeviceType} from './device-type';
import {MetadataModel} from './metadata-model';
import {LightingModel} from './lighting-model';

export interface SimulatedDeviceModel {
	id: string;
	deviceType: DeviceType;
	metadata: Array<MetadataModel>;
	lighting: LightingModel;
}
