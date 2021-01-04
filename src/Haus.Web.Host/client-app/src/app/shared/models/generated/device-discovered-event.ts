import {DeviceType} from './device-type';
import {MetadataModel} from './metadata-model';

export interface DeviceDiscoveredEvent {
	id: string;
	deviceType: DeviceType;
	metadata: Array<MetadataModel>;
}
