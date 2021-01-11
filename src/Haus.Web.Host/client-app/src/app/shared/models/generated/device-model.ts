import {DeviceType} from './device-type';
import {LightType} from './light-type';
import {LightingModel} from './lighting-model';
import {MetadataModel} from './metadata-model';

export interface DeviceModel {
	id: number;
	roomId?: number;
	externalId: string;
	name: string;
	deviceType: DeviceType;
	lightType: LightType;
	lighting: LightingModel;
	metadata: Array<MetadataModel>;
}
