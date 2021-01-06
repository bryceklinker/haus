import {DeviceType} from './device-type';
import {MetadataModel} from './metadata-model';
import {LightingModel} from './lighting-model';

export interface DeviceModel {
	id: number;
	roomId?: number;
	externalId: string;
	name: string;
	deviceType: DeviceType;
	metadata: Array<MetadataModel>;
	lighting: LightingModel;
}